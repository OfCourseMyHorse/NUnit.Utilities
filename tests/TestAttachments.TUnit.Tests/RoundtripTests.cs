using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUnit;


namespace TestAttachments
{
    [AttachmentPathFormat("{WorkDirectory}/AttachmentResults/{ID}")]
    public class RoundtripTests
    {
        [Test]
        public async Task WriteTextAttachment()
        {
            var text = ResourceInfo
                .From("text1.txt")
                .ReadAllText();

            await Assert.That(text).IsEqualTo("hello world");

            var finfo = AttachmentInfo
                .From("text1.txt")
                .WriteAllText(text);

            await Assert.That(finfo.Exists).IsTrue();

            TestContext.Current!.Output.WriteLine(finfo.FullName);

            AttachmentInfo.AttachTOC();
        }

        [Test]
        [ResourcePathFormat("{TestDirectory}/Resources/Extended")]
        [AttachmentPathFormat("?")]
        public async Task WriteHelloWorldAttachment()
        {
            var text = ResourceInfo
                .From("text2.txt")
                .ReadAllText();

            await Assert.That(text).IsEqualTo("extended hello world");

            var finfo = AttachmentInfo
                .From("text2.txt")
                .WriteAllText(text);

            await Assert.That(finfo.Exists).IsTrue();
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult-{Date}")]
        public async Task WriteExplicitTextAttachment()
        {
            var finfo = AttachmentInfo
                .From("hello.txt")
                .WriteAllText("hello world");

            await Assert.That(finfo.Exists).IsTrue();

            TestContext.Current!.Output.WriteLine(finfo.FullName);

            await Assert.That(finfo.FullName).Contains("ExplicitMethodResult");
        }        

        [Test]        
        public async Task WriteMarkDownAttachment()
        {
            var finfo = AttachmentInfo
                .From("hello.md")
                .WriteTextLines("### Hello World", "This is a markdown example.", "[dir](D:/Github/(_Owned_)/(_LIBS_)/_GIT/NUnit.Utilities/tests/TestAttachments.NUnit.Tests/bin/Debug/net6.0/AttachmentResults/0-1007)");

            await Assert.That(finfo.Exists).IsTrue();
        }

        /*
        [Test]
        public async Task WriteDirectoryContext()
        {
            using(var dirctx = new AttachmentDirectory())
            {
                System.IO.File.WriteAllText(dirctx.GetFileInfo("hello.txt").FullName, "hello world");
                System.IO.File.WriteAllText(dirctx.GetFileInfo("hello2.txt").FullName, "hello world 2");
                System.IO.File.WriteAllText(dirctx.GetFileInfo("hello2.html").FullName, "<html> <body>hello world</body> </hrml>");
            }

            AttachmentInfo.AttachTOC();
        }*/
    }
}
