using System;

namespace Gehtsoft.PDFFlowLib.Xml.Actions
{
    /// <summary>
    /// Base class to represent value. Use Null(of T) instead.
    /// </summary>
    internal abstract class Null
    {
        public abstract Type NullType { get; }

        public override string ToString()
        {
            return "Null";
        }
    }
}
