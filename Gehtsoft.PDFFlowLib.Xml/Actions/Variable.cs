using System;

namespace Gehtsoft.PDFFlowLib.Xml.Actions
{
    /// <summary>
    /// The base class that represents the variable reference. Use Variable(of T) instead.
    /// </summary>
    public abstract class Variable
    {
        public abstract Type VariableType { get; }
        public string VariableName { get; }

        protected Variable(string name)
        {
            VariableName = name;
        }

        public override string ToString()
        {
            return $"Variable({VariableType.Name}):{VariableName}";
        }

        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 23 + VariableType.GetHashCode();
                hash = hash * 23 + VariableName.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is Variable variable)
                return this.VariableType == variable.VariableType && this.VariableName == variable.VariableName;
            return object.ReferenceEquals(this, obj);
        }
    }
}
