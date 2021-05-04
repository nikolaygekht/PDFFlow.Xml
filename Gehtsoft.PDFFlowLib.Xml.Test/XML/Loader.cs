using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using FluentAssertions;
using Xunit;

namespace Gehtsoft.PDFFlowLib.Xml.Test.Xml
{
    public class Loader
    {
        [Fact]
        public void LoadSchema()
        {
            bool hasError = false;
            ((Action)(() => PDFFlowLib.Xml.Loader.LoadSchema((_/* target */, _ /*severity*/, _ /*message*/, _ /*exception*/) => hasError = true))).Should().NotThrow();
            hasError.Should().BeFalse();
        }

        [Fact]
        public void ValidateUsingSchema_InvalidDocument()
        {
            var doc = XmlLoader.LoadResourceAsString("InvalidDocument1");
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(PDFFlowLib.Xml.Loader.LoadSchema(null));

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Schemas = schemas,
                ConformanceLevel = ConformanceLevel.Document,
                ValidationType = ValidationType.Schema,
            };

            bool fired = false;
            settings.ValidationEventHandler += (obj, args) => fired = true;

            using var ss = new MemoryStream(Encoding.UTF8.GetBytes(doc));
            using var r = XmlReader.Create(ss, settings);

            XmlDocument document = new XmlDocument();
            document.Load(r);
            fired.Should().BeTrue();
        }

        [Fact]
        public void ValidateUsingSchema_ValidDocument()
        {
            var doc = XmlLoader.LoadResourceAsString("MinimumValidDocument");
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(PDFFlowLib.Xml.Loader.LoadSchema(null));

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Schemas = schemas,
                ConformanceLevel = ConformanceLevel.Document,
                ValidationType = ValidationType.Schema,
            };

            bool fired = false;
            string message = null;
            settings.ValidationEventHandler += (obj, args) => { fired = true; message = args.Message; };

            using var ss = new MemoryStream(Encoding.UTF8.GetBytes(doc));
            using var r = XmlReader.Create(ss, settings);

            XmlDocument document = new XmlDocument();
            document.Load(r);
            fired.Should().BeFalse($"Document excepted to be valid, but \"{message}\" was fired.");
        }

        [Fact]
        public void LoadGoodDocument()
        {
            bool hasError = false;
            string receivedMessage = null;
            var doc = XmlLoader.LoadResourceAsString("MinimumValidDocument");
            Schema.XmlPdfDocument document = null;
            ((Action)(() => document = PDFFlowLib.Xml.Loader.LoadXmlPdfDocument(doc, (_ /*target*/, _ /*severity*/, message, _ /*exception*/) => { hasError = true; receivedMessage = message; }))).Should().NotThrow();
            hasError.Should().BeFalse($"Document excepted to be valid, but \"{receivedMessage}\" was fired.");
        }

        [Fact]
        public void LoadInvalidDocument()
        {
            bool hasError = false;
            var doc = XmlLoader.LoadResourceAsString("InvalidDocument1");
            Schema.XmlPdfDocument document = null;
            ((Action)(() => document = PDFFlowLib.Xml.Loader.LoadXmlPdfDocument(doc, (_ /*target*/, _ /*servery*/, _ /*message*/, _ /*exception*/) => hasError = true))).Should().NotThrow();
            hasError.Should().BeTrue();
        }
    }
}
