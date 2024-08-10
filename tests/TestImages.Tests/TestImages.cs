using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace TestImages
{
    [TestFixture]
    [AttachmentPathFormat("?")]
    internal class TestImageOperations
    {
        [Test]
        public void TestEquality()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            WpfTestImage.Load("shannon.png")
                .AssertThat(ImageProperty.PixelWidth, Is.EqualTo(512))
                .AssertThat(ImageProperty.PixelHeight, Is.EqualTo(512))
                .AssertThat(ImageProperty.PixelArea, Is.GreaterThan(0))
                .AssertThat(ImageComparison.With("shannon.png").ByExactPixels, Is.True)
                .SaveTo( new AttachmentInfo("result.jpg") );
        }

        [Test]
        public void TestCrop()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            var image = TestImage.FromFile("shannon.png");

            var crop = image.Crop(10, 10, 32, 32)
                .SaveTo(new AttachmentInfo("result.jpg"))
                .AssertThat(ImageProperty.PixelArea, Is.EqualTo(32*32))
                .AssertThat(ImageProperty.CheckSum, Is.EqualTo(2540420981u));

            Assert.That(image.ComparedTo(crop).ByOccurrences, Is.EqualTo(1));

            image.AssertThat(ImageComparison.With(crop).ByOccurrences, Is.EqualTo(1));
        }

        [Test]
        public void TestProperties()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            TestImage.FromFile("shannon.png")
                .AssertThat(ImageProperty.AverageBrightness, Is.GreaterThan(0.35))
                .AssertThat(ImageProperty.NotOpaquePixelsCount, Is.EqualTo(0))
                .AssertThat(ImageProperty.Count(System.Drawing.Color.FromArgb(100,120,32)) , Is.EqualTo(0));

        }

        [Test]
        public void TestStandardDeviation()
        {            
            TestImage.FromFile("shannon.jpg")
                .AssertThat(ImageProperty.PixelWidth, Is.EqualTo(512))
                .AssertThat(ImageProperty.PixelHeight, Is.EqualTo(512))
                .AssertThat(ImageProperty.PixelArea, Is.GreaterThan(0))
                .AssertThat(ImageComparison.With("shannon.png").ByStandardDeviation, Is.LessThan(0.03));            
        }
    }
}
