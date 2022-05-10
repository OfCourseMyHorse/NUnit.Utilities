using System;
using System.Collections.Generic;
using System.Text;

namespace TestImages
{
    /// <summary>
    /// ImageSharp specific properties.
    /// </summary>
    public static class ImageSharpProperty
    {
        /// <summary>
        /// Gets the native pixel format type, as in <see cref="SixLabors.ImageSharp.PixelFormats.Rgb24"/>
        /// </summary> 
        public static Type PixelsType(TestImage image)
        {
            if (image is _ImageSharpTestImage ximage)
            {
                return SixLabors.ImageSharp.ImageTesting.GetPixelType(ximage.Image);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// calculates the hash code of the image pixels in their native pixel format.
        /// </summary>        
        public static int PixelsHashCode(TestImage image)
        {
            if (image is _ImageSharpTestImage ximage)
            {
                return SixLabors.ImageSharp.ImageTesting.GetPixelsHashCode(ximage.Image);
            }

            return Property.PixelsHashCode(image);
        }        
    }
}
