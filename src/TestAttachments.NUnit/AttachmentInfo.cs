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
    /// File is only attached after it's been written.
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
            return action => ainfo.WriteFile(action);
        }

        public System.IO.FileInfo WriteFile(Action<System.IO.FileInfo> writeAction)
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

            return WriteBytes(Array.Empty<Byte>()).Create();
        }

        public System.IO.FileInfo WriteTextLines(params String[] textLines)
        {
            var text = string.Join("\r\n", textLines.Select(line => line == null ? string.Empty : line) );
            return WriteText(text);
        }

        public System.IO.FileInfo WriteText(String textContent)
        {
            if (textContent == null) textContent = string.Empty;

            void writeText(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllText(finfo.FullName, textContent);
            }

            return WriteFile(writeText);
        }

        public System.IO.FileInfo WriteBytes(Byte[] byteContent)
        {
            if (byteContent == null) byteContent = Array.Empty<Byte>();

            void writeBytes(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllBytes(finfo.FullName, byteContent);
            }

            return WriteFile(writeBytes);
        }

        public System.IO.FileInfo WriteBytes(ArraySegment<Byte> byteContent)
        {
            if (byteContent.Array == null) byteContent = new ArraySegment<byte>(Array.Empty<Byte>());

            void writeBytes(System.IO.FileInfo finfo)
            {
                using(var stream = finfo.Create())
                {
                    stream.Write(byteContent.Array,byteContent.Offset,byteContent.Count);
                }                
            }

            return WriteFile(writeBytes);
        }

        #endregion
    }
}
