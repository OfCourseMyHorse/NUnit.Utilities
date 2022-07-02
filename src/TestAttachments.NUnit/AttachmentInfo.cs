using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// Represents an attachement file object.
    /// </summary>
    /// <remarks>
    /// <para>File is only attached to the test after it's been written.</para>
    /// <para>Use <see cref="AttachmentPathFormatAttribute"/> at the top of the test class to define the attachment directory path</para>    
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{File.FullName}")]
    public class AttachmentInfo
    {
        #region lifecycle

        public static AttachmentInfo From(string fileName, string description = null) { return new AttachmentInfo(fileName, description); }

        public AttachmentInfo(string fileName, string description = null)
            : this(TestContext.CurrentContext, fileName, description) { }

        public AttachmentInfo(TestContext context, string fileName, string description = null)
        {            
            File = context.GetAttachmentFileInfo(fileName);
            Description = description;
        }

        #endregion

        #region data

        public System.IO.FileInfo File { get; }
        public string Description { get; set; }

        #endregion

        #region API

        /// <summary>
        /// This is a special callback used to consume <see cref="AttachmentInfo"/>
        /// without importing this library dependency.
        /// </summary>
        /// <param name="ainfo">A <see cref="AttachmentInfo"/> instance.</param>
        public static implicit operator Action<Action<System.IO.FileInfo>>(AttachmentInfo ainfo)
        {
            return action => ainfo.WriteObject(action);
        }

        [Obsolete("Use WriteObject", true)]
        public System.IO.FileInfo WriteFile(Action<System.IO.FileInfo> writeAction)
        {
            return WriteObject(writeAction);
        }

        public System.IO.FileInfo WriteObject(Action<System.IO.FileInfo> writeAction)
        {
            if (writeAction == null) throw new ArgumentNullException(nameof(writeAction));

            File.Directory.Create();

            writeAction(File);

            if (File.Exists) TestContext.AddTestAttachment(File.FullName, Description);

            return File;
        }

        public System.IO.Stream CreateStream()
        {
            // we need to create a file so we can attach it.
            // another strategy would be to create a stream derived class that would do the attachment on stream close.            

            return WriteAllBytes(Array.Empty<Byte>()).Create();
        }

        public System.IO.FileInfo WriteTextLines(params String[] textLines)
        {
            var text = string.Join("\r\n", textLines.Select(line => line == null ? string.Empty : line) );
            return WriteAllText(text);
        }

        public System.IO.FileInfo WriteAllText(String textContent)
        {
            if (textContent == null) textContent = string.Empty;

            void writeText(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllText(finfo.FullName, textContent);
            }

            return WriteObject(writeText);
        }

        public System.IO.FileInfo WriteAllBytes(Byte[] byteContent)
        {
            if (byteContent == null) byteContent = Array.Empty<Byte>();

            void writeBytes(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllBytes(finfo.FullName, byteContent);
            }

            return WriteObject(writeBytes);
        }

        public System.IO.FileInfo WriteAllBytes(ArraySegment<Byte> byteContent)
        {
            if (byteContent.Array == null) byteContent = new ArraySegment<byte>(Array.Empty<Byte>());

            void writeBytes(System.IO.FileInfo finfo)
            {
                using(var stream = finfo.Create())
                {
                    stream.Write(byteContent.Array,byteContent.Offset,byteContent.Count);
                }                
            }

            return WriteObject(writeBytes);
        }

        #endregion
    }
}
