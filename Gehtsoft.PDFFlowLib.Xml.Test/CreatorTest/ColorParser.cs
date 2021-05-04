using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Gehtsoft.PDFFlowLib.Xml.Test.CreatorTest
{
    public class ColorParser
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
        [InlineData("#aabbxx")]
        public void IncorrectColor(string color)
        {
            ((Action)(() => Creator.ParseColor(color))).Should().Throw<ArgumentException>();
        }
    }
}
