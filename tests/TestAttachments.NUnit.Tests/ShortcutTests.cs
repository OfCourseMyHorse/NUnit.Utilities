using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace TestAttachments
{
    [ResourcePathFormat("Resources")]
    internal class ShortcutTests
    {
        [Test]
        public void WriteShortcuts()
        {
            var txtdoc = AttachmentInfo.From("hello.txt").WriteAllText("hello world");

            AttachmentInfo.From("hello.url").WriteShortcut(txtdoc.FullName);
            AttachmentInfo.From("hello.lnk").WriteShortcut(txtdoc.FullName);

            AttachmentInfo.From("open dir.url").WriteShortcut(txtdoc.Directory.FullName);
            AttachmentInfo.From("open dir.lnk").WriteShortcut(txtdoc.Directory.FullName);
        }        
    }
}
