using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Gehtsoft.PDFFlowLib.Xml.Schema;

namespace Gehtsoft.PDFFlowLib.Xml
{
    /// <summary>
    /// Callback for validating the schema and the source Xml to PDF document
    /// </summary>
    /// <param name="location">Location of the problem: `schema` or `document`</param>
    /// <param name="serverty">The problem severity</param>
    /// <param name="message">The message</param>
    /// <param name="innerException">The inner exception (if any)</param>
    public delegate void XmlPdfLoadActionDelegate(string location, XmlSeverityType serverty, string message, Exception innerException);

    /// <summary>
    /// XML loader for the resources.
    /// </summary>
    internal static class Loader
    {
        /// <summary>
        /// Loads the schema of XML to PDF file from the resources
        /// </summary>
        /// <returns></returns>
        internal static XmlSchema LoadSchema(XmlPdfLoadActionDelegate errorAction)
        {
            using (var stream = typeof(Loader).Assembly.GetManifestResourceStream("Gehtsoft.PDFFlowLib.Xml.schema.xsd"))
            {
                if (stream == null)
                    throw new FileNotFoundException("The schema resource is not found (Gehtsoft.PDFFlowLib.Xml.schema.xsd)");

                using (var reader = new StreamReader(stream))
                {
                    var xml = reader.ReadToEnd();
                    using var stringReader = new StringReader(xml);
                    return XmlSchema.Read(stringReader, (obj, args) => errorAction?.Invoke("schema", args.Severity, args.Message, args.Exception));
                }
            }
        }

        internal static XmlPdfDocument LoadXmlPdfDocument(string document, XmlPdfLoadActionDelegate errorAction)
        {
            using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(document));
            return LoadXmlPdfDocument(ms, errorAction);
        }

        /// <summary>
        /// Loads and validates the document from the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="errorAction"></param>
        /// <returns></returns>
        internal static XmlPdfDocument LoadXmlPdfDocument(Stream stream, XmlPdfLoadActionDelegate errorAction)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlPdfDocument));
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(LoadSchema(errorAction));

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Schemas = schemas,
                ConformanceLevel = ConformanceLevel.Document,
                ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ReportValidationWarnings,
                ValidationType = ValidationType.Schema,
            };

            settings.ValidationEventHandler += (obj, args) => errorAction?.Invoke("document", args.Severity, args.Message, args.Exception);

            using XmlReader xmlReader = XmlReader.Create(stream, settings);

            XmlDeserializationEvents events = new XmlDeserializationEvents
            {
                OnUnknownAttribute = (obj, args) => errorAction?.Invoke("document", XmlSeverityType.Error, $"({args.LineNumber}:{args.LinePosition}) - Attribute expected {args.ExpectedAttributes}", null),
                OnUnknownElement = (obj, args) => errorAction?.Invoke("document", XmlSeverityType.Error, $"({args.LineNumber}:{args.LinePosition}) - Elements expected {args.ExpectedElements}", null),
                OnUnknownNode = (obj, args) => errorAction?.Invoke("document", XmlSeverityType.Error, $"({args.LineNumber}:{args.LinePosition}) - Unknown node {args.Name}", null),
                OnUnreferencedObject = (obj, args) => errorAction?.Invoke("document", XmlSeverityType.Error, $"Unreferenced object {args.UnreferencedId}", null)
            };

            return xmlSerializer.Deserialize(xmlReader, events) as XmlPdfDocument;
        }
    }
}
