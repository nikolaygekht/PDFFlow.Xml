using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gehtsoft.PDFFlowLib.Xml.Actions;
using Xunit;
using FluentAssertions;
using Xunit.Sdk;

namespace Gehtsoft.PDFFlowLib.Xml.Test
{
    public class CallActionAssertionsTest
    {
        [Fact]
        public void SaveTo_OK()
        {
            CallAction action = CallAction.Construct(saveTo: "x");

            ((Action)(() => action.Should().SaveTo("x"))).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void SaveTo_FailWrongName()
        {
            CallAction action = CallAction.Construct(saveTo: "x");

            ((Action)(() => action.Should().SaveTo("y"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void SaveTo_FailNotSaving()
        {
            CallAction action = CallAction.Construct(saveTo: null);

            ((Action)(() => action.Should().SaveTo("y"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void NotSaveTo_OK()
        {
            CallAction action = CallAction.Construct(saveTo: null);

            ((Action)(() => action.Should().NotSave())).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void NotSaveTo_Fail()
        {
            CallAction action = CallAction.Construct(saveTo: "x");

            ((Action)(() => action.Should().NotSave())).Should().Throw<XunitException>();
        }

        [Fact]
        public void CallStatic_OK()
        {
            CallAction action = CallAction.Construct(targetType: typeof(object), method: "Method");
            ((Action)(() => action.Should().CallStatic<object>("Method"))).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void CallStatic_Fail_WrongType()
        {
            CallAction action = CallAction.Construct(targetType: typeof(int), method: "Method");
            ((Action)(() => action.Should().CallStatic<object>("Method"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void CallStatic_Fail_WrongMethod()
        {
            CallAction action = CallAction.Construct(targetType: typeof(object), method: "Method1");
            ((Action)(() => action.Should().CallStatic<object>("Method"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void CallStatic_Fail_CallInstance()
        {
            CallAction action = CallAction.Construct(target: "x", method: "Method");
            ((Action)(() => action.Should().CallStatic<object>("Method"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void CallInstance_OK()
        {
            CallAction action = CallAction.Construct(target: "x", method: "Method");
            ((Action)(() => action.Should().CallInstance("x", "Method"))).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void CallInstance_Fail_WrongTarget()
        {
            CallAction action = CallAction.Construct(target: "x", method: "Method");
            ((Action)(() => action.Should().CallInstance("y", "Method"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void CallInstance_Fail_WrongMethod()
        {
            CallAction action = CallAction.Construct(target: "x", method: "Method");
            ((Action)(() => action.Should().CallInstance("x", "Method1"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void CallStatic_Fail_CallStatic()
        {
            CallAction action = CallAction.Construct(targetType: typeof(object), method: "Method");
            ((Action)(() => action.Should().CallInstance("x", "Method"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HasNoParameters_OK()
        {
            CallAction action = CallAction.Construct();
            ((Action)(() => action.Should().HaveNoParameters())).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HasNoParameters_Fail()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1 });
            ((Action)(() => action.Should().HaveNoParameters())).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveParameters_OK()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1 });
            ((Action)(() => action.Should().HaveParametersCount(1))).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveParameters_Fail_NoParams()
        {
            CallAction action = CallAction.Construct(parameters: null);
            ((Action)(() => action.Should().HaveParametersCount(1))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveParameters_Fail_MoreParams()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1 });
            ((Action)(() => action.Should().HaveParametersCount(2))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveParameters_Fail_LessParams()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, 2 });
            ((Action)(() => action.Should().HaveParametersCount(1))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveParameter_OK_Value()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, 2 });
            ((Action)(() => action.Should().HaveParameterN(0, 1))).Should().NotThrow<XunitException>();
            ((Action)(() => action.Should().HaveParameterN(1, 2))).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveParameter_OK_Null()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, new Null<string>() });
            ((Action)(() => action.Should().HaveParameterN(1, null))).Should().NotThrow<XunitException>();
            ((Action)(() => action.Should().HaveNullParameterN(1))).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveParameter_Fail_Null()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, null });
            ((Action)(() => action.Should().HaveParameterN(0, null))).Should().Throw<XunitException>();
            ((Action)(() => action.Should().HaveNullParameterN(1))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveParameter_Fail_Value()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, new Null<string>() });
            ((Action)(() => action.Should().HaveParameterN(0, "a"))).Should().Throw<XunitException>();
            ((Action)(() => action.Should().HaveParameterN(1, 10))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveParameter_OK_Variable()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, new Variable<string>("x") });
            ((Action)(() => action.Should().HaveVariableParameterN<string>(1, "x"))).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveParameter_Fail_Variable_WrongName()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, new Variable<string>("x") });
            ((Action)(() => action.Should().HaveVariableParameterN<string>(1, "y"))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveParameter_Fail_Variable_WrongType()
        {
            CallAction action = CallAction.Construct(parameters: new object[] { 1, new Variable<int>("x") });
            ((Action)(() => action.Should().HaveVariableParameterN<string>(1, "x"))).Should().Throw<XunitException>();
        }

        private static Queue<CallAction> CreateXYZ()
        {
            Queue<CallAction> queue = new Queue<CallAction>();
            var action = CallAction.Construct(saveTo: "x");
            queue.Enqueue(action);
            action = CallAction.Construct(saveTo: "y");
            queue.Enqueue(action);
            action = CallAction.Construct(saveTo: "z");
            queue.Enqueue(action);
            return queue;
        }

        [Fact]
        public void HaveAction_OK()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should().HaveAction(action => action.Should().SaveTo("x"));
                queue.Should().HaveAction(action => action.Should().SaveTo("y"));
                queue.Should().HaveAction(action => action.Should().SaveTo("z"));
            })).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveAction_Fail()
        {
            var queue = CreateXYZ();

            ((Action)(() => queue.Should().HaveAction(action => action.Should().SaveTo("a")))).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveActionAfter_OK()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("z"))
                    .And.HaveItAfter(1);
            })).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveActionAfter_Fail()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("x"))
                    .And.HaveItAfter(1);
            })).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveActionBefore_OK()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("y"))
                    .And.HaveItBefore(2);
            })).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveActionBefore_Fail()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("y"))
                    .And.HaveItBefore(1);
            })).Should().Throw<XunitException>();
        }

        [Fact]
        public void HaveActionAt_OK()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("y"))
                    .And.HaveItAt(1);
            })).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void HaveActionAt_Fail()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("y"))
                    .And.HaveItAt(2);
            })).Should().Throw<XunitException>();
        }

        [Fact]
        public void AndThen_OK()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("x"))
                    .And.Then.HaveAction(action => action.Should().SaveTo("y"))
                    .And.Then.HaveAction(action => action.Should().SaveTo("z"));
            })).Should().NotThrow<XunitException>();
        }

        [Fact]
        public void AndThen_FailBefore()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("z"))
                    .And.Then.HaveAction(action => action.Should().SaveTo("y"))
                    .And.Then.HaveAction(action => action.Should().SaveTo("x"));
            })).Should().Throw<XunitException>();
        }

        [Fact]
        public void AndThen_FailNotTheSame()
        {
            var queue = CreateXYZ();

            ((Action)(() =>
            {
                queue.Should()
                    .HaveAction(action => action.Should().SaveTo("z"))
                    .And.Then.HaveAction(action => action.Should().SaveTo("z"));
            })).Should().Throw<XunitException>();
        }
    }
}
