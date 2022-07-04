﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{
    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Always)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AttachmentPathFormatAttribute : PathFormatAttribute
    {
        #region constructors
        
        /// <summary>
        /// Declares a directory formatter to write test attachments.
        /// </summary>
        /// <param name="format">Recommended patterns are: "?" , "*/?" or "*/yourClassName/?" </param>
        public AttachmentPathFormatAttribute(string format) : base(format) { }

        /// <summary>
        /// Declares a directory formatter to write test attachments.
        /// </summary>
        /// <param name="format">Recommended patterns are: "?" , "*/?" or "*/yourClassName/?" </param>
        /// <param name="attachShowDirectoryLink">true to automatically add a "show directory" attachment for every test.</param>
        public AttachmentPathFormatAttribute(string format, bool attachShowDirectoryLink) : base(format)
        {
            if (attachShowDirectoryLink)
            {
                this.Properties.Add("AttachShowDirectoryLink", true);
            }
        }

        #endregion
    }
}
