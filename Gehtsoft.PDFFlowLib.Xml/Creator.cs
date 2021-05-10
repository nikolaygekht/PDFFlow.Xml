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
            variables["documentBuilder"] = mDocumentBuilder;

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

            if (document.sections != null)
                foreach (var section in document.sections)
                    HandleSection(actions, section);

            return actions;
        }

        internal static void HandleFontBuilder(XmlPdfFont font, Queue<CallAction> actions, string fontBuilderName)
        {
            actions.Let<FontBuilder>(fontBuilderName, null, _ => FontBuilder.New());
            if (!string.IsNullOrEmpty(font.name))
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetName(font.name));
            if (!string.IsNullOrEmpty(font.encoding))
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetEncodingName(font.encoding));
            if (!string.IsNullOrEmpty(font.size))
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetSize(ParseUnit(font.size).Point));
            if (!string.IsNullOrEmpty(font.color))
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetColor(ParseColor(font.color)));
            if (font.boldSpecified)
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetBold(font.bold));
            if (font.italicSpecified)
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetItalic(font.italic));
            if (font.obliqueSpecified)
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetOblique(font.oblique));

            if (font.underlineSpecified && font.underline)
            {
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetUnderline());
                if (!string.IsNullOrEmpty(font.underlinecolor))
                    actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetUnderlineColor(ParseColor(font.underlinecolor)));
            }

            if (font.strikethroughSpecified && font.strikethrough)
            {
                actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetStrikethrough());
                if (!string.IsNullOrEmpty(font.strikethroughcolor))
                    actions.Call<FontBuilder>(fontBuilderName, builder => builder.SetStrikethroughColor(ParseColor(font.strikethroughcolor)));
            }
        }
    }
}
