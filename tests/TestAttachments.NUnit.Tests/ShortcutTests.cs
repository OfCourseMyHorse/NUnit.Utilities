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

            AttachmentInfo.From("hello.url").WriteLink(txtdoc.FullName);
            AttachmentInfo.From("hello.lnk").WriteLink(txtdoc.FullName);

            AttachmentInfo.From("open dir.url").WriteLink(txtdoc.Directory.FullName);
            AttachmentInfo.From("open dir.lnk").WriteLink(txtdoc.Directory.FullName);
        }        
    }
}
