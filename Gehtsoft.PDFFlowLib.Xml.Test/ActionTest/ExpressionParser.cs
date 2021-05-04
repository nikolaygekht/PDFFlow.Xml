using FluentAssertions;
using Gehtsoft.PDFFlowLib.Xml.Actions;
using Xunit;
using Xunit.Sdk;

namespace Gehtsoft.PDFFlowLib.Xml.Test.ActionTest
{
    public class ExpressionParser
    {
        [Fact]
        public void ParseStaticExpression()
        {
            var action = CallAction.Let<TestActionObject>("v1", null, _ => TestActionObject.Method1());

            action.Should().SaveTo("v1");
            action.Should().CallStatic(typeof(TestActionObject), "Method1");
            action.Should().HaveNoParameters();
        }

        [Fact]
        public void ParseInstanceExpression()
        {
            var action = CallAction.Let<TestActionObject>("v1", "v2", v => v.Method2());

            action.Should().SaveTo("v1")
                .And.CallInstance("v2", "Method2")
                .And.HaveNoParameters();
        }

        [Fact]
        public void ParseParameters()
        {
            var action = CallAction.Let<TestActionObject>("v1", null, _ => TestActionObject.Formula(10, 20, 5));

            action.Should().SaveTo("v1")
                .And.CallStatic<TestActionObject>("Formula")
                .And.HaveParametersCount(3)
                .And.HaveParameterN(0, 10)
                .And.HaveParameterN(1, 20)
                .And.HaveParameterN(2, 5);
        }

        [Fact]
        public void CalculateParameterFromConstants()
        {
            var action = CallAction.Let<TestActionObject>("v1", null, _ => TestActionObject.Duplicate(10 + 5));

            action.Should().SaveTo("v1")
                .And.CallStatic<TestActionObject>("Duplicate")
                .And.HaveParametersCount(1)
                .And.HaveParameterN(0, 15);
        }

        [Fact]
        public void CalculateParameterFromExternals()
        {
            const int a = 10;
            const int b = 5;
            var action = CallAction.Let<TestActionObject>("v1", null, _ => TestActionObject.Duplicate(a + b));

            action.Should().SaveTo("v1")
                .And.CallStatic<TestActionObject>("Duplicate")
                .And.HaveParametersCount(1)
                .And.HaveParameterN(0, 15);
        }

        [Fact]
        public void NullParameter()
        {
            var action = CallAction.Let<TestActionObject>("v1", null, _ => TestActionObject.Stringize(null));

            action.Should().SaveTo("v1")
                .And.CallStatic<TestActionObject>("Stringize")
                .And.HaveParametersCount(1)
                .And.HaveNullParameterN(0);
        }

        [Fact]
        public void VariableParameter()
        {
            var action = CallAction.Let<TestActionObject>("v1", null, _ => TestActionObject.Duplicate((new Variable<int>("a")).Value));
            action.Should().SaveTo("v1")
                .And.CallStatic<TestActionObject>("Duplicate")
                .And.HaveParametersCount(1)
                .And.HaveVariableParameterN<int>(0, "a");
        }
    }
}
