using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NUnit.Framework
{
    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Always)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ResourcePathFormatAttribute : PathFormatAttribute
    {
        #region constructors

        /// <summary>
        /// Declares a directory formatter to read resource files.
        /// </summary>
        /// <param name="format">Recommended patterns is: "*/Resources"  </param>
        public ResourcePathFormatAttribute(string format) : base(format) { }

        #endregion        
    }
}
