using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

[assembly: ResourcePathFormat("{TestDirectory}/Resources")]
[assembly: AttachmentPathFormat("{WorkDirectory}/AssemblyResults/{ID}")]


namespace TestAttachments
{
    [AttachmentPathFormat("{WorkDirectory}/AttachmentResults/{ID}")]
    public class RoundtripTests
    {
        [Test]
        public void WriteTextAttachment()
        {
            var text = ResourceInfo
                .From("text1.txt")
                .ReadAllText();

            Assert.AreEqual("hello world", text);

            var finfo = AttachmentInfo
                .From("text1.txt")
                .WriteAllText(text);

            Assert.IsTrue(finfo.Exists);

            TestContext.WriteLine(finfo);
        }

        [Test]
        [ResourcePathFormat("{TestDirectory}/Resources/Extended")]
        [AttachmentPathFormat("?")]
        public void WriteHelloWorldAttachment()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            var text = ResourceInfo
                .From("text2.txt")
                .ReadAllText();

            Assert.AreEqual("extended hello world", text);

            var finfo = AttachmentInfo
                .From("text2.txt")
                .WriteAllText(text);

            Assert.IsTrue(finfo.Exists);
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult-{Date}")]
        public void WriteExplicitTextAttachment()
        {
            var finfo = AttachmentInfo
                .From("hello.txt")
                .WriteAllText("hello world");

            Assert.IsTrue(finfo.Exists);

            TestContext.WriteLine(finfo);

            Assert.IsTrue(finfo.FullName.Contains("\\Explicit"));
        }        

        [Test]        
        public void WriteMarkDownAttachment()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            var finfo = AttachmentInfo
                .From("hello.md")
                .WriteTextLines("### Hello World", "This is a markdown example.");

            Assert.IsTrue(finfo.Exists);
        }
    }
}
