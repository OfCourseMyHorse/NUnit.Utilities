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

            WpfTestImage.Load("shannon.png")
                .AssertThat(Property.PixelWidth, Is.EqualTo(512))
                .AssertThat(Property.PixelHeight, Is.EqualTo(512))
                .AssertThat(Property.PixelArea, Is.GreaterThan(0))
                .AssertThat(Comparison.With("shannon.png").ByExactPixels, Is.True)
                .SaveTo( new AttachmentInfo("result.jpg") );               
        }

        [Test]
        public void TestCrop()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            var image = TestImage.FromFile("shannon.png");

            var crop = image.Crop(10, 10, 32, 32)
                .SaveTo(new AttachmentInfo("result.jpg"))
                .AssertThat(Property.PixelArea, Is.EqualTo(32*32))
                .AssertThat(Property.PixelsHashCode, Is.EqualTo(300997758));

            image.AssertThat(Comparison.With(crop).ByOccurrences, Is.EqualTo(1));
        }

        [Test]
        public void TestProperties()
        {
            TestContext.CurrentContext.AttachFolderBrowserShortcut();

            TestImage.FromFile("shannon.png")
                .AssertThat(Property.AverageBrightness, Is.GreaterThan(0.35))
                .AssertThat(Property.NotOpaquePixelsCount, Is.EqualTo(0))
                .AssertThat(Property.Count(System.Drawing.Color.FromArgb(100,120,32)) , Is.EqualTo(0));

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
