using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// The class to read and parse XML model and generate PDF document.
    /// </summary>
    public sealed partial class Creator
    {
        private DocumentBuilder mDocumentBuilder;

        /// <summary>
        /// Saves the document to the file specified.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            if (mDocumentBuilder == null)
                throw new InvalidOperationException("No model is processed yet to build a document");
            mDocumentBuilder.Build(fileName);
        }

        /// <summary>
        /// Processes XML with the model
        /// </summary>
        /// <param name="xml">XML document with the model</param>
        /// <param name="errorAction">Error action</param>
        public void ProcessXml(string xml, XmlPdfLoadActionDelegate errorAction)
        {
            var document = Loader.LoadXmlPdfDocument(xml, errorAction);
            ProcessModel(document);
        }

        /// <summary>
        /// Processes stream that contains XML model
        /// </summary>
        /// <param name="stream">The stream object</param>
        /// <param name="encoding">Encoding to be used to read the text</param>
        /// <param name="errorAction">The XML/schema error action callback</param>
        public void ProcessStream(Stream stream, Encoding encoding, XmlPdfLoadActionDelegate errorAction)
        {
            byte[] content = new byte[stream.Length];
            stream.Read(content, 0, content.Length);
            ProcessXml(encoding.GetString(content), errorAction);
        }

        /// <summary>
        /// Processes file that contains XML model
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="encoding">Encoding to be used to read the text</param>
        /// <param name="errorAction">The XML/schema error action callback</param>
        public void ProcessFile(string fileName, Encoding encoding, XmlPdfLoadActionDelegate errorAction)
        {
            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096);
            ProcessStream(stream, encoding, errorAction);
        }

        /// <summary>
        /// Processes the model
        /// </summary>
        /// <param name="document">The model</param>
        internal void ProcessModel(XmlPdfDocument document)
        {
            if (mDocumentBuilder != null)
                mDocumentBuilder = DocumentBuilder.New();
            Variables variables = new Variables();
            variables["builder"] = mDocumentBuilder;

            var actions = Compile(document);

            foreach (var action in actions)
                action.Execute(variables);
        }

        internal static Queue<CallAction> Compile(XmlPdfDocument document)
        {
            Queue<CallAction> actions = new Queue<CallAction>();

            if (document.styles != null)
                foreach (var style in document.styles)
                    HandleStyle(actions, style);

            return actions;
        }

        private static void HandleStyle(Queue<CallAction> actions, XmlPdfStyle style)
        {
            CallAction action;

            //create a style
            var builderName = $"{style.name}_styleBuilder";
            if (string.IsNullOrEmpty(style.basestyle))
            {
                action = CallAction.Let<StyleBuilder>(builderName, null, _ => StyleBuilder.New(null));
                actions.Enqueue(action);
            }
            else
            {
                var parentName = $"{style.basestyle}_styleBuilder";
                action = CallAction.Let<StyleBuilder>(builderName, null, _ => StyleBuilder.New((new Variable<StyleBuilder>(parentName)).Value));
                actions.Enqueue(action);
            }

            if (style.backgroundcolor != null)
            {
                var color = ParseColor(style.backgroundcolor);
                action = CallAction.Call<StyleBuilder>(builderName, builder => builder.SetBackColor(color));
                actions.Enqueue(action);
            }

            if (style.pagebreakSpecified)
            {
                action = CallAction.Call<StyleBuilder>(builderName, builder => builder.SetPageBreak(style.pagebreak ? PageBreak.NextPage : PageBreak.InFlow));
                actions.Enqueue(action);
            }

            if (style.keepwithnextSpecified)
            {
                action = CallAction.Call<StyleBuilder>(builderName, builder => builder.SetKeepWithNext(style.keepwithnext));
                actions.Enqueue(action);
            }

            if (style.horizontalalignmentSpecified)
            {
                HorizontalAlignment alignment = style.horizontalalignment switch
                {
                    XmlPdfHorizontalAlignment.left => HorizontalAlignment.Left,
                    XmlPdfHorizontalAlignment.center => HorizontalAlignment.Center,
                    XmlPdfHorizontalAlignment.right => HorizontalAlignment.Right,
                    XmlPdfHorizontalAlignment.justify => HorizontalAlignment.Left,
                    _ => throw new InvalidDataException($"Horizontal alignment value {style.horizontalalignment} is not supported")
                };

                action = CallAction.Call<StyleBuilder>(builderName, builder => builder.SetHorizontalAlignment(alignment));
                actions.Enqueue(action);

                action = CallAction.Call<StyleBuilder>(builderName, builder => builder.SetJustifyAlignment(style.horizontalalignment == XmlPdfHorizontalAlignment.justify));
                actions.Enqueue(action);
            }

            if (style.verticalalignmentSpecified)
            {
                VerticalAlignment alignment = style.verticalalignment switch
                {
                    XmlPdfVerticalAlignment.top => VerticalAlignment.Top,
                    XmlPdfVerticalAlignment.center => VerticalAlignment.Center,
                    XmlPdfVerticalAlignment.bottom => VerticalAlignment.Bottom,
                    _ => throw new InvalidDataException($"Vertical alignment value {style.horizontalalignment} is not supported")
                };

                action = CallAction.Call<StyleBuilder>(builderName, builder => builder.SetVerticalAlignment(alignment));
                actions.Enqueue(action);
            }

            if (style.linespacingSpecified)
            {
                action = CallAction.Call<StyleBuilder>(builderName, builder => builder.SetLineSpacing(style.linespacing));
                actions.Enqueue(action);
            }
        }

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
    }
}
