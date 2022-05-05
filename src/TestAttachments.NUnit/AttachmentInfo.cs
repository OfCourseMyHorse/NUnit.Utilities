﻿using System;
using System.Collections.Generic;
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
        public AttachmentInfo(string fileName, string description = null)
            : this(TestContext.CurrentContext, fileName, description) { }

        public AttachmentInfo(TestContext context, string fileName, string description = null)
        {            
            File = context.GetAttachmentFileInfo(fileName);
            Description = description;
        }        
        public System.IO.FileInfo File { get; }
        public string Description { get; set; }

        public System.IO.FileInfo WriteFile(Action<System.IO.FileInfo> writeAction)
        {
            File.Directory.Create();

            writeAction(File);

            if (File.Exists) TestContext.AddTestAttachment(File.FullName, Description);

            return File;
        }

        public System.IO.FileInfo WriteText(string textContent)
        {
            void writeText(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllText(finfo.FullName, textContent);
            }

            return WriteFile(writeText);
        }

        public System.IO.FileInfo WriteBytes(Byte[] byteContent)
        {
            void writeBytes(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllBytes(finfo.FullName, byteContent);
            }

            return WriteFile(writeBytes);
        }

        public System.IO.Stream CreateStream()
        {
            // we need to create a file so we can attach it.
            // another strategy would be to create a stream derived class that would do the attachment on stream close.

            void writeDummy(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllBytes(finfo.FullName, Array.Empty<Byte>());
            }

            return WriteFile(writeDummy).Create();
        }
    }
}