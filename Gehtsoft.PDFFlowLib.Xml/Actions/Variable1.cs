using System;

namespace Gehtsoft.PDFFlowLib.Xml.Actions
{
    /// <summary>
    /// The class to represent the value of a variable in action expression arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Variable<T> : Variable
    {
        public static Variable<T> Ref(string name) => new Variable<T>(name);

        public Variable(string name) : base(name)
        {
        }

        public override Type VariableType => typeof(T);

        public T Value => default;
    }
}
