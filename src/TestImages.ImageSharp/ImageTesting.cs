using System;
using System.Collections.Generic;
using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp
{
    public static class ImageTesting
    {
        public static TestImages.TestImage ToTestImage(this Image image)
        {
            return new TestImages._ImageSharpTestImage(image);
        }

        internal static void CopyPixelsTo<TPixel>(this Image image, Span<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
        {
            var bytes = System.Runtime.InteropServices.MemoryMarshal.Cast<TPixel, byte>(pixels);

            if (image is Image<TPixel> imageTyped)
            {
                imageTyped.CopyPixelDataTo(bytes);
            }
            else
            {
                using (var imageCloned = image.CloneAs<TPixel>())
                {
                    imageCloned.CopyPixelDataTo(bytes);
                }
            }
        }
    }
}
