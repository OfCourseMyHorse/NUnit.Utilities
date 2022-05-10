using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

[assembly: AttachmentPathFormat("{WorkDirectory}/AssemblyResults/{ID}")]

namespace TestAttachments
{
    [AttachmentPathFormat("{WorkDirectory}/AttachmentResults/{ID}")]
    public class Attachments : IAttachmentWriter
    {
        public AttachmentInfo Attach(string fileName, string desc = null) => new AttachmentInfo(fileName, desc);

        [Test]
        public void WriteTextAttachment()
        {
            var finfo = this
                .Attach("hello.text")
                .WriteText("hello world");

            Assert.IsTrue(finfo.Exists);

            TestContext.WriteLine(finfo);
        }

        [Test]
        [AttachmentPathFormat("?")]
        public void WriteHelloWorldAttachment()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            var finfo = this
                .Attach("hello.txt")
                .WriteText("hello world");

            Assert.IsTrue(finfo.Exists);
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult-{Date}")]
        public void WriteExplicitTextAttachment()
        {
            var finfo = this
                .Attach("hello.txt")
                .WriteText("hello world");

            Assert.IsTrue(finfo.Exists);

            TestContext.WriteLine(finfo);

            Assert.IsTrue(finfo.FullName.Contains("\\Explicit"));
        }        

        [Test]        
        public void WriteMarkDownAttachment()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            var finfo = this
                .Attach("hello.md")
                .WriteTextLines("### Hello World", "This is a markdown example.");

            Assert.IsTrue(finfo.Exists);
        }
    }
}
