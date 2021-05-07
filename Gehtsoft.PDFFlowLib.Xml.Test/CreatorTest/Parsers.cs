using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gehtsoft.PDFFlow.Models.Enumerations;
using Gehtsoft.PDFFlow.Models.Shared;
using Xunit;

namespace Gehtsoft.PDFFlowLib.Xml.Test.CreatorTest
{
    public class Parsers
    {
        [Theory]
        [InlineData("#000000", 0x0, 0x0, 0x0)]
        [InlineData("#ffffff", 0xff, 0xff, 0xff)]
        [InlineData("#FfFfFf", 0xff, 0xff, 0xff)]
        [InlineData("#fFfFfF", 0xff, 0xff, 0xff)]
        [InlineData("#123456", 0x12, 0x34, 0x56)]

        [InlineData("Black", 0x00, 0x00, 0x00)]
        [InlineData("black", 0x00, 0x00, 0x00)]
        [InlineData("BLACK", 0x00, 0x00, 0x00)]
        [InlineData("white", 0xff, 0xff, 0xff)]
        [InlineData("gray", 0x80, 0x80, 0x80)]
        [InlineData("grey", 0x80, 0x80, 0x80)]
        public void CorrectColor(string color, byte r, byte g, byte b)
        {
            var pdfColor = Creator.ParseColor(color);
            pdfColor.Should().NotBeNull();
            pdfColor.R.Should().Be(r);
            pdfColor.G.Should().Be(g);
            pdfColor.B.Should().Be(b);
        }

        [Theory]
        [InlineData("greey")]
        [InlineData("#aa")]
        [InlineData("agreyx")]
        [InlineData("xx#012345xx")]
        [InlineData("#aabbxx")]
        public void IncorrectColor(string color)
        {
            ((Action)(() => Creator.ParseColor(color))).Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("0", 0f, MeasurementUnit.Point)]
        [InlineData("0cm", 0f, MeasurementUnit.Centimeter)]
        [InlineData("1pt", 1f, MeasurementUnit.Point)]
        [InlineData("-1px", -1f, MeasurementUnit.Pixel)]
        [InlineData("-1.23mm", -1.23f, MeasurementUnit.Millimeter)]
        [InlineData("15.75cm", 15.75f, MeasurementUnit.Centimeter)]
        public void CorrectUnit(string text, float n, MeasurementUnit unit)
        {
            XUnit u = Creator.ParseUnit(text);
            u.Value.Should().Be(n);
            u.Type.Should().Be(unit);
        }

        [Theory]
        [InlineData("pt")]
        [InlineData("1")]
        [InlineData("x1pt")]
        [InlineData("cm1")]
        public void IncorrectUnit(string text)
        {
            ((Action)(() => Creator.ParseColor(text))).Should().Throw<ArgumentException>();
        }
    }
}


