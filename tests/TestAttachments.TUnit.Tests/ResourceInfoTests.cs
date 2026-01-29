using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUnit;

namespace TestAttachments
{
    internal class ResourceInfoTests
    {
        [Test]
        public async Task TestDefaultResourcePath()
        {
            var r = ResourceInfo.From("text1.txt");

            TestContext.Current!.Output.WriteLine(r.File.FullName);

            await Assert.That(r.ReadAllText()).IsEqualTo("hello world");
        }

        
        [Test]
        [ResourcePathFormat("{SolutionDirectory}/testdata")]
        public async Task TestCustomResourcePath()
        {
            var r = ResourceInfo.From("resource1.txt");

            TestContext.Current!.Output.WriteLine(r.File.FullName);

            await Assert.That(r.ReadAllText()).IsEqualTo("hello world!");
        }

        [Test]
        [ResourcePathFormat("{SolutionDirectory}/testdata")]
        public async Task TestWithTemplate()
        {
            var bytes = ResourceInfo.From("box.glb").ReadAllBytes();

            AttachmentInfo.From("cube.glb").WriteAllBytes(bytes);
        }

        [Test]
        [ResourcePathFormat("{RepositoryRoot}/testdata")]
        public async Task TestWithRepositoryPath()
        {
            var bytes = ResourceInfo.From("box.glb").ReadAllBytes();

            AttachmentInfo.From("cube.glb").WriteAllBytes(bytes);
        }
    }
}
