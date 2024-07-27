using System;
using System.Collections.Generic;
using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

using TestImages;

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
            switch (image)
            {
                case Image<L8> typed: return typeof(L8);
                case Image<L16> typed: return typeof(L16);
                case Image<La16> typed: return typeof(La16);
                case Image<La32> typed: return typeof(La32);
                case Image<Bgra4444> typed: return typeof(Bgra4444);
                case Image<Bgra5551> typed: return typeof(Bgra5551);
                case Image<Bgr565> typed: return typeof(Bgr565);
                case Image<Bgr24> typed: return typeof(Bgr24);
                case Image<Rgb24> typed: return typeof(Rgb24);
                case Image<Bgra32> typed: return typeof(Bgra32);
                case Image<Rgba32> typed: return typeof(Rgba32);
                case Image<Argb32> typed: return typeof(Argb32);
                case Image<Rgb48> typed: return typeof(Rgb48);
                case Image<Rgba64> typed: return typeof(Rgba64);
                case Image<Rgba1010102> typed: return typeof(Rgba1010102);
                case Image<RgbaVector> typed: return typeof(RgbaVector);
                default:
                    // todo: could implement reflection here.
                    throw new NotImplementedException();
            }
        }

        [Obsolete("Use GetPixelsCheckSum")]
        internal static int GetPixelsHashCode(this Image image)
        {
            switch (image)
            {
                case Image<L8> typed: return typed.GetPixelsHashCode();
                case Image<L16> typed: return typed.GetPixelsHashCode();
                case Image<La16> typed: return typed.GetPixelsHashCode();
                case Image<La32> typed: return typed.GetPixelsHashCode();
                case Image<Bgra4444> typed: return typed.GetPixelsHashCode();
                case Image<Bgra5551> typed: return typed.GetPixelsHashCode();
                case Image<Bgr565> typed: return typed.GetPixelsHashCode();
                case Image<Bgr24> typed: return typed.GetPixelsHashCode();
                case Image<Rgb24> typed: return typed.GetPixelsHashCode();
                case Image<Bgra32> typed: return typed.GetPixelsHashCode();
                case Image<Rgba32> typed: return typed.GetPixelsHashCode();
                case Image<Argb32> typed: return typed.GetPixelsHashCode();
                case Image<Rgb48> typed: return typed.GetPixelsHashCode();
                case Image<Rgba64> typed: return typed.GetPixelsHashCode();
                case Image<Rgba1010102> typed: return typed.GetPixelsHashCode();
                case Image<RgbaVector> typed: return typed.GetPixelsHashCode();
                default:
                    throw new NotImplementedException();
            }
        }

        [Obsolete("Use GetPixelsCheckSum")]
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

        internal static uint GetPixelsCheckSum(this Image image)
        {
            switch (image)
            {
                case Image<L8> typed: return typed.GetPixelsCheckSum();
                case Image<L16> typed: return typed.GetPixelsCheckSum();
                case Image<La16> typed: return typed.GetPixelsCheckSum();
                case Image<La32> typed: return typed.GetPixelsCheckSum();
                case Image<Bgra4444> typed: return typed.GetPixelsCheckSum();
                case Image<Bgra5551> typed: return typed.GetPixelsCheckSum();
                case Image<Bgr565> typed: return typed.GetPixelsCheckSum();
                case Image<Bgr24> typed: return typed.GetPixelsCheckSum();
                case Image<Rgb24> typed: return typed.GetPixelsCheckSum();
                case Image<Bgra32> typed: return typed.GetPixelsCheckSum();
                case Image<Rgba32> typed: return typed.GetPixelsCheckSum();
                case Image<Argb32> typed: return typed.GetPixelsCheckSum();
                case Image<Rgb48> typed: return typed.GetPixelsCheckSum();
                case Image<Rgba64> typed: return typed.GetPixelsCheckSum();
                case Image<Rgba1010102> typed: return typed.GetPixelsCheckSum();
                case Image<RgbaVector> typed: return typed.GetPixelsCheckSum();
                default:
                    throw new NotImplementedException();
            }
        }

        internal static uint GetPixelsCheckSum<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            var accum = _Crc32Accumulator.Create();
            accum.AppendChecksum(image.Width);
            accum.AppendChecksum(image.Height);

            foreach(var pix in GetPixels(image))
            {
                accum.AppendChecksum(pix);
            }

            return accum.Value;
        }

        internal static IEnumerable<TPixel> GetPixels<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            var row = new TPixel[image.Width];

            for(int y=0; y<image.Height; y++)
            {
                image.DangerousGetPixelRowMemory(y).CopyTo(row);                

                for (int x = 0; x < row.Length; x++)
                {
                    var v = row[x];
                    yield return row[x];
                }
            }
        }
    }
}
