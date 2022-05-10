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
                .AssertThat(Property.PixelWidth, Is.EqualTo(512));            
        }
    }
}
