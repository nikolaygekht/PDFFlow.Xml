using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gehtsoft.PDFFlowLib.Xml.Debug
{
    internal class MainTest1 : ITestScenario
    {
        public void Do()
        {
            Console.WriteLine("MainTest1");
        }
    }

    internal static class MainTest2
    {
        public static void Do()
        {
            Console.WriteLine("MainTest2");
        }
    }
}
