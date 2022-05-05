using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

[assembly: AttachmentPathFormat("{WorkDirectory}/AssemblyResults/{ID}")]

namespace NUnitForImages
{
    [AttachmentPathFormat("{WorkDirectory}/AttachmentResults/{ID}")]
    public class Attachments : IAttachmentWriter
    {
        public AttachmentInfo Attachment(string fileName, string desc = null) => new AttachmentInfo(fileName, desc);

        [Test]
        public void WriteTextAttachment()
        {
            var finfo = this.Attachment("hello.text").WriteText("hello world");
            Assert.IsTrue(finfo.Exists);

            TestContext.WriteLine(finfo);
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult-{Date}")]
        public void WriteExplicitTextAttachment()
        {
            var finfo = this.Attachment("hello.txt").WriteText("hello world");

            Assert.IsTrue(finfo.Exists);

            TestContext.WriteLine(finfo);

            Assert.IsTrue(finfo.FullName.Contains("\\Explicit"));
        }

        [Test]
        [AttachmentPathFormat("?")]
        public void WriteTextAttachment2()
        {
            var finfo = this.Attachment("hello.txt").WriteText("hello world");

            Assert.IsTrue(finfo.Exists);
        }
    }
}
