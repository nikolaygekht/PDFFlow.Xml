using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gehtsoft.PDFFlowLib.Xml.Test
{
    static class XmlLoader
    {
        public static byte[] LoadResource(string name)
        {
            string resourceName = $"Gehtsoft.PDFFlowLib.Xml.Test.TestDocuments.{name}.xml";
            using (var mf = typeof(XmlLoader).Assembly.GetManifestResourceStream(resourceName))
            {
                if (mf == null)
                    throw new FileNotFoundException($"Resource {resourceName} is not found in the test assembly");

                int l = (int)mf.Length;
                byte[] b = new byte[l];
                mf.Read(b, 0, l);
                return b;
            }
        }

        public static string LoadResourceAsString(string name, Encoding encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(LoadResource(name));
        }
    }
}
