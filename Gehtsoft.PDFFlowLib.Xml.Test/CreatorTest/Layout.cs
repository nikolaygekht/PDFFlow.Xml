﻿using System;
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
    public class Layout
    {
        private const string SECTIONBUILDER = "sectionBuilder";
        private const string AREABUILDER = "areaBuilder";

        private static XmlPdfDocument ModelOf(string styleElements)
        {
            StringBuilder builder = new StringBuilder();
            builder
                .Append("<document xmlns='http://docs.gehtsoftusa.com/schemas/pdf2xsd.xsd'>")
                .Append("<sections>")
                .Append("<section>")
                .Append(styleElements)
                .Append("</section>")
                .Append("</sections>")
                .Append("</document>");
            return Loader.LoadXmlPdfDocument(builder.ToString(), null);
        }

        [Fact]
        public void CreateSectionBuilder()
        {
            var model = ModelOf("<layout><page size='letter' /><area type='header' height='1in'/></layout>");
            var actions = Creator.Compile(model);

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallStatic<SectionBuilder>(nameof(SectionBuilder.New))
                                            .And
                                            .HaveVariableParameter<DocumentBuilder>("documentBuilder")
                                            .And
                                            .SaveTo(SECTIONBUILDER))
                .And.Then
                .HaveAction(action => action.Should().CallAnyOf(SECTIONBUILDER));
        }

        [Fact]
        public void CreateAreaBuilder()
        {
            var model = ModelOf("<layout><page size='letter' /><area type='header' height='1in'/></layout>");
            var actions = Creator.Compile(model);

            actions.Should()
                .HaveAction(action => action.Should()
                                            .CallAnyOf(SECTIONBUILDER)
                                            .And
                                            .SaveTo(AREABUILDER));
        }

        [Theory]
        [InlineData("size='custom' height='5in' width='8in'", "8in", "5in")]
        [InlineData("size='custom' height='8in' width='5in' orientation='landscape'", "8in", "5in")]
        [InlineData("size='A4'", "210mm", "297mm")]
        [InlineData("size='A4' orientation='portrait'", "210mm", "297mm")]
        [InlineData("size='A4' orientation='landscape'", "297mm", "210mm")]
        [InlineData("size='letter'", "8.5in", "11in")]
        public void PageSize(string pageSpec, string width, string height)
        {
            var model = ModelOf($"<layout><page {pageSpec} /><area type='header' height='1in'/></layout>");
            var actions = Creator.Compile(model);

            actions.Should()
                .HaveAction(action => action.Should()
                                        .CallInstance(SECTIONBUILDER, nameof(SectionBuilder.SetSize))
                                        .And
                                        .HaveParameter<XSize>(param =>
                                            param.Width.IsWithinMMFrom(Creator.ParseUnit(width)) &&
                                            param.Height.IsWithinMMFrom(Creator.ParseUnit(height)))
                            );
        }

        [Theory]
        [InlineData("type='arabic' start-from='22'", 22, NumerationStyle.Arabic)]
        [InlineData("type='arabic'", null, NumerationStyle.Arabic)]
        [InlineData("start-from='22'", 22, null)]
        public void Numeration(string numerationSpec, int? startNumber, NumerationStyle? numerationStyle)
        {
            var model = ModelOf($"<layout><page size='A4' /><page-numeration {numerationSpec} /><area type='header' height='1in'/></layout>");
            var actions = Creator.Compile(model);

            if (startNumber != null)
            {
                actions.Should()
                    .HaveAction(action => action.Should()
                                    .CallInstance(SECTIONBUILDER, nameof(SectionBuilder.SetPageNumberStart))
                                    .And
                                    .HaveParameterN(0, (uint)startNumber.Value));
            }

            if (numerationStyle != null)
            {
                actions.Should()
                    .HaveAction(action => action.Should()
                                    .CallInstance(SECTIONBUILDER, nameof(SectionBuilder.SetNumerationStyle))
                                    .And
                                    .HaveParameterN(0, numerationStyle.Value));
            }
        }

        [Theory]
        [InlineData("left='10pt' top='0' right='10mm' bottom='1in'", "10pt", "0pt", "10mm", "1in")]
        public void Margins(string marginSpec, string left, string top, string right, string bottom)
        {
            var model = ModelOf($"<layout><page size='A4' /><margins {marginSpec} /><area type='header' height='1in'/></layout>");
            var actions = Creator.Compile(model);

            XUnit _left, _top, _bottom, _right;

            _left = Creator.ParseUnit(left);
            _top = Creator.ParseUnit(top);
            _right = Creator.ParseUnit(right);
            _bottom = Creator.ParseUnit(bottom);

            actions.Should()
                    .HaveAction(action => action.Should()
                                    .CallInstance(SECTIONBUILDER, nameof(SectionBuilder.SetMargins))
                                    .And
                                    .HaveParameterN<Box>(0, box =>
                                        box.Left.IsWithinMMFrom(_left) &&
                                        box.Right.IsWithinMMFrom(_right) &&
                                        box.Top.IsWithinMMFrom(_top) &&
                                        box.Bottom.IsWithinMMFrom(_bottom)));
        }
    }
}


