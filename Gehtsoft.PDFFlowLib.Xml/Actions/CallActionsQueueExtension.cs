using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Gehtsoft.PDFFlowLib.Xml.Actions
{
    internal static class CallActionsQueueExtension
    {
        /// <summary>
        /// Call an instance method
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void Call(this Queue<CallAction> queue, string variable, string name, params object[] parameters)
        {
            queue.Enqueue(CallAction.Call(variable, name, parameters));
        }

        /// <summary>
        /// Create a static call
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void Call(this Queue<CallAction> queue, Type type, string name, params object[] parameters)
        {
            queue.Enqueue(CallAction.Call(type, name, parameters));
        }

        /// <summary>
        /// Call an instance method defined by a lambda expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variable"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static void Call<T>(this Queue<CallAction> queue, string variable, Expression<Action<T>> action) => Let<T>(queue, null, variable, action);

        /// <summary>
        /// Call an instance method and save value.
        /// </summary>
        /// <param name="targetVariable"></param>
        /// <param name="variable"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void Let(this Queue<CallAction> queue, string targetVariable, string variable, string name, params object[] parameters)
        {
            queue.Enqueue(CallAction.Let(targetVariable, variable, name, parameters));
        }

        /// <summary>
        /// Create a static call and save the results to a variable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void Let(this Queue<CallAction> queue, string target, Type type, string name, params object[] parameters)
        {
            queue.Enqueue(CallAction.Let(target, type, name, parameters));
        }

        /// <summary>
        /// Call an instance method as lambda expression and save value.
        /// </summary>
        /// <param name="targetVariable"></param>
        /// <param name="variable"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void Let<T>(this Queue<CallAction> queue, string targetVariable, string variable, Expression<Action<T>> action)
        {
            queue.Enqueue(CallAction.Let<T>(targetVariable, variable, action));
        }
    }
}
