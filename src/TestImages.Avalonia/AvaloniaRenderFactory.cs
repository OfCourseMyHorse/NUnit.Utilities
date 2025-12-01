using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using AVBITMAP = Avalonia.Media.Imaging.Bitmap;
using AVPIXFMT = Avalonia.Platform.PixelFormat;
using AVPIXFMTS = Avalonia.Platform.PixelFormats;

using AVRENDERTARGET = Avalonia.Media.Imaging.RenderTargetBitmap;

// https://github.com/AvaloniaUI/Avalonia/blob/master/tests/Avalonia.RenderTests/TestRenderHelper.cs

namespace TestImages
{
    /// <summary>
    /// Used to render an Avalonia control to a bitmap.
    /// </summary>    
    static class AvaloniaRenderFactory
    {
        /// <summary>
        /// initial control size, before control fitting.
        /// </summary>
        const int FIT_SIZE = 2048;

        /// <summary>
        /// bitmap size if fitting is not possible.
        /// </summary>
        const int MAX_SIZE = 1024;

        public static AVRENDERTARGET RenderToBitmap(Visual visual, PixelSize? renderSize = null, double dpi = 96)
        {
            if (visual == null) throw new ArgumentNullException(nameof(visual));

            var tryShrink = true;

            // a window cannot be rendered "as is", but its content can.
            if (visual is Avalonia.Controls.Window wnd) 
            {
                visual = wnd.Content as Visual;
                if (visual == null) return null;
                // tryShrink = false;
            }

            var changes = new _VisualStateRecord(visual);

            var rtSize = _PrepareContent(visual, tryShrink, renderSize);
            if (rtSize.Width * rtSize.Height <= 0) throw new ArgumentException(nameof(renderSize));

            var rt = new AVRENDERTARGET(rtSize, new Avalonia.Vector(dpi, dpi));

            rt.Render(visual);
            // rt.Freeze();

            changes.RestoreTo(visual);

            return rt;
        }

        private static PixelSize _PrepareContent(Visual visual, bool tryShrink, PixelSize? renderSize = null)
        {
            // maximum available size
            var s = new PixelSize(FIT_SIZE, FIT_SIZE);            

            if (renderSize.HasValue)
            {
                s = renderSize.Value;
            }
            else
            {
                // change the alignments so it shrinks to its minimum size on layout update.
                if (tryShrink && visual is Layoutable fe)
                {
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                    fe.VerticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    // if we can't fit the visual, use a more
                    // reasonable render size
                    s = new PixelSize(MAX_SIZE, MAX_SIZE);
                }
            }

            if (visual is Layoutable uie)
            {
                UpdateLayout(uie, s);

                // take the size from the control after layout.

                s = new PixelSize((int)uie.Bounds.Width, (int)uie.Bounds.Height); 
            }
            return s;
        }

        private static void UpdateLayout(Layoutable uie, PixelSize availableSize)
        {
            var s = availableSize.ToSize(1);

            uie.UpdateLayout();
            uie.Measure(s);
            uie.Arrange(new Rect(s));
            uie.UpdateLayout();
        }      

        public static AVBITMAP ConvertBitmap(AVBITMAP sourceBitmap, AVPIXFMT fmt)
        {
            var ps = sourceBitmap.PixelSize;

            var writeableBitmap = new WriteableBitmap(ps, sourceBitmap.Dpi, fmt, AlphaFormat.Unpremul);

            using (var lockedBitmap = writeableBitmap.Lock())
            {
                sourceBitmap.CopyPixels(new PixelRect(ps), lockedBitmap.Address, lockedBitmap.RowBytes * ps.Height, lockedBitmap.RowBytes);
            }

            return writeableBitmap;
        }

        public static void SaveTo(AVBITMAP bitmap, System.IO.FileInfo finfo)
        {
            if (finfo == null) throw new ArgumentNullException(nameof(finfo));
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            if (bitmap.Format.Value == null) throw new ArgumentNullException(nameof(bitmap));

            // convert the bitmap if needed

            var sfmt = GetSerializable(bitmap.Format.Value);
            if (sfmt != bitmap.Format) bitmap = ConvertBitmap(bitmap, sfmt);

            bitmap.Save(finfo.FullName);            
        }        

        public static Byte[] GetBytesRGBA32(AVBITMAP bitmap)
        {
            if (bitmap == null) return null;

            bitmap = ConvertBitmap(bitmap, AVPIXFMTS.Bgra8888);
            return GetBytes(bitmap);
        }

        public unsafe static Byte[] GetBytes(AVBITMAP bitmap)
        {
            

            if (bitmap == null) return null;
            if (bitmap.PixelSize.Width == 0) return null;
            if (bitmap.PixelSize.Height == 0) return null;

            if (bitmap.AlphaFormat == Avalonia.Platform.AlphaFormat.Premul) throw new ArgumentException($"Premultiplied format not supported.", nameof(bitmap));

            var pxW = bitmap.PixelSize.Width;
            var pxH = bitmap.PixelSize.Height;
            var bpp = (bitmap.Format.Value.BitsPerPixel + 7) / 8;
            var pix = new byte[pxW * pxH * bpp];

            var sourceRect = new PixelRect(0, 0, bitmap.PixelSize.Width, bitmap.PixelSize.Height);

            fixed (byte* ptr = pix)
            {
                bitmap.CopyPixels(sourceRect, (IntPtr)ptr, pix.Length, pxW * bpp);
            }        

            return pix;
        }        

        public static AVPIXFMT GetSerializable(this AVPIXFMT fmt)
        {
            if (fmt == AVPIXFMTS.Gray2) return AVPIXFMTS.Gray8;
            if (fmt == AVPIXFMTS.Gray4) return AVPIXFMTS.Gray8;
            if (fmt == AVPIXFMTS.Gray8) return AVPIXFMTS.Gray8;
            if (fmt == AVPIXFMTS.Gray16) return AVPIXFMTS.Gray8;
            if (fmt == AVPIXFMTS.BlackWhite) return AVPIXFMTS.Gray8;
            
            if (fmt == AVPIXFMTS.Bgr24) return AVPIXFMTS.Bgr24;
            if (fmt == AVPIXFMTS.Bgr32) return AVPIXFMTS.Bgr24;
            if (fmt == AVPIXFMTS.Bgr555) return AVPIXFMTS.Bgr24;
            if (fmt == AVPIXFMTS.Bgr565) return AVPIXFMTS.Bgr24;

            if (fmt == AVPIXFMTS.Rgb24) return AVPIXFMTS.Rgb24;
            if (fmt == AVPIXFMTS.Rgb32) return AVPIXFMTS.Rgb24;
            if (fmt == AVPIXFMTS.Rgb565) return AVPIXFMTS.Rgb24;

            if (fmt == AVPIXFMTS.Rgba64) return AVPIXFMTS.Rgba8888;
            if (fmt == AVPIXFMTS.Rgba8888) return AVPIXFMTS.Rgba8888;

            return fmt;
        }
    }

    /// <summary>
    /// stores visual properties that we need to change so we can restore them.
    /// </summary>
    struct _VisualStateRecord
    {
        public _VisualStateRecord(Visual visual)
        {
            _HAlign = default;
            _VAlign = default;

            if (visual is Layoutable fe)
            {
                _HAlign = fe.HorizontalAlignment;
                _VAlign = fe.VerticalAlignment;
            }
        }

        HorizontalAlignment _HAlign;
        VerticalAlignment _VAlign;

        public void RestoreTo(Visual visual)
        {
            if (visual is Layoutable fe)
            {
                fe.HorizontalAlignment = _HAlign;
                fe.VerticalAlignment = _VAlign;
            }
        }
    }
}
