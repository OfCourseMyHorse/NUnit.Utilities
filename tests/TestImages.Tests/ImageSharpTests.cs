using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using SixLabors.ImageSharp;

namespace TestImages
{
    internal class ImageSharpTests
    {
        [Test]
        public void TestImageSharp()
        {
            using var image = Image.Load("shannon.jpg");

            var testImage = image
                .ToTestImage()
                .AssertThat(ImageSharpProperty.PixelsHashCode, Is.EqualTo(1213663661))
                .AssertThat(ImageSharpProperty.PixelsType, Is.EqualTo(typeof(SixLabors.ImageSharp.PixelFormats.Rgb24)))
                .AssertThat(ImageProperty.PixelWidth, Is.EqualTo(512))
                .AssertThat(ImageProperty.AverageBrightness, Is.GreaterThan(0.35))
                .AssertThat(ImageProperty.PixelsSha256Hex, Is.EqualTo("442E6A84EFB1E1B9AB3E80B948F9BE27E390C2762913F09C39BDF42A347F4C19"));

            Assert.That(testImage, Has.Property("Width").EqualTo(512));
            Assert.That(testImage, Has.Property("PixelsSha256Hex").EqualTo("442E6A84EFB1E1B9AB3E80B948F9BE27E390C2762913F09C39BDF42A347F4C19"));

            // Assert.ThatImage(testImage).HasWidth.EqualTo(512));
        }
    }
}
