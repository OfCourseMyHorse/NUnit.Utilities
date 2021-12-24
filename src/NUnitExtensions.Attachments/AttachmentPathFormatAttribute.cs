using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{    
    public class AttachmentPathFormatAttribute : PropertyAttribute
    {

        #region constructors
        public AttachmentPathFormatAttribute(string format) : base(format) { }        

        #endregion
    }
}
