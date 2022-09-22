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

            Assert.AreEqual("hello world", r.ReadAllText());
        }

        
        [Test]
        [ResourcePathFormat("{SolutionDirectory}\\testdata")]
        public void TestCustomResourcePath()
        {
            var r = ResourceInfo.From("resource1.txt");

            TestContext.WriteLine(r.File.FullName);

            Assert.AreEqual("hello world!", r.ReadAllText());
        }
    }
}
