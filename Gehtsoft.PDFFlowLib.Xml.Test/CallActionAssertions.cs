using System;
using System.Linq;
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
    /// Assertions for actions
    /// </summary>
    internal class CallActionAssertions : ReferenceTypeAssertions<CallAction, CallActionAssertions>
    {
        protected override string Identifier => "action";

        public CallActionAssertions(CallAction instance) : base(instance)
        {
        }

        /// <summary>
        /// Asserts whether the action calls the static method specified of the type specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> CallStatic<T>(string methodName, string because = null, params object[] becauseParameters) => CallStatic(typeof(T), methodName, because, becauseParameters);

        /// <summary>
        /// Asserts whether the action calls the static method specified of the type specified
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> CallStatic(Type type, string methodName, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.TargetType != null)
                .FailWith("Expected {context:action} call a static method but it calls an instance method.")
                .Then
                .ForCondition(action => action.TargetType == type)
                .FailWith("Expected {context:action} call a static method of {0} but it calls a method of {1}.", type.FullName, Subject.TargetType.FullName)
                .Then
                .ForCondition(action => action.Method == methodName)
                .FailWith("Expected {context:action} call the method {0} but it calls a method of {1}.", methodName, Subject.Method);

            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the action calls a dynamic method of an object stored in a variable
        /// </summary>
        /// <param name="targetVariable"></param>
        /// <param name="methodName"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> CallInstance(string targetVariable, string methodName, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.TargetType == null)
                .FailWith("Expected {context:action} call an instance method but it calls a static method.")
                .Then
                .ForCondition(action => action.Target == targetVariable)
                .FailWith("Expected {context:action} call a method of object saved in {0} but it calls a method of {1}.", targetVariable, Subject.Target)
                .Then
                .ForCondition(action => action.Method == methodName)
                .FailWith("Expected {context:action} call the method {0} but it calls a method of {1}.", methodName, Subject.Method);
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the action saves the value to the variable specified
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> SaveTo(string variableName, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.SaveTo == variableName)
                .FailWith("Expected {context:action} save the value to {0} but it save it to {1}.", variableName, Subject.SaveTo);
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the action does not save value to any variable.
        /// </summary>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> NotSave(string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.SaveTo == null)
                .FailWith("Expected {context:action} doesn't save the value but it save it to {0}.", Subject.SaveTo);
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the action does have no parameters
        /// </summary>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveNoParameters(string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.Parameters.Length == 0)
                .FailWith("Expected {context:action} doesn't have parameters but it has {0}.", Subject.Parameters.Length);

            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the action have any parameters
        /// </summary>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveParameters(string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.Parameters.Length != 0)
                .FailWith("Expected {context:action} has parameters but it doesn't.");

            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the action have the specified number of the parameters
        /// </summary>
        /// <param name="parametersCount"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveParametersCount(int parametersCount, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.Parameters.Length == parametersCount)
                .FailWith("Expected {context:action} has {0} parameters but it has {1}.", parametersCount, Subject.Parameters.Length);

            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the parameter at the specified index has the value
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveParameterN(int parameter, object value, string because = null, params object[] becauseParameters)
        {
            if (value == null)
                return HaveNullParameterN(parameter, because, becauseParameters);

            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => value.Equals(action.Parameters[parameter]))
                .FailWith("Expected {context:action} has {0}th parameter equals to {1} but it is {2}.", parameter, value, Subject.Parameters[parameter]);

            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the parameter at the specified index has the value matching the predicate specified
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="predicate"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveParameterN<T>(int parameter, Func<T, bool> predicate, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                .BecauseOf(because, becauseParameters)
                .Given(() => Subject)
                .ForCondition(action => action.Parameters[parameter] is T t && predicate(t))
                .FailWith("Expected {context:action} has {0}th parameter matches the predicate but it does not.", parameter);

            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts whether the parameter at the index specified is a variable reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="variable"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveVariableParameterN<T>(int parameter, string variable, string because = null, params object[] becauseParameters)
        {
            return HaveParameterN(parameter, new Variable<T>(variable), because, becauseParameters);
        }

        /// <summary>
        /// Asserts whether the parameter at the index specified is a null
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveNullParameterN(int parameter, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .Given(() => Subject)
                    .ForCondition(action => action.Parameters[parameter] is Null)
                    .FailWith("Expected {context:action} has {0}th parameter equals to {1} but it is {2}.", parameter, "Null", Subject.Parameters[parameter]);
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts the action have any parameter equals to the action specified.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveParameter(object value, string because = null, params object[] becauseParameters)
        {
            bool any = false;

            if (value == null)
                return HaveNullParameter(because, becauseParameters);

            for (int i = 0; i < Subject.Parameters.Length && !any; i++)
            {
                if (Subject.Parameters[i].Equals(value))
                    any = true;
            }

            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .ForCondition(any)
                    .FailWith("Expected {context:action} has a parameter equals to {0} but it doesn't have.", value);
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts the action have any parameter of the action matches the predicate
        /// </summary>
        /// <param name="value"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveParameter(Func<object, bool> predicate, string because = null, params object[] becauseParameters)
        {
            bool any = false;

            for (int i = 0; i < Subject.Parameters.Length && !any; i++)
            {
                if (predicate(Subject.Parameters[i]))
                    any = true;
            }

            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .ForCondition(any)
                    .FailWith("Expected {context:action} has a parameter matches the predicate specified but it doesn't have.");
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts the action have any parameter of the action matches the predicate
        /// </summary>
        /// <param name="value"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveParameter<T>(Func<T, bool> predicate, string because = null, params object[] becauseParameters)
        {
            bool any = false;

            for (int i = 0; i < Subject.Parameters.Length && !any; i++)
            {
                if (Subject.Parameters[i] is T t && predicate(t))
                    any = true;
            }

            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .ForCondition(any)
                    .FailWith("Expected {context:action} has a parameter matches the predicate specified but it doesn't have.");
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts the action have any null parameter
        /// </summary>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveNullParameter(string because = null, params object[] becauseParameters)
        {
            bool any = false;

            for (int i = 0; i < Subject.Parameters.Length && !any; i++)
            {
                if (Subject.Parameters[i] is Null)
                    any = true;
            }

            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .ForCondition(any)
                    .FailWith("Expected {context:action} has a parameter equals to Null but it doesn't have.");
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts the action have any parameter referring to the variable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> HaveVariableParameter<T>(string name, string because = null, params object[] becauseParameters)
        {
            bool any = false;

            for (int i = 0; i < Subject.Parameters.Length && !any; i++)
            {
                if (Subject.Parameters[i] is Variable v &&
                    v.VariableName == name &&
                    v.VariableType == typeof(T))
                    any = true;
            }

            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .ForCondition(any)
                    .FailWith("Expected {context:action} has a parameter referring to variable {0} of type {1} but it doesn't have.", name, typeof(T).Name);
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts the action calls any static method of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> CallAnyOf<T>(string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .Given(() => Subject)
                    .ForCondition(action => action.TargetType == typeof(T))
                    .FailWith("Expected {context:action} call a method of type {0} but it doesn't have.", typeof(T).Name);
            return new AndConstraint<CallActionAssertions>(this);
        }

        /// <summary>
        /// Asserts the action calls any static method of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="because"></param>
        /// <param name="becauseParameters"></param>
        /// <returns></returns>
        public AndConstraint<CallActionAssertions> CallAnyOf(string variableName, string because = null, params object[] becauseParameters)
        {
            Execute.Assertion
                    .BecauseOf(because, becauseParameters)
                    .Given(() => Subject)
                    .ForCondition(action => action.Target == variableName)
                    .FailWith("Expected {context:action} call a method of type {0} but it doesn't have.", variableName);
            return new AndConstraint<CallActionAssertions>(this);
        }
    }
}
