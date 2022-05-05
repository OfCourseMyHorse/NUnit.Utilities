using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// Implemented by test classes that will write file attachments.
    /// </summary>
    public interface IAttachmentWriter
    {
        /// <summary>
        /// Creates an attachment object.
        /// </summary>
        /// <param name="fileName">a relative file name.</param>
        /// <param name="description">an optional attachment description</param>
        /// <returns>An attachment object</returns>
        /// <remarks>
        /// This method is usually implemented like this:<br/><br/>
        /// <c>public AttachmentInfo Attachment(string name, string desc = null) => new AttachmentInfo(name, desc);</c>
        /// </remarks>
        AttachmentInfo Attachment(string fileName, string description = null);
    }
}
