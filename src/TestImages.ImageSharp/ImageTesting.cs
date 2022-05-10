using System;
using System.Collections.Generic;
using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
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

        internal static Type GetPixelType(this Image image)
        {
            if (image is Image<L8> l8) return typeof(L8);
            if (image is Image<L16> l16) return typeof(L16);            
            if (image is Image<La16> la16) return typeof(La16);
            if (image is Image<La32> la32) return typeof(La32);

            if (image is Image<Bgra4444> bgra4444) return typeof(Bgra4444);
            if (image is Image<Bgra5551> bgra5551) return typeof(Bgra5551);
            if (image is Image<Bgr565> bgr565) return typeof(Bgr565);
            if (image is Image<Bgr24> bgr24) return typeof(Bgr24);
            if (image is Image<Rgb24> rgb24) return typeof(Rgb24);

            if (image is Image<Bgra32> bgra32) return typeof(Bgra32);
            if (image is Image<Rgba32> rgba32) return typeof(Rgba32);
            if (image is Image<Argb32> argb32) return typeof(Argb32);

            if (image is Image<Rgb48> rgb48) return typeof(Rgb48);
            if (image is Image<Rgba64> rgba64) return typeof(Rgba64);
            if (image is Image<Rgba1010102> rgba1010102) return typeof(Rgba1010102);

            if (image is Image<RgbaVector> rgbaVector) return typeof(RgbaVector);

            // todo: could implement reflection here.

            throw new NotImplementedException();
        }

        internal static int GetPixelsHashCode(this Image image)
        {
            if (image is Image<L8> l8) return l8.GetPixelsHashCode();
            if (image is Image<L16> l16) return l16.GetPixelsHashCode();            
            if (image is Image<La16> la16) return la16.GetPixelsHashCode();
            if (image is Image<La32> la32) return la32.GetPixelsHashCode();

            if (image is Image<Bgra4444> bgra4444) return bgra4444.GetPixelsHashCode();
            if (image is Image<Bgra5551> bgra5551) return bgra5551.GetPixelsHashCode();
            if (image is Image<Bgr565> bgr565) return bgr565.GetPixelsHashCode();
            if (image is Image<Bgr24> bgr24) return bgr24.GetPixelsHashCode();
            if (image is Image<Rgb24> rgb24) return rgb24.GetPixelsHashCode();

            if (image is Image<Bgra32> bgra32) return bgra32.GetPixelsHashCode();
            if (image is Image<Rgba32> rgba32) return rgba32.GetPixelsHashCode();
            if (image is Image<Argb32> argb32) return argb32.GetPixelsHashCode();

            if (image is Image<Rgb48> rgb48) return rgb48.GetPixelsHashCode();
            if (image is Image<Rgba64> rgba64) return rgba64.GetPixelsHashCode();
            if (image is Image<Rgba1010102> rgba1010102) return rgba1010102.GetPixelsHashCode();

            if (image is Image<RgbaVector> rgbaVector) return rgbaVector.GetPixelsHashCode();

            throw new NotImplementedException();
        }       

        internal static int GetPixelsHashCode<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            int h = 0;

            if (typeof(TPixel) == typeof(RgbaVector) || typeof(TPixel) == typeof(Rgb48))
            {
                // we need a special case for RgbaVector and Rgb48 because internally,
                // they use ValueTuple(R,G,B).GetHashCode()  which uses a seed
                // randomly changed at every execution.                

                foreach (var pixel in image.GetPixels())
                {                    
                    h ^= pixel.ToVector4().GetHashCode();
                    h *= 37;
                }
            }
            else if (typeof(TPixel) == typeof(Rgb24) || typeof(TPixel) == typeof(Bgr24))
            {
                // we need a special case for Rgb24 and Bgr24 because internally,
                // they use ValueTuple(R,G,B).GetHashCode()  which uses a seed
                // randomly changed at every execution.

                Rgba32 tmp = default;

                foreach (var pixel in image.GetPixels())
                {
                    pixel.ToRgba32(ref tmp);
                    h ^= tmp.GetHashCode();
                    h *= 37;
                }
            }
            else // fast implementation using PackedValue.GetHashCode();
            {
                foreach (var pixel in image.GetPixels())
                {
                    h ^= pixel.GetHashCode();
                    h *= 37;
                }
            }

            return h;
        }

        internal static IEnumerable<TPixel> GetPixels<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            var row = new TPixel[image.Width];

            for(int y=0; y<image.Height; y++)
            {
                image.DangerousGetPixelRowMemory(y).CopyTo(row);

                for (int x = 0; x < row.Length; x++)
                {
                    yield return row[x];
                }
            }
        }
    }
}
