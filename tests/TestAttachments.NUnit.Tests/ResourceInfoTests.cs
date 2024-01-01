using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace TestAttachments
{
    internal class ResourceInfoTests
    {
        [Test]
        public void TestDefaultResourcePath()
        {
            var r = ResourceInfo.From("text1.txt");

            TestContext.WriteLine(r.File.FullName);

            Assert.That(r.ReadAllText(), Is.EqualTo("hello world"));
        }

        
        [Test]
        [ResourcePathFormat("{SolutionDirectory}/testdata")]
        public void TestCustomResourcePath()
        {
            var r = ResourceInfo.From("resource1.txt");

            TestContext.WriteLine(r.File.FullName);

            Assert.That(r.ReadAllText(), Is.EqualTo("hello world!"));
        }

        [Test]
        [ResourcePathFormat("{SolutionDirectory}/testdata")]
        public void TestWithTemplate()
        {
            var bytes = ResourceInfo.From("box.glb").ReadAllBytes();

            AttachmentInfo.From("cube.glb").WriteAllBytes(bytes);
        }
    }
}
