using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gehtsoft.PDFFlow.Builder;
using Gehtsoft.PDFFlow.Models.Enumerations;
using Gehtsoft.PDFFlow.Models.Shared;
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

    }
}
