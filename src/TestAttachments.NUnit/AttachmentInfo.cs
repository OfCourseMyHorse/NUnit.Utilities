using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework.Internal;

using FILEINFO = System.IO.FileInfo;

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

        public static IEnumerable<FILEINFO> AttachDirectoryFiles(string directory, string mask="*", SearchOption soption = SearchOption.AllDirectories)
        {
            var dinfo = TestContext.CurrentContext.GetAttachmentDirectoryInfo();

            if (!string.IsNullOrWhiteSpace(directory))
            {
                directory = System.IO.Path.IsPathRooted(directory)
                    ? directory
                    : System.IO.Path.Combine(dinfo.FullName, directory);

                dinfo = new System.IO.DirectoryInfo(directory);
            }

            foreach (var f in dinfo.EnumerateFiles(mask, soption))
            {                
                yield return f;
                TestContext.AddTestAttachment(f.FullName);
            }
        }

        public static AttachmentInfo From(string fileName, string description = null) { return new AttachmentInfo(fileName, description); }

        public AttachmentInfo(string fileName, string description = null)
            : this(TestContext.CurrentContext, fileName, description) { }

        public AttachmentInfo(TestContext context, string fileName, string description = null)
        {
            _WriteShowDirectorShortcut = context.FindAttachmentShowDirectoryLinkEnabled();

            File = context.GetAttachmentFileInfo(fileName);
            Description = description;
        }

        #endregion

        #region data

        private bool _WriteShowDirectorShortcut;

        public FILEINFO File { get; }
        public string Description { get; set; }

        #endregion

        #region core

        private void _BeginWriteAttachment()
        {
            if (_WriteShowDirectorShortcut)
            {
                var ainfo = From("📂 Show Directory.url");
                ainfo._WriteShowDirectorShortcut = false; // prevent reentrancy

                if (!ainfo.File.Exists)
                {
                    ainfo.WriteShortcut(File.Directory.FullName);
                }
            }

            File.Directory.Create();
        }

        private FILEINFO _EndWriteAttachment()
        {
            if (!File.Exists) return File;

            // TODO:
            // in here we have the opportunity to modify the file, or
            // create a template wrapper, and attach the template instead
            // of the original file.

            TestContext.AddTestAttachment(File.FullName, Description);

            return File;
        }

        #endregion

        #region API

        /// <summary>
        /// This is a special callback used to consume <see cref="AttachmentInfo"/>
        /// without importing this library dependency.
        /// </summary>
        /// <param name="ainfo">A <see cref="AttachmentInfo"/> instance.</param>
        public static implicit operator Action<Action<FILEINFO>>(AttachmentInfo ainfo)
        {
            return action => ainfo.WriteObjectEx(action);
        }

        public System.IO.Stream CreateStream()
        {
            // we need to create a file so we can attach it.
            // another strategy would be to create a stream derived class that would do the attachment on stream close.            

            return WriteAllBytes(Array.Empty<Byte>()).Create();
        }

        public FILEINFO WriteObject(Action<string> writeAction)
        {
            return WriteObjectEx(f => writeAction(f.FullName));
        }
        
        public FILEINFO WriteObjectEx(Action<FILEINFO> writeAction)
        {
            if (writeAction == null) throw new ArgumentNullException(nameof(writeAction));

            _BeginWriteAttachment();
            writeAction(File);
            return _EndWriteAttachment();
        }

        public FILEINFO WriteToStream(Action<System.IO.Stream> writeAction)
        {
            _BeginWriteAttachment();

            using (var stream = File.Create())
            {
                writeAction(stream);
            }

            return _EndWriteAttachment();
        }        

        public FILEINFO WriteAllBytes<T>(List<T> collection)
            where T:unmanaged
        {
            #if NETSTANDARD
            var span = collection.ToArray();
            #else
            var span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(collection);
            #endif

            var bytes = System.Runtime.InteropServices.MemoryMarshal.Cast<T,Byte>(span);

            return WriteAllBytes(bytes);
        }

        public FILEINFO WriteAllBytes(ReadOnlySpan<Byte> byteContent)
        {
            _BeginWriteAttachment();

            using (var stream = File.Create())
            {
                #if NETSTANDARD2_0
                foreach (var b in byteContent) stream.WriteByte(b);
                #else
                stream.Write(byteContent);
                #endif
            }

            return _EndWriteAttachment();
        }        

        public FILEINFO WriteTextLines(params String[] textLines)
        {
            var text = string.Join("\r\n", textLines.Select(line => line == null ? string.Empty : line) );
            return WriteAllText(text);
        }

        public FILEINFO WriteAllText(String textContent)
        {
            if (textContent == null) textContent = string.Empty;

            void writeText(FILEINFO finfo)
            {
                System.IO.File.WriteAllText(finfo.FullName, textContent);
            }

            return WriteObjectEx(writeText);
        }        

        public FILEINFO WriteJson<T>(T value, System.Text.Json.JsonSerializerOptions options = null)
        {
            _BeginWriteAttachment();

            using (var stream = File.Create())
            {
                System.Text.Json.JsonSerializer.Serialize(stream, value, options);
            }            

            return _EndWriteAttachment();
        }

        public FILEINFO WriteShortcut(string fileOrDirectoryPath)
        {
            return this.WriteObjectEx(f => ShortcutUtils.CreateLink(f.FullName, fileOrDirectoryPath));
        }

        #endregion

        #region API - TOC

        /// <summary>
        /// Attaches a table of contents file with all the attached files to date.
        /// </summary>
        public static void AttachTOC()
        {
            _AttachHtmlTOC();
        }

        private static void _AttachMarkdownTOC()
        {
            var toc = From("📂 Table of Contents.md");
            var buri = new Uri(toc.File.Directory.FullName + "\\", UriKind.Absolute);

            var attachments = TestExecutionContext.CurrentContext.CurrentResult.TestAttachments;

            var md = new StringBuilder();
            foreach (var attachment in attachments)
            {
                var key = System.IO.Path.GetFileName(attachment.FilePath);
                var val = new Uri(attachment.FilePath, UriKind.Absolute);

                val = buri.MakeRelativeUri(val);

                md.AppendLine($"- [{key}]({val})");
            }

            toc.WriteAllText(md.ToString());
        }

        private static void _AttachHtmlTOC()
        {
            var toc = From("📂 Table of Contents.html");
            var buri = new Uri(toc.File.Directory.FullName + "\\", UriKind.Absolute);

            var attachments = TestExecutionContext.CurrentContext.CurrentResult.TestAttachments;

            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<body>");
            html.AppendLine("<ul>");

            foreach (var attachment in attachments)
            {
                var key = System.IO.Path.GetFileName(attachment.FilePath);
                var val = new Uri(attachment.FilePath, UriKind.Absolute);

                val = buri.MakeRelativeUri(val);

                html.AppendLine($"<li><a href=\"{val}\">{key}</a></li>");
            }

            html.AppendLine("</ul>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            toc.WriteAllText(html.ToString());
        }

        #endregion
    }
}
