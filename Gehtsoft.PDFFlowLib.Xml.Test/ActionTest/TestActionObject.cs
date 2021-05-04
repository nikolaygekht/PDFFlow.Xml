namespace Gehtsoft.PDFFlowLib.Xml.Test.ActionTest
{
    public class TestActionObject
    {
        public static bool Method1Called { get; set; } = false;
        public bool Method2Called { get; set; } = false;

        public static TestActionObject Method1()
        {
            Method1Called = true;
            return new TestActionObject();
        }

        public int Method2()
        {
            Method2Called = true;
            return 5;
        }

        public static int Duplicate(int value) => value * 2;

        public static object Stringize(object v) => v?.ToString() ?? "null";

        public static int Formula(int a, int b, int c) => a + b - c;
    }
}
