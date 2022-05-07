using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace TestImages
{
    [AttachmentPathFormat("?")]
    internal class TestImageOperations
    {
        [Test]
        public void TestEquality()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            WpfImage.Load("shannon.png")
                .AssertThat(Property.PixelWidth, Is.EqualTo(512))
                .AssertThat(Property.PixelHeight, Is.EqualTo(512))
                .AssertThat(Property.PixelArea, Is.GreaterThan(0))
                .AssertThat(Comparison.With("shannon.png").ByExactPixels, Is.True)
                .SaveTo( new AttachmentInfo("result.jpg") );               
        }

        [Test]
        public void TestStandardDeviation()
        {            
            TestImage.FromFile("shannon.jpg")
                .AssertThat(Property.PixelWidth, Is.EqualTo(512))
                .AssertThat(Property.PixelHeight, Is.EqualTo(512))
                .AssertThat(Property.PixelArea, Is.GreaterThan(0))
                .AssertThat(Comparison.With("shannon.png").ByStandardDeviation, Is.LessThan(0.03));            
        }
    }
}
