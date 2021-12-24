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
    public class Attachments
    {
        [Test]
        public void WriteTextAttachment()
        {
            var finfo = TestContext.CurrentContext.AttachText("hello.txt","hello world");

            TestContext.WriteLine(finfo);
        }

        [Test]
        [AttachmentPathFormat("{WorkDirectory}/ExplicitMethodResult")]
        public void WriteExplicitTextAttachment()
        {
            var finfo = TestContext.CurrentContext.AttachText("hello.txt", "hello world");

            TestContext.WriteLine(finfo);

            Assert.IsTrue(finfo.FullName.Contains("\\Explicit"));
        }
    }
}
