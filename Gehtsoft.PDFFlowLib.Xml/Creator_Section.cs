using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
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
        private const string SECTIONBUILDER = "sectionBuilder";
        private const string AREABUILDER = "areaBuilder";

        private static void HandleSection(Queue<CallAction> actions, XmlPdfSection section)
        {
            actions.Let<SectionBuilder>(SECTIONBUILDER, null, _ => SectionBuilder.New((new Variable<DocumentBuilder>("documentBuilder")).Value));

            if (section.layout != null)
                HandleLayout(actions, section.layout);
        }

        private static void HandleLayout(Queue<CallAction> actions, XmlPdfLayout layout)
        {
            HandlePage(actions, layout.page);

            if (layout.margins != null)
                actions.Call<SectionBuilder>(SECTIONBUILDER, builder => builder.SetMargins(ToPDF(layout.margins)));

            if (layout.pagenumeration != null)
                HandlePageNumeration(actions, layout.pagenumeration);

            if (layout.area?.Length > 0)
                foreach (var area in layout.area)
                    HandleArea(actions, area);
        }

        private static void HandlePageNumeration(Queue<CallAction> actions, XmlPdfPageNumeration pageNumeration)
        {
            if (pageNumeration.startfromSpecified)
                actions.Call<SectionBuilder>(SECTIONBUILDER, builder => builder.SetPageNumberStart((uint)pageNumeration.startfrom));

            if (pageNumeration.typeSpecified)
                actions.Call<SectionBuilder>(SECTIONBUILDER, builder => builder.SetNumerationStyle(ToPDF(pageNumeration.type)));
        }

        private static void HandlePage(Queue<CallAction> actions, XmlPdfPage page)
        {
            var orientation = ToPDF(page.orientation);

            XSize size;

            switch (page.size)
            {
                case XmlPdfPageSize.A0:
                    size = new XSize(2384, 3370);
                    break;
                case XmlPdfPageSize.A1:
                    size = new XSize(1684, 2384);
                    break;
                case XmlPdfPageSize.A2:
                    size = new XSize(1191, 1684);
                    break;
                case XmlPdfPageSize.A3:
                    size = new XSize(842, 1191);
                    break;
                case XmlPdfPageSize.A4:
                    size = new XSize(595, 842);
                    break;
                case XmlPdfPageSize.A5:
                    size = new XSize(420, 595);
                    break;
                case XmlPdfPageSize.halfletter:
                    size = new XSize(396, 612);
                    break;
                case XmlPdfPageSize.juniorlegal:
                    size = new XSize(360, 576);
                    break;
                case XmlPdfPageSize.ledger:
                    size = new XSize(792, 1224);
                    break;
                case XmlPdfPageSize.letter:
                    size = new XSize(612, 792);
                    break;
                case XmlPdfPageSize.legal:
                    size = new XSize(612, 1008);
                    break;
                case XmlPdfPageSize.custom:
                    if (string.IsNullOrEmpty(page.width) || string.IsNullOrEmpty(page.height))
                        throw new ArgumentException("For the custom page size, both width and height attributes must be define", nameof(page));
                    size = new XSize(ParseUnit(page.width).Point, ParseUnit(page.height).Point);
                    break;
                default:
                    throw new ArgumentException($"The page size {page.size} is not supported", nameof(page));
            }

            if (orientation == PageOrientation.Landscape && size.Width.Point < size.Height.Point)
                size = new XSize(size.Height.Point, size.Width.Point);

            actions.Call<SectionBuilder>(SECTIONBUILDER, builder => builder.SetSize(size));
        }

        private static void HandleArea(Queue<CallAction> actions, XmlPdfPageArea area)
        {
            Box? box = null;
            XUnit? _height = null;
            float top, left, right, bottom, width, height;

            top = left = width = height = 0;

            if (area.location != null)
            {
                box = ToPDF(area.location);

                top = box.Value.Top.Point;
                left = box.Value.Left.Point;
                right = box.Value.Right.Point;
                bottom = box.Value.Bottom.Point;

                width = right - left;
                height = bottom - top;
            }
            if (!string.IsNullOrEmpty(area.height))
            {
                _height = ParseUnit(area.height);
                height = _height.Value.Point;
            }

            if (box == null && _height == null)
                throw new ArgumentException("Either location or height parameters must be specified for the area", nameof(area));

            if ((area.type == XmlPdfAreaType.flow || area.type == XmlPdfAreaType.@fixed) && box == null)
                throw new ArgumentException("The full location must be set for the flow or fixed area", nameof(area));

            if ((area.type == XmlPdfAreaType.footer || area.type == XmlPdfAreaType.header) && _height == null)
                throw new ArgumentException("The height must be set for the header and footer area", nameof(area));

            switch (area.type)
            {
                case XmlPdfAreaType.flow:
                    {
                        switch (area.page)
                        {
                            case XmlPdfPageSide.single:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddDocumentFlowAreaToBothPages(left, top, width, height));
                                break;
                            case XmlPdfPageSide.odd:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddDocumentFlowAreaToOddPage(left, top, width, height));
                                break;
                            case XmlPdfPageSide.even:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddDocumentFlowAreaToEvenPage(left, top, width, height));
                                break;
                            default:
                                throw new ArgumentException($"Not supported page {area.page} in area", nameof(area));
                        }
                    }
                    break;
                case XmlPdfAreaType.header:
                    {
                        switch (area.page)
                        {
                            case XmlPdfPageSide.single:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddHeaderToBothPages(height, null));
                                break;
                            case XmlPdfPageSide.odd:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddHeaderToOddPage(height, null));
                                break;
                            case XmlPdfPageSide.even:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddHeaderToEvenPage(height, null));
                                break;
                            default:
                                throw new ArgumentException($"Not supported page {area.page} in area", nameof(area));
                        }
                    }
                    break;
                case XmlPdfAreaType.footer:
                    {
                        switch (area.page)
                        {
                            case XmlPdfPageSide.single:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddFooterToBothPages(height, null));
                                break;
                            case XmlPdfPageSide.odd:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddFooterToOddPage(height, null));
                                break;
                            case XmlPdfPageSide.even:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddFooterToEvenPage(height, null));
                                break;
                            default:
                                throw new ArgumentException($"Not supported page {area.page} in area", nameof(area));
                        }
                    }
                    break;
                case XmlPdfAreaType.@fixed:
                    {
                        switch (area.page)
                        {
                            case XmlPdfPageSide.single:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddRptAreaToBothPages(left, top, width, height, null));
                                break;
                            case XmlPdfPageSide.odd:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddRptAreaToOddPage(left, top, width, height, null));
                                break;
                            case XmlPdfPageSide.even:
                                actions.Let<SectionBuilder>(AREABUILDER, SECTIONBUILDER, builder => builder.AddRptAreaToEvenPage(left, top, width, height, null));
                                break;
                            default:
                                throw new ArgumentException($"Not supported page {area.page} in area", nameof(area));
                        }
                    }
                    break;
                default:
                    throw new ArgumentException($"Not supported area type {area.type}", nameof(area));
            }
        }
    }
}
