using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Gehtsoft.PDFFlow.Builder;
using Gehtsoft.PDFFlow.Models.Enumerations;
using Gehtsoft.PDFFlow.Models.Shared;
using Gehtsoft.PDFFlowLib.Xml.Actions;
using Gehtsoft.PDFFlowLib.Xml.Schema;

namespace Gehtsoft.PDFFlowLib.Xml
{
    public sealed partial class Creator
    {
        private static void HandleStyle(Queue<CallAction> actions, XmlPdfStyle style)
        {
            //create a style
            var builderName = $"{style.name}_styleBuilder";
            if (string.IsNullOrEmpty(style.basestyle))
                actions.Let<StyleBuilder>(builderName, null, _ => StyleBuilder.New(null));
            else
                actions.Let<StyleBuilder>(builderName, null, _ => StyleBuilder.New(Variable<StyleBuilder>.Ref($"{style.basestyle}_styleBuilder").Value));

            if (style.backgroundcolor != null)
            {
                var color = ParseColor(style.backgroundcolor);
                actions.Call<StyleBuilder>(builderName, builder => builder.SetBackColor(color));
            }

            if (style.pagebreakSpecified)
                actions.Call<StyleBuilder>(builderName, builder => builder.SetPageBreak(ToPDF(style.pagebreak)));

            if (style.keepwithnextSpecified)
                actions.Call<StyleBuilder>(builderName, builder => builder.SetKeepWithNext(style.keepwithnext));

            if (style.horizontalalignmentSpecified)
            {
                HorizontalAlignment alignment = ToPDF(style.horizontalalignment, out bool justify);
                actions.Call<StyleBuilder>(builderName, builder => builder.SetHorizontalAlignment(alignment));
                actions.Call<StyleBuilder>(builderName, builder => builder.SetJustifyAlignment(justify));
            }

            if (style.verticalalignmentSpecified)
                actions.Call<StyleBuilder>(builderName, builder => builder.SetVerticalAlignment(ToPDF(style.verticalalignment)));

            if (style.linespacingSpecified)
                actions.Call<StyleBuilder>(builderName, builder => builder.SetLineSpacing(style.linespacing));

            /*
            if (!string.IsNullOrEmpty(style.firstlineindent))
                actions.Call<StyleBuilder>(builderName, builder => builder.SetFirstLineIndent(ParseUnit(style.firstlineindent)));
            */

            if (style.overflowactionSpecified)
                actions.Call<StyleBuilder>(builderName, builder => builder.SetParagraphTextOverflowAction(ToPDF(style.overflowaction)));

            if (!string.IsNullOrEmpty(style.mincellheight))
                actions.Call<StyleBuilder>(builderName, builder => builder.SetTableCellMinHeight(ParseUnit(style.mincellheight)));

            if (style.margin != null)
            {
                if (!string.IsNullOrEmpty(style.margin.top))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetMarginTop(ParseUnit(style.margin.top)));
                if (!string.IsNullOrEmpty(style.margin.left))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetMarginLeft(ParseUnit(style.margin.left)));
                if (!string.IsNullOrEmpty(style.margin.bottom))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetMarginBottom(ParseUnit(style.margin.bottom)));
                if (!string.IsNullOrEmpty(style.margin.right))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetMarginRight(ParseUnit(style.margin.right)));
            }

            if (style.padding != null)
            {
                if (!string.IsNullOrEmpty(style.padding.top))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetPaddingTop(ParseUnit(style.padding.top)));
                if (!string.IsNullOrEmpty(style.padding.left))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetPaddingLeft(ParseUnit(style.padding.left)));
                if (!string.IsNullOrEmpty(style.padding.bottom))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetPaddingBottom(ParseUnit(style.padding.bottom)));
                if (!string.IsNullOrEmpty(style.padding.right))
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetPaddingRight(ParseUnit(style.padding.right)));
            }

            if (style.border != null)
            {
                HandleStyle_Border(style.border.all, actions, builderName, "");
                HandleStyle_Border(style.border.top, actions, builderName, "Top");
                HandleStyle_Border(style.border.left, actions, builderName, "Left");
                HandleStyle_Border(style.border.right, actions, builderName, "Right");
                HandleStyle_Border(style.border.bottom, actions, builderName, "Bottom");
            }

            if (style.font != null)
            {
                var fontBuilderName = $"{builderName}_font";
                HandleFontBuilder(style.font, actions, fontBuilderName);
                actions.Call<StyleBuilder>(builderName, builder => builder.SetFont(Variable<FontBuilder>.Ref(fontBuilderName).Value));
            }

            if (style.list != null)
            {
                if (style.list.style == XmlPdfListStyle.bullet)
                {
                    ListBullet bullet;
                    if (style.list.bulletstyleSpecified)
                        bullet = ToPDF(style.list.bulletstyle);
                    else
                        bullet = ListBullet.Bullet;
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetListTypeBulleted(bullet));
                }
                else if (style.list.style == XmlPdfListStyle.number)
                {
                    NumerationStyle numeration;
                    if (style.list.numerationstyleSpecified)
                        numeration = ToPDF(style.list.numerationstyle);
                    else
                        numeration = NumerationStyle.Arabic;
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetListTypeNumbered(numeration));
                }

                if (style.list.leftindentSpecified)
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetListLevelLeftIndent(style.list.leftindent));

                if (style.list.levelSpecified)
                    actions.Call<StyleBuilder>(builderName, builder => builder.SetListLevel((uint)style.list.level));
            }
        }

        private static void HandleStyle_Border(XmlPdfBorderSide side, Queue<CallAction> actions, string builderName, string methodName)
        {
            if (side == null)
                return;
            object width = null;
            object stroke = null;
            object color = null;
            if (side.strokeSpecified)
                stroke = ToPDF(side.stroke);
            if (!string.IsNullOrEmpty(side.color))
                color = ParseColor(side.color);
            if (!string.IsNullOrEmpty(side.width))
                width = ParseUnit(side.width);

            if (stroke != null || color != null || width != null)
                actions.Call(builderName, $"SetBorder{methodName}", new object[] { width ?? (new Null<XUnit?>()), stroke ?? (new Null<Stroke?>()), color ?? (new Null<Color?>()) });
        }
    }
}
