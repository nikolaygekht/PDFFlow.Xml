using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Gehtsoft.PDFFlowLib.Xml.Debug
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
            {
                Help();
                return;
            }
            foreach (string arg in args)
            {
                if (arg == "--all" || arg == "-a")
                {
                    foreach (var test in AllTests())
                        DoTest(test);
                }
                else
                    Test(arg);
            }
        }

        private static void Help()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  -h or --help   - show help");
            Console.WriteLine("  -a or --all    - run all test");
            Console.WriteLine("  testName       - run the specified test");
            Console.WriteLine("The tests are:");
            foreach (var test in AllTests())
                Console.WriteLine("  {0}", test.Name);
        }

        private static bool HasTestInterface(Type type) => Array.Find(type.GetInterfaces(), @interface => @interface == typeof(ITestScenario)) != null;
        private static MethodInfo GetDoMethod(Type type) => type.GetMethod("Do", BindingFlags.Public | BindingFlags.Static, null, Array.Empty<Type>(), null);
        private static bool HasDoMethod(Type type) => GetDoMethod(type) != null;
        private static bool IsTest(Type type) => HasTestInterface(type) || HasDoMethod(type);

        private static void Test(string testName)
        {
            var assembly = typeof(Program).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                if (string.Equals(type.Name, testName, StringComparison.OrdinalIgnoreCase))
                {
                    if (IsTest(type))
                        DoTest(type);
                    else
                        Console.WriteLine("Test {0} is found, but it neither implements ITestScenario, nor has a parameterless public static method Do");
                    return;
                }
            }
            Console.Write("Test {0} is not found", testName);
        }

        private static void DoTest(Type type)
        {
            if (HasTestInterface(type))
            {
                var test = Activator.CreateInstance(type) as ITestScenario;
                test.Do();
            }
            else
            {
                var method = GetDoMethod(type);
                if (method != null)
                    method.Invoke(null, null);
                else
                    throw new ArgumentException($"Class {type.FullName} neither implements ITestScenario nor has a parameterless public static method Do", nameof(type));
            }
        }

        private static IEnumerable<Type> AllTests()
        {
            var assembly = typeof(Program).Assembly;
            foreach (var type in assembly.GetTypes())
                if (IsTest(type))
                    yield return type;
        }
    }
}
