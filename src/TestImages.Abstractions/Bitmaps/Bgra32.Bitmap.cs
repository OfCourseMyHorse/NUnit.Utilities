using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestImages.Bitmaps
{
    partial struct Bgra32
    {
        /// <summary>
        /// Represents a bitmap in Bgra32 format.
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("{Width}×{Height}")]
        public sealed partial class Bitmap : Bitmap<Bgra32>
        {
            #region serialization

            public static unsafe Bitmap LoadFrom(System.IO.FileInfo finfo)
            {
                #if !WINDOWS
                throw new NotImplementedException();
                #else

                using (var img = System.Drawing.Image.FromFile(finfo.FullName))
                {
                    using (var bmp = new System.Drawing.Bitmap(img))
                    {
                        var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

                        System.Drawing.Imaging.BitmapData bits = null;

                        try
                        {
                            bits = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            if (bits.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) throw new InvalidOperationException();

                            var span = new Span<Byte>(bits.Scan0.ToPointer(), (bits.Height - 1) * bits.Stride + (bits.Width * 4));

                            return new Bitmap(span.ToArray(), bits.Width, bits.Height, bits.Stride);

                        }
                        finally
                        {
                            if (bits != null) bmp.UnlockBits(bits);
                        }
                    }
                }
                #endif
            }

            public unsafe void SaveTo(System.IO.FileInfo finfo)
            {
                #if !WINDOWS
                throw new NotImplementedException();
                #else

                using (var bmp = new System.Drawing.Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

                    System.Drawing.Imaging.BitmapData bits = null;

                    try
                    {
                        bits = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        if (bits.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) throw new InvalidOperationException();

                        var span = new Span<Byte>(bits.Scan0.ToPointer(), (bits.Height - 1) * bits.Stride + (bits.Width * 4));

                        for(int y = 0; y < bits.Height; y++)
                        {
                            var srcRow = System.Runtime.InteropServices.MemoryMarshal.Cast<Bgra32,Byte>(this.GetRow(y));
                            var dstRow = span.Slice(y* bits.Stride, srcRow.Length);

                            srcRow.CopyTo(dstRow);
                        }

                    }
                    finally
                    {
                        if (bits != null) bmp.UnlockBits(bits);
                    }

                    bmp.Save(finfo.FullName);
                }

                #endif
            }

            #endregion

            #region lifecycle

            public Bitmap(byte[] pixels, int width, int height, int byteStride = 0)
                : base(pixels, width, height, byteStride)
            { }

            #endregion

            #region API

            public new Bitmap Crop(int x, int y, int w, int h)
            {
                var bmp = new Bitmap(Array.Empty<Byte>(), 0, 0);
                bmp.CropFrom(this, x, y, w, h);
                return bmp;
            }           


            public static double GetStandardDeviation(Bitmap a, Bitmap b)
            {
                return Math.Sqrt(GetVariance(a, b));
            }

            public static double GetVariance(Bitmap a, Bitmap b)
            {
                if (a.Width != b.Width || a.Height != b.Height) throw new ArgumentException("dimensions mismatch", nameof(b));

                var variance = new _VarianceAccumulator();

                foreach (var distance in ZipPixels(a, b).Select(pair => Bitmaps.Bgra32.Distance(pair.Left, pair.Right)))
                {
                    variance.AddSample(distance);
                }

                return variance.Variance;
            }

            #endregion
        }
    }
}
