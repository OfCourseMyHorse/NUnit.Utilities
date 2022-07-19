using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace TestAttachments
{
    [ResourcePathFormat("{SolutionDirectory}/Tests/ExtraResources")]
    [AttachmentPathFormat("{SolutionDirectory}/Tests/TestResults/{ID}", true)]
    internal class SolutionDirectoryTests
    {
        [Test]
        public void WriteTextAttachment()
        {
            var text = ResourceInfo
                .From("text1.txt")
                .ReadAllText();

            Assert.AreEqual("Extra Resources", text);

            var finfo = AttachmentInfo
                .From("text1.txt")
                .WriteAllText(text);

            Assert.IsTrue(finfo.Exists);

            TestContext.WriteLine(finfo);
        }
    }
}
