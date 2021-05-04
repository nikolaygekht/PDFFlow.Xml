using System;

namespace Gehtsoft.PDFFlowLib.Xml.Actions
{
    /// <summary>
    /// The class to represent a null value in action arguments.
    /// </summary>
    /// <typeparam name="T">The value of the type</typeparam>
    internal class Null<T> : Null
        where T : class
    {
        public override Type NullType => typeof(T);

        public T Value => (T)null;
    }
}
