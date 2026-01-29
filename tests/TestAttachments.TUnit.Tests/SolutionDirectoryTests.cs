using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUnit;

namespace TestAttachments
{
    [ResourcePathFormat("{SolutionDirectory}/tests/ExtraResources")]
    [AttachmentPathFormat("{SolutionDirectory}/tests/TestResults/{ID}")]
    internal class SolutionDirectoryTests
    {
        [Test]
        public async Task WriteTextAttachment()
        {
            var text = ResourceInfo
                .From("text1.txt")
                .ReadAllText();

            await Assert.That(text).IsEqualTo("Extra Resources");

            var finfo = AttachmentInfo
                .From("text1.txt")
                .WriteAllText(text);

            await Assert.That(finfo.Exists).IsTrue();

            TestContext.Current!.Output.WriteLine(finfo.FullName);
        }

        [Test]
        [ResourcePathFormat("{\".gitignore\"}/tests/ExtraResources")]        
        public async Task WriteTextAttachmentFromGitIgnore()
        {
            var text = ResourceInfo
                .From("text1.txt")
                .ReadAllText();

            await Assert.That(text).IsEqualTo("Extra Resources");

            var finfo = AttachmentInfo
                .From("text1.txt")
                .WriteAllText(text);

            await Assert.That(finfo.Exists).IsTrue();

            TestContext.Current!.Output.WriteLine(finfo.FullName);
        }
    }
}
