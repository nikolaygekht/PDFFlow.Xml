using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gehtsoft.PDFFlow.Builder;
using Gehtsoft.PDFFlow.Models.Enumerations;
using Gehtsoft.PDFFlow.Models.Shared;
using Gehtsoft.PDFFlowLib.Xml.Actions;
using Gehtsoft.PDFFlowLib.Xml.Schema;
using Xunit;

namespace Gehtsoft.PDFFlowLib.Xml.Test.CreatorTest
{
    public class Styles
    {
        private static XmlPdfDocument ModelOf(string styleElements)
        {
            var doc = XmlLoader.LoadResourceAsString("MinimumValidDocument");
            doc = doc.Replace("<sections>",
                $"<styles>{styleElements}</styles><sections>");
            return Loader.LoadXmlPdfDocument(doc, null);
        }

        [Fact]
        public void BaseStyle()
        {
            var model = ModelOf("<style name='style1'/><style name='style2' base-style='style1'/>");
            var actions = Creator.Compile(model);

            const string style1Builder = "style1_styleBuilder";
            const string style2Builder = "style2_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallStatic<StyleBuilder>(nameof(StyleBuilder.New))
                                            .And
                                            .HaveNullParameter()
                                            .And
                                            .SaveTo(style1Builder))
                .And.Then
                .HaveAction(action => action.Should()
                                            .CallStatic<StyleBuilder>(nameof(StyleBuilder.New))
                                            .And
                                            .HaveVariableParameter<StyleBuilder>(style1Builder)
                                            .And
                                            .SaveTo(style2Builder));

            actions.Should()
                .HaveNoActions(action => action.Should().CallAnyOf(style1Builder));

            actions.Should()
                .HaveNoActions(action => action.Should().CallAnyOf(style2Builder));
        }

        [Theory]
        [InlineData("left", HorizontalAlignment.Left, false)]
        [InlineData("center", HorizontalAlignment.Center, false)]
        [InlineData("right", HorizontalAlignment.Right, false)]
        [InlineData("justify", HorizontalAlignment.Left, true)]
        public void HorizontalAlighment(string alignment, HorizontalAlignment alignment1, bool justify)
        {
            var model = ModelOf($"<style name='style1' horizontal-alignment='{alignment}'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetHorizontalAlignment))
                                            .And
                                            .HaveParameter(alignment1))
                .And
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetJustifyAlignment))
                                            .And
                                            .HaveParameter(justify));
        }

        [Theory]
        [InlineData("top", VerticalAlignment.Top)]
        [InlineData("center", VerticalAlignment.Center)]
        [InlineData("bottom", VerticalAlignment.Bottom)]
        public void VerticalAlighment(string alignment, VerticalAlignment alignment1)
        {
            var model = ModelOf($"<style name='style1' vertical-alignment='{alignment}'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetVerticalAlignment))
                                            .And
                                            .HaveParameter(alignment1));
        }

        [Theory]
        [InlineData("#123456", 0x12, 0x34, 0x56)]
        [InlineData("black", 0, 0, 0)]
        [InlineData("white", 255, 255, 255)]
        public void BackgrounColor(string color, byte r, byte g, byte b)
        {
            var model = ModelOf($"<style name='style1' background-color='{color}'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetBackColor))
                                            .And
                                            .HaveParameter<Color>(color => color.R == r && color.G == g && color.B == b));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void KeepWithNext(bool value)
        {
            var model = ModelOf($"<style name='style1' keep-with-next='{ (value ? "true" : "false") }'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetKeepWithNext))
                                            .And
                                            .HaveParameter(value));
        }

        [Theory]
        [InlineData("flow", PageBreak.InFlow)]
        [InlineData("new-page", PageBreak.NextPage)]
        [InlineData("new-even-page", PageBreak.NextEvenPage)]
        [InlineData("new-odd-page", PageBreak.NextOddPage)]
        public void NewPage(string mode1, PageBreak mode2)
        {
            var model = ModelOf($"<style name='style1' page-break='{ mode1 }'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetPageBreak))
                                            .And
                                            .HaveParameter(mode2));
        }

        [Theory]
        [InlineData("truncate", TextOverflowAction.Truncate)]
        [InlineData("ellipsis", TextOverflowAction.Ellipsis)]
        [InlineData("throw", TextOverflowAction.Throw)]
        public void OverflowAction(string mode1, TextOverflowAction mode2)
        {
            var model = ModelOf($"<style name='style1' overflow-action='{ mode1 }'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetParagraphTextOverflowAction))
                                            .And
                                            .HaveParameter(mode2));
        }

        [Fact]
        public void LineSpacing()
        {
            var model = ModelOf("<style name='style1' line-spacing='5.5'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetLineSpacing))
                                            .And
                                            .HaveParameter(5.5f));
        }

        [Fact]
        public void MinCellHeight()
        {
            var model = ModelOf("<style name='style1' min-cell-height='3.45mm'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetTableCellMinHeight))
                                            .And
                                            .HaveParameter<XUnit>(mu => Math.Abs(mu.Value - 3.45) < 1e-5 && mu.Type == MeasurementUnit.Millimeter));
        }

        [Theory]
        [InlineData("top='1.5pt'", nameof(StyleBuilder.SetMarginTop), 1.5, MeasurementUnit.Point)]
        [InlineData("left='0'", nameof(StyleBuilder.SetMarginLeft), 0, MeasurementUnit.Point)]
        [InlineData("bottom='-2.51mm'", nameof(StyleBuilder.SetMarginBottom), -2.51, MeasurementUnit.Millimeter)]
        [InlineData("right='20%'", nameof(StyleBuilder.SetMarginRight), 20, MeasurementUnit.Percent)]
        public void Margin(string side, string method, float u, MeasurementUnit t)
        {
            var model = ModelOf($"<style name='style1'><margin {side} /></style>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, method)
                                            .And
                                            .HaveParameter<XUnit>(mu => Math.Abs(mu.Value - u) < 1e-5 && mu.Type == t));
        }

        [Theory]
        [InlineData("top='1.5pt'", nameof(StyleBuilder.SetPaddingTop), 1.5, MeasurementUnit.Point)]
        [InlineData("left='0'", nameof(StyleBuilder.SetPaddingLeft), 0, MeasurementUnit.Point)]
        [InlineData("bottom='-2.51mm'", nameof(StyleBuilder.SetPaddingBottom), -2.51, MeasurementUnit.Millimeter)]
        [InlineData("right='20%'", nameof(StyleBuilder.SetPaddingRight), 20, MeasurementUnit.Percent)]
        public void Padding(string side, string method, float u, MeasurementUnit t)
        {
            var model = ModelOf($"<style name='style1'><padding {side} /></style>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, method)
                                            .And
                                            .HaveParameter<XUnit>(mu => Math.Abs(mu.Value - u) < 1e-5 && mu.Type == t));
        }

        [Theory]
        [InlineData("all stroke='solid' width='0.5pt' color='#123456'",
                    nameof(StyleBuilder.SetBorder), 0.5f, MeasurementUnit.Point, Stroke.Solid,
                                                    (byte)0x12, (byte)0x34, (byte)0x56)]

        [InlineData("all width='0.5pt' color='#123456'",
                    nameof(StyleBuilder.SetBorder), 0.5f, MeasurementUnit.Point, null,
                                                    (byte)0x12, (byte)0x34, (byte)0x56)]

        [InlineData("all stroke='solid' width='0.5pt'",
                    nameof(StyleBuilder.SetBorder), 0.5f, MeasurementUnit.Point, Stroke.Solid,
                                                    null, null, null)]

        [InlineData("all stroke='double' ",
                    nameof(StyleBuilder.SetBorder), null, null, Stroke.Double,
                                                    null, null, null)]

        [InlineData("right width='0.5pt' color='#123456'",
                    nameof(StyleBuilder.SetBorderRight), 0.5f, MeasurementUnit.Point, null,
                                                    (byte)0x12, (byte)0x34, (byte)0x56)]

        [InlineData("left stroke='dot' width='0.5pt' color='#123456'",
                    nameof(StyleBuilder.SetBorderLeft), 0.5f, MeasurementUnit.Point, Stroke.Dotted,
                                                    (byte)0x12, (byte)0x34, (byte)0x56)]

        [InlineData("bottom stroke='solid' width='1.23in'",
                    nameof(StyleBuilder.SetBorderBottom), 1.23f, MeasurementUnit.Inch, Stroke.Solid,
                                                    null, null, null)]

        public void Border(string side, string method, float? u, MeasurementUnit? t, Stroke ?s, byte? r, byte ?g, byte ?b)
        {
            var model = ModelOf($"<style name='style1'><border><{side}/></border></style>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action =>
                {
                    action.Should().CallInstance(style1Builder, method);

                    if (u != null && t != null)
                        action.Should().HaveParameterN<XUnit>(0, xu => Math.Abs(xu.Value - u.Value) < 1e-5 && xu.Type == t.Value);
                    else
                        action.Should().HaveNullParameterN(0);

                    if (s != null)
                        action.Should().HaveParameterN(1, s.Value);
                    else
                        action.Should().HaveNullParameterN(1);

                    if (r != null && g != null && b != null)
                        action.Should().HaveParameterN<Color>(2, c => c.R == r.Value && c.G == g.Value && c.B == b.Value);
                    else
                        action.Should().HaveNullParameterN(2);
                });
        }

        [Theory]
        [InlineData("name='font-name-here'", nameof(FontBuilder.SetName), "font-name-here", typeof(string))]
        [InlineData("encoding='encoding-name-here'", nameof(FontBuilder.SetEncodingName), "encoding-name-here", typeof(string))]
        [InlineData("size='15mm'", nameof(FontBuilder.SetSize), "15mm", typeof(XUnit))]
        [InlineData("color='grey'", nameof(FontBuilder.SetColor), "#808080", typeof(Color))]
        [InlineData("bold='true'", nameof(FontBuilder.SetBold), true, typeof(bool))]
        [InlineData("italic='false'", nameof(FontBuilder.SetItalic), false, typeof(bool))]
        [InlineData("oblique='true'", nameof(FontBuilder.SetOblique), true, typeof(bool))]
        public void Font(string font, string methodName, object value, Type valueType)
        {
            if (value is string s && valueType != typeof(string))
            {
                if (valueType == typeof(XUnit))
                    value = Creator.ParseUnit(s);
                else if (valueType == typeof(Color))
                    value = Creator.ParseColor(s);
            }

            var model = ModelOf($"<style name='style1'><font {font}/></style>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";
            string fontBuilder = $"{style1Builder}_font";

            actions.Should()
                .HaveAction(action => action.Should().SaveTo(fontBuilder)
                                          .And
                                  .CallStatic<FontBuilder>(nameof(FontBuilder.New)))
                .And.Then
                .HaveAction(action =>
                {
                    action.Should().CallInstance(fontBuilder, methodName)
                          .And
                          .HaveParameterN<object>(0, obj =>
                          {
                              if (value is null)
                                  return obj is Null;
                              if (value is Color s && obj is Color d)
                                  return s.R == d.R && s.G == d.G && s.B == d.B;
                              if (value is XUnit s1 && obj is XUnit d1)
                                  return Math.Abs(s1.Value - d1.Value) < 1e-5 && s1.Type == d1.Type;
                              if (value is XUnit s2 && obj is float d2)
                                  return Math.Abs(s2.Point - d2) < 1e-5;
                              return value.Equals(obj);
                          });
                })
                .And.Then
                .HaveAction(action =>
                    action.Should().CallInstance(style1Builder, nameof(StyleBuilder.SetFont))
                            .And
                            .HaveVariableParameter<FontBuilder>(fontBuilder));
        }

        /*
        [Fact]
        public void FirstLineIndent()
        {
            var model = ModelOf("<style name='style1' first-line-indent='1.23cm'/>");
            var actions = Creator.Compile(model);
            const string style1Builder = "style1_styleBuilder";

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallInstance(style1Builder, nameof(StyleBuilder.SetFirstLineIndent))
                                            .And
                                            .HaveParameter<XUnit>(mu => Math.Abs(mu.Value - 1.23) < 1e-5 && mu.Type == MeasurementUnit.Centimeter));
        }
        */
    }
}

