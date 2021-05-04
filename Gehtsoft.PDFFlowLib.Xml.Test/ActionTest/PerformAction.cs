using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gehtsoft.PDFFlowLib.Xml.Actions;
using Xunit;

namespace Gehtsoft.PDFFlowLib.Xml.Test.ActionTest
{
    public class PerformAction
    {
        [Fact]
        public void StaticCall()
        {
            TestActionObject.Method1Called = false;
            Variables variables = new Variables();

            var action = CallAction.Let("v1", typeof(TestActionObject), "Method1", Array.Empty<Type>());
            action.Execute(variables);

            TestActionObject.Method1Called.Should().BeTrue();
            variables["v1"].Should().BeOfType<TestActionObject>();
        }

        [Fact]
        public void InstanceCall()
        {
            TestActionObject.Method1Called = false;
            Variables variables = new Variables();
            TestActionObject tc = new TestActionObject();
            variables["v1"] = tc;

            var action = CallAction.Let("v2", "v1", "Method2");
            action.Execute(variables);

            tc.Method2Called.Should().BeTrue();
            variables["v2"].Should().BeOfType<int>();
            variables["v2"].Should().Be(5);
        }

        [Fact]
        public void CallWithParameter()
        {
            TestActionObject.Method1Called = false;
            Variables variables = new Variables();

            var action = CallAction.Let("v2", typeof(TestActionObject), "Duplicate", 7 + 3);
            action.Execute(variables);

            variables["v2"].Should().BeOfType<int>();
            variables["v2"].Should().Be(20);
        }

        [Fact]
        public void InstanceCallWithVariable()
        {
            TestActionObject.Method1Called = false;
            Variables variables = new Variables();
            variables["v2"] = 10;

            var action = CallAction.Let("v2", typeof(TestActionObject), "Duplicate", new Variable<int>("v2"));
            action.Execute(variables);

            variables["v2"].Should().BeOfType<int>();
            variables["v2"].Should().Be(20);
        }

        [Fact]
        public void InstanceCallWithOtherType()
        {
            TestActionObject.Method1Called = false;
            Variables variables = new Variables();
            TestActionObject tc = new TestActionObject();
            variables["v1"] = tc;

            var action = CallAction.Let("v2", "v1", "Stringize", 10);
            action.Execute(variables);

            variables["v2"].Should().BeOfType<string>();
            variables["v2"].Should().Be("10");
        }

        [Fact]
        public void InstanceCallWithNull()
        {
            TestActionObject.Method1Called = false;
            Variables variables = new Variables();

            var action = CallAction.Let("v2", typeof(TestActionObject), "Stringize", new Null<object>());
            action.Execute(variables);

            variables["v2"].Should().BeOfType<string>();
            variables["v2"].Should().Be("null");
        }

        [Fact]
        public void MultipleParameter()
        {
            TestActionObject.Method1Called = false;
            Variables variables = new Variables();

            var action = CallAction.Let("v2", typeof(TestActionObject), "Formula", 2, 4, 5);
            action.Execute(variables);

            variables["v2"].Should().BeOfType<int>();
            variables["v2"].Should().Be(1);
        }
    }
}
