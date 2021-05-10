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
        private readonly static Regex gColorValidator = new Regex("#[0-9a-fA-F]{6}");

        internal static Color ParseColor(string color)
        {
            if (color.StartsWith("#"))
            {
                if (!gColorValidator.IsMatch(color))
                    throw new ArgumentException($"Color {color} is not a correct HTML color", nameof(color));
                return Color.FromHtml(color);
            }
            else
            {
                var colors = ColorDictionary.Instance;
                if (colors.IsDefined(color))
                    return colors[color];
                throw new ArgumentException($"Color {color} name is not know", nameof(color));
            }
        }
        //12      3          4
        private readonly static Regex gXUnitValidator = new Regex(@"^((-?\d+)(\.(\d*))?)(pt|mm|in|cm|px|%)$");

        internal static XUnit ParseUnit(string unit)
        {
            if (unit == "0")
                return XUnit.Zero;

            Match m = gXUnitValidator.Match(unit);
            if (!m.Success)
                throw new ArgumentException($"Unit {unit} is not a unit expression", nameof(unit));

            float n = float.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);

            MeasurementUnit mm = m.Groups[5].Value switch
            {
                "pt" => MeasurementUnit.Point,
                "cm" => MeasurementUnit.Centimeter,
                "mm" => MeasurementUnit.Millimeter,
                "in" => MeasurementUnit.Inch,
                "px" => MeasurementUnit.Pixel,
                "%" => MeasurementUnit.Percent,
                _ => throw new ArgumentException($"Unknown unit {m.Groups[4].Value}")
            };
            return new XUnit(n, mm);
        }

        internal static XUnit ParseUnitOrDefault(string unit, XUnit? defaultValue = null)
        {
            if (string.IsNullOrEmpty(unit))
                return defaultValue ?? XUnit.Zero;

            return ParseUnit(unit);
        }

        private static HorizontalAlignment ToPDF(XmlPdfHorizontalAlignment alignment, out bool justify)
        {
            justify = alignment == XmlPdfHorizontalAlignment.justify;
            return alignment switch
            {
                XmlPdfHorizontalAlignment.left => HorizontalAlignment.Left,
                XmlPdfHorizontalAlignment.center => HorizontalAlignment.Center,
                XmlPdfHorizontalAlignment.right => HorizontalAlignment.Right,
                XmlPdfHorizontalAlignment.justify => HorizontalAlignment.Left,
                _ => throw new ArgumentException($"Horizontal alignment value {alignment} is not supported", nameof(alignment))
            };
        }

        private static VerticalAlignment ToPDF(XmlPdfVerticalAlignment alignment) =>
            alignment switch
            {
                XmlPdfVerticalAlignment.top => VerticalAlignment.Top,
                XmlPdfVerticalAlignment.center => VerticalAlignment.Center,
                XmlPdfVerticalAlignment.bottom => VerticalAlignment.Bottom,
                _ => throw new ArgumentException($"Vertical alignment value {alignment} is not supported", nameof(alignment))
            };

        private static PageBreak ToPDF(XmlPdfNewPageMode mode) =>
            mode switch
            {
                XmlPdfNewPageMode.no or XmlPdfNewPageMode.@false or XmlPdfNewPageMode.flow => PageBreak.InFlow,
                XmlPdfNewPageMode.yes or XmlPdfNewPageMode.@true or XmlPdfNewPageMode.newpage => PageBreak.NextPage,
                XmlPdfNewPageMode.newevenpage => PageBreak.NextEvenPage,
                XmlPdfNewPageMode.newoddpage => PageBreak.NextOddPage,
                _ => PageBreak.InFlow
            };

        private static Stroke ToPDF(XmlPdfStroke stroke) =>
            stroke switch
            {
                XmlPdfStroke.none => Stroke.None,
                XmlPdfStroke.solid => Stroke.Solid,
                XmlPdfStroke.dash => Stroke.Dashed,
                XmlPdfStroke.dot => Stroke.Dotted,
                XmlPdfStroke.@double => Stroke.Double,
                _ => throw new ArgumentException($"Unsupported stroke {stroke}", nameof(stroke))
            };

        internal static ListType ToPDF(XmlPdfListStyle style) =>
            style switch
            {
                XmlPdfListStyle.bullet => ListType.Bulleted,
                XmlPdfListStyle.number => ListType.Numbered,
                _ => throw new ArgumentException($"Unsupported list style {style}", nameof(style))
            };

        internal static NumerationStyle ToPDF(XmlPdfNumerationStyle style) =>
            style switch
            {
                XmlPdfNumerationStyle.arabic => NumerationStyle.Arabic,
                XmlPdfNumerationStyle.lowerlatin => NumerationStyle.LowerLatin,
                XmlPdfNumerationStyle.lowerroman => NumerationStyle.LowerRoman,
                XmlPdfNumerationStyle.upperlatin => NumerationStyle.UpperLatin,
                XmlPdfNumerationStyle.upperroman => NumerationStyle.UpperRoman,
                XmlPdfNumerationStyle.lowercyrillic => NumerationStyle.LowerCyrillic,
                XmlPdfNumerationStyle.uppercyrillic => NumerationStyle.UpperCyrillic,
                _ => throw new ArgumentException($"Unsupported numeration style {style}", nameof(style))
            };

        internal static ListBullet ToPDF(XmlPdfListBulletStyle style) =>
            style switch
            {
                XmlPdfListBulletStyle.bullet => ListBullet.Bullet,
                XmlPdfListBulletStyle.dash => ListBullet.Dash,
                _ => throw new ArgumentException($"Unsupported bullet style {style}", nameof(style))
            };

        internal static TextOverflowAction ToPDF(XmlPdfOverflowAction action) =>
            action switch
            {
                XmlPdfOverflowAction.truncate => TextOverflowAction.Truncate,
                XmlPdfOverflowAction.ellipsis => TextOverflowAction.Ellipsis,
                XmlPdfOverflowAction.@throw => TextOverflowAction.Throw,
                _ => throw new ArgumentException($"Unsupported overflow action {action}", nameof(action))
            };

        internal static Box ToPDF(XmlPdfBox xmlBox)
        {
            XUnit left, right, top, bottom;

            left = ParseUnitOrDefault(xmlBox.left);
            right = ParseUnitOrDefault(xmlBox.right);
            top = ParseUnitOrDefault(xmlBox.top);
            bottom = ParseUnitOrDefault(xmlBox.bottom);

            return new Box(left, top, right, bottom);
        }

        internal static PageOrientation ToPDF(XmlPdfPageOrientation orientation) =>
            orientation switch
            {
                XmlPdfPageOrientation.landscape => PageOrientation.Landscape,
                XmlPdfPageOrientation.portrait => PageOrientation.Portrait,
                _ => throw new ArgumentException($"Unsupported orientation {orientation}", nameof(orientation))
            };
    }
}

