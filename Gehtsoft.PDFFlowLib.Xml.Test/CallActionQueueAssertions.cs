using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Gehtsoft.PDFFlowLib.Xml.Actions;
using Xunit.Sdk;

namespace Gehtsoft.PDFFlowLib.Xml.Test
{
    /// <summary>
    /// Assertions to apply to a queue of actions
    /// </summary>
    internal class CallActionQueueAssertions : CollectionAssertions<IReadOnlyCollection<CallAction>, CallActionQueueAssertions>
    {
        protected override string Identifier => "actions";
        private readonly int Skip = 0;
        private int LastFoundAt = -1;

        public CallActionQueueAssertions(IReadOnlyCollection<CallAction> instance, int skip = 0) : base(instance)
        {
            Skip = skip;
        }

        /// <summary>
        /// Checks whether the last HaveAction found the action before the position specified.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionQueueAssertions> HaveItBefore(int index, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt >= 0)
                .FailWith("Expected that {context:actions} is already checked for action and check was successful");

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt < index)
                .FailWith("Expected that {context:actions} contains the action specified before the element {0} but found at {1}", index, LastFoundAt);
            return new AndConstraint<CallActionQueueAssertions>(this);
        }

        /// <summary>
        /// Checks whether the last HaveAction found the action after the position specified.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionQueueAssertions> HaveItAfter(int index, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt >= 0)
                .FailWith("Expected that {context:actions} is already checked for action and check was successful");

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt > index)
                .FailWith("Expected that {context:actions} contains the action specified after the element {0} but found at {1}", index, LastFoundAt);
            return new AndConstraint<CallActionQueueAssertions>(this);
        }

        /// <summary>
        /// Checks whether the last HaveAction found the action at the position specified.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionQueueAssertions> HaveItAt(int index, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt >= 0)
                .FailWith("Expected that {context:actions} is already checked for action and check was successful");

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt == index)
                .FailWith("Expected that {context:actions} contains the action specified at the element {0} but found at {1}", index, LastFoundAt);
            return new AndConstraint<CallActionQueueAssertions>(this);
        }

        /// <summary>
        /// Checks whether the last HaveAction found the action at or before the position specified.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionQueueAssertions> HaveItAtOrBefore(int index, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt >= 0)
                .FailWith("Expected that {context:actions} is already checked for action and check was successful");

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt <= index)
                .FailWith("Expected that {context:actions} contains the action specified at the element {0} but found at {1}", index, LastFoundAt);
            return new AndConstraint<CallActionQueueAssertions>(this);
        }

        /// <summary>
        /// Checks whether the last HaveAction found the action at or after the position specified.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionQueueAssertions> HaveItAtOrAfter(int index, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt >= 0)
                .FailWith("Expected that {context:actions} is already checked for action and check was successful");

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(LastFoundAt >= index)
                .FailWith("Expected that {context:actions} contains the action specified at the element {0} but found at {1}", index, LastFoundAt);
            return new AndConstraint<CallActionQueueAssertions>(this);
        }

        /// <summary>
        /// Now check actions only after the action found during the last `HaveAction` call.
        /// </summary>
        public CallActionQueueAssertions Then
        {
            get
            {
                return new CallActionQueueAssertions(Subject, LastFoundAt + 1);
            }
        }

        /// <summary>
        /// Checks whether the queue has any action which passes assertions defined.
        /// in the specified lambda expression.
        /// </summary>
        /// <param name="checkAction"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionQueueAssertions> HaveAction(Action<CallAction> checkAction, string because = null, params object[] becauseParameters)
        {
            bool any = false;
            int index = -1;
            int skip = Skip;
            foreach (var action in Subject)
            {
                index++;
                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                try
                {
                    checkAction(action);
                    any = true;
                }
                catch (XunitException) { /* suppress assertion */ }

#pragma warning disable S2589 // Boolean expressions should not be gratuitous: False positive
                if (any)
                {
                    LastFoundAt = index;
                    break;
                }
#pragma warning restore S2589 // Boolean expressions should not be gratuitous            }
            }
            string message;
            if (Skip > 0)
                message = $"Expected {{context:actions}} contains the action specified after element {Skip - 1}, but it does not";
            else
                message = "Expected {context:actions} contains the action specified, but it does not";

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(any)
                .FailWith(message);
            return new AndConstraint<CallActionQueueAssertions>(this);
        }

        /// <summary>
        /// Checks whether the queue does not have any action which passes assertions defined.
        /// in the specified lambda expression.
        /// </summary>
        /// <param name="checkAction"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionQueueAssertions> HaveNoActions(Action<CallAction> checkAction, string because = null, params object[] becauseParameters)
        {
            bool any = false;
            int index = -1;
            int skip = Skip;
            foreach (var action in Subject)
            {
                index++;
                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                try
                {
                    checkAction(action);
                    any = true;
                }
                catch (XunitException) { /* suppress assertion */ }
            }
            string message;
            if (Skip > 0)
                message = $"Expected {{context:actions}} contains the action specified after element {Skip - 1}, but it does not";
            else
                message = "Expected {context:actions} contains the action specified, but it does not";

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .ForCondition(!any)
                .FailWith(message);
            return new AndConstraint<CallActionQueueAssertions>(this);
        }
    }
}
