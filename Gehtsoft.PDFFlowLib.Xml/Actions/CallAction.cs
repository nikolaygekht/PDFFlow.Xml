using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gehtsoft.PDFFlowLib.Xml.Actions
{
    /// <summary>
    /// The action to perform.
    /// </summary>
    internal sealed class CallAction
    {
        /// <summary>
        /// The target type of static calls.
        /// </summary>
        public Type TargetType { get; private set; }
        /// <summary>
        /// The name of the variable to call the method for instance calls.
        /// </summary>
        public string Target { get; private set; }
        /// <summary>
        /// The name of the method.
        /// </summary>
        public string Method { get; private set; }
        /// <summary>
        /// The call parameters
        /// </summary>
        public object[] Parameters { get; private set; }
        /// <summary>
        /// The name of the variable to save the result to
        /// </summary>
        public string SaveTo { get; private set; }

        private CallAction()
        {
        }

        internal static CallAction Construct(string saveTo = null, string target = null, Type targetType = null, string method = null, object[] parameters = null)
        {
            return new CallAction()
            {
                SaveTo = saveTo,
                Target = target,
                TargetType = targetType,
                Method = method,
                Parameters = parameters ?? new object[] { }
            };
        }

        /// <summary>
        /// Call an instance method
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CallAction Call(string variable, string name, params object[] parameters)
        {
            return new CallAction()
            {
                Target = variable,
                Method = name,
                Parameters = parameters,
            };
        }

        /// <summary>
        /// Create a static call
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CallAction Call(Type type, string name, params object[] parameters)
        {
            return new CallAction()
            {
                TargetType = type,
                Method = name,
                Parameters = parameters,
            };
        }

        /// <summary>
        /// Call an instance method defined by a lambda expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variable"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static CallAction Call<T>(string variable, Expression<Action<T>> action) => Let<T>(null, variable, action);

        /// <summary>
        /// Call an instance method and save value.
        /// </summary>
        /// <param name="targetVariable"></param>
        /// <param name="variable"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CallAction Let(string targetVariable, string variable, string name, params object[] parameters)
        {
            return new CallAction()
            {
                Target = variable,
                Method = name,
                Parameters = parameters,
                SaveTo = targetVariable,
            };
        }

        /// <summary>
        /// Create a static call and save the results to a variable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CallAction Let(string target, Type type, string name, params object[] parameters)
        {
            return new CallAction()
            {
                TargetType = type,
                Method = name,
                Parameters = parameters,
                SaveTo = target
            };
        }

        /// <summary>
        /// Call an instance method as lambda expression and save value.
        /// </summary>
        /// <param name="targetVariable"></param>
        /// <param name="variable"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CallAction Let<T>(string targetVariable, string variable, Expression<Action<T>> action)
        {
            ParseExpression<T>(action, out Type targetType, out string methodName, out object[] parameters);
            if (targetType == null)
                return Let(targetVariable, variable, methodName, parameters);
            else
                return Let(targetVariable, targetType, methodName, parameters);
        }

        private static void ParseExpression<T>(Expression<Action<T>> action, out Type targetType, out string method, out object[] parameters)
        {
            targetType = null;
            method = null;
            parameters = null;

            if (action.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("The expression must be a lambda expression", nameof(action));

            var body = action.Body;

            if (body.NodeType != ExpressionType.Call)
                throw new ArgumentException("The expression must be a method call", nameof(action));

            var call = body as MethodCallExpression;

            if (!call.Method.IsStatic && call.Object.NodeType == ExpressionType.Parameter)
                targetType = null;
            else if (call.Method.IsStatic && call.Method.DeclaringType.IsAssignableFrom(typeof(T)))
                targetType = call.Method.DeclaringType;
            else
                throw new ArgumentException($"The expression must be either a call of action argument method or a call of static method of type T({typeof(T).FullName})", nameof(action));

            method = call.Method.Name;
            parameters = ScanParameters(call.Arguments);
        }

        private static object[] ScanParameters(ReadOnlyCollection<Expression> parameters)
        {
            object[] args = new object[parameters.Count];

            for (int i = 0; i < parameters.Count; i++)
            {
                var expression = parameters[i];
                if (expression.NodeType is ExpressionType.MemberAccess && typeof(Variable).IsAssignableFrom(((MemberExpression)expression).Expression.Type))
                {
                    var lambdaExpression = Expression.Lambda(((MemberExpression)expression).Expression, new ParameterExpression[] { });
                    var calculator = lambdaExpression.Compile();
                    var v = calculator.DynamicInvoke() as Variable;
                    args[i] = v;
                }
                else
                {
                    var lambdaExpression = Expression.Lambda(expression, new ParameterExpression[] { });
                    var calculator = lambdaExpression.Compile();
                    var v = calculator.DynamicInvoke();
                    if (v == null)
                    {
                        var nullType = typeof(Null<>).MakeGenericType(new Type[] { expression.Type });
                        args[i] = Activator.CreateInstance(nullType);
                    }
                    else
                        args[i] = v;
                }
            }

            return args;
        }

        public void Execute(Variables variables)
        {
            object target = null;
            Type targetType;
            if (!string.IsNullOrEmpty(Target))
            {
                target = variables[Target];
                targetType = target.GetType();
            }
            else
                targetType = TargetType;

            var parameterTypes = new Type[Parameters.Length];
            var parameterValues = new object[Parameters.Length];
            for (int i = 0; i < Parameters.Length; i++)
            {
                switch (Parameters[i])
                {
                    case Null nullValue:
                        parameterTypes[i] = nullValue.NullType;
                        parameterValues[i] = null;
                        break;
                    case Variable variable:
                        parameterTypes[i] = variable.VariableType;
                        parameterValues[i] = variables[variable.VariableName];
                        break;
                    default:
                        if (Parameters[i] == null)
                            throw new InvalidOperationException("Method argument is null. Null<T> should be used instead.");
                        parameterTypes[i] = Parameters[i].GetType();
                        parameterValues[i] = Parameters[i];
                        break;
                }
            }
            var invokator = targetType.GetMethod(Method, parameterTypes);

            if (invokator == null)
                throw new InvalidOperationException($"The method {Method} with parameter type ({TypesToString(parameterTypes)}) is not found in {(target?.GetType() ?? TargetType).FullName} stored in {Target}");

            if (!string.IsNullOrEmpty(SaveTo) && invokator.ReturnType != typeof(void))
            {
                object r = invokator.Invoke(target, parameterValues);
                variables[SaveTo] = r;
            }
            else
                invokator.Invoke(target, parameterValues);
        }

        private static string TypesToString(Type[] types)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < types.Length; i++)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(types[i].FullName);
            }
            return sb.ToString();
        }
    }
}
