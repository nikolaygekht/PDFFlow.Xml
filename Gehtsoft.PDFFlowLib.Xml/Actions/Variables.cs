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
    /// The list of variables
    /// </summary>
    internal class Variables
    {
        private readonly Dictionary<string, object> mVariables = new Dictionary<string, object>();

        /// <summary>
        /// Get, set or remove variable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get => mVariables[name];

            set
            {
                if (value == null)
                    mVariables.Remove(name);
                else
                    mVariables[name] = value;
            }
        }
    }
}
