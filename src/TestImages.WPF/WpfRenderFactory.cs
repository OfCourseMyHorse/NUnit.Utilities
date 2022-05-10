using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestImages
{
    /// <summary>
    /// Used to render an WPF control to a bitmap.
    /// </summary>
    /// <remarks>
    /// <see href="https://stackoverflow.com/questions/5189139/how-to-render-a-wpf-usercontrol-to-a-bitmap-without-creating-a-window">how to render a wpf usercontrol to a bitmap</see>
    /// </remarks>    
    static class WpfRenderFactory
    {
        /// <summary>
        /// initial control size, before control fitting.
        /// </summary>
        const int FIT_SIZE = 2048;

        /// <summary>
        /// bitmap size if fitting is not possible.
        /// </summary>
        const int MAX_SIZE = 1024;

        public static RenderTargetBitmap RenderToBitmap(Visual visual, Size? renderSize = null, double dpi = 96)
        {
            if (visual == null) throw new ArgumentNullException(nameof(visual));

            var tryShrink = true;

            // windows cannot be rendered "as is", but their content can.
            if (visual is Window wnd) 
            {
                // wnd.SizeToContent = SizeToContent.WidthAndHeight;
                // UpdateLayout(wnd, new Size(FIT_SIZE, FIT_SIZE));

                visual = wnd.Content as Visual;
                if (visual == null) return null;
                // tryShrink = false;
            }

            var changes = new _VisualStateRecord(visual);

            var s = _PrepareContent(visual, tryShrink, renderSize);

            var fmt = PixelFormats.Pbgra32; // PixelFormats.Default; also works.

            var rt = new RenderTargetBitmap((int)s.Width, (int)s.Height, dpi, dpi, fmt);

            rt.Render(visual);
            // rt.Freeze();

            changes.RestoreTo(visual);

            return rt;
        }

        private static Size _PrepareContent(Visual visual, bool tryShrink, Size? renderSize = null)
        {
            // maximum available size
            var s = new Size(FIT_SIZE, FIT_SIZE);            

            if (renderSize.HasValue)
            {
                s = renderSize.Value;
            }
            else
            {
                // change the alignments so it shrinks to its minimum size on layout update.
                if (tryShrink && visual is FrameworkElement fe)
                {
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                    fe.VerticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    // if we can't fit the visual, use a more
                    // reasonable render size
                    s = new Size(MAX_SIZE, MAX_SIZE);
                }
            }

            if (visual is UIElement uie)
            {
                UpdateLayout(uie, s);

                s = uie.RenderSize; // take the size from the control after layout.
            }

            return s;
        }

        private static void UpdateLayout(UIElement uie, Size availableSize)
        {
            uie.SnapsToDevicePixels = true;

            uie.UpdateLayout();
            uie.Measure(availableSize);
            uie.Arrange(new Rect(availableSize));
            uie.UpdateLayout();
        }

        public static BitmapSource ConvertBitmap(BitmapSource bitmap, PixelFormat fmt)
        {
            if (bitmap.Format == fmt) return bitmap;
            return new FormatConvertedBitmap(bitmap, fmt, null, 0);
        }

        public static void SaveTo(BitmapSource bitmap, System.IO.FileInfo finfo)
        {
            if (finfo == null) throw new ArgumentNullException(nameof(finfo));
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

            // convert the bitmap if needed
            var sfmt = GetSerializable(bitmap.Format);
            if (sfmt != bitmap.Format) bitmap = ConvertBitmap(bitmap, sfmt);

            var frame = BitmapFrame.Create(bitmap);

            BitmapEncoder encoder = null;

            var ext = System.IO.Path.GetExtension(finfo.Name).ToLower();
            if (ext.EndsWith(".png")) encoder = new PngBitmapEncoder();
            if (ext.EndsWith(".jpg")) encoder = new JpegBitmapEncoder();
            if (ext.EndsWith(".gif")) encoder = new GifBitmapEncoder();

            encoder.Frames.Add(frame);

            using (var s = finfo.Create()) encoder.Save(s);
        }        

        public static Byte[] GetBytesRGBA32(BitmapSource bitmap)
        {
            if (bitmap == null) return null;

            bitmap = ConvertBitmap(bitmap, PixelFormats.Bgra32);
            return GetBytes(bitmap);
        }

        public static Byte[] GetBytes(BitmapSource bitmap)
        {
            if (bitmap == null) return null;
            if (bitmap.PixelWidth == 0) return null;
            if (bitmap.PixelHeight == 0) return null;
            if (bitmap.Format.IsPremultiplied())
                throw new ArgumentException($"Premultiplied {bitmap.Format} format not supported.", nameof(bitmap));

            var pxW = bitmap.PixelWidth;
            var pxH = bitmap.PixelHeight;
            var bpp = (bitmap.Format.BitsPerPixel + 7) / 8;
            var pix = new byte[pxW * pxH * bpp];

            bitmap.CopyPixels(pix, pxW * bpp, 0);

            return pix;
        }

        public static bool IsPremultiplied(this PixelFormat fmt)
        {
            if (fmt == PixelFormats.Pbgra32) return true;
            if (fmt == PixelFormats.Prgba64) return true;
            if (fmt == PixelFormats.Prgba128Float) return true;
            return false;
        }

        public static PixelFormat GetSerializable(this PixelFormat fmt)
        {
            if (IsPremultiplied(fmt)) return PixelFormats.Bgra32;

            if (fmt == PixelFormats.Indexed1) return PixelFormats.Gray8;
            if (fmt == PixelFormats.BlackWhite) return PixelFormats.Gray8;            

            if (fmt == PixelFormats.Indexed2) return PixelFormats.Bgr24;
            if (fmt == PixelFormats.Indexed4) return PixelFormats.Bgr24;
            if (fmt == PixelFormats.Indexed8) return PixelFormats.Bgr24;
            if (fmt == PixelFormats.Bgr101010) return PixelFormats.Bgr24;
            if (fmt == PixelFormats.Bgr24) return PixelFormats.Bgr24;
            if (fmt == PixelFormats.Bgr32) return PixelFormats.Bgr24;
            if (fmt == PixelFormats.Bgr555) return PixelFormats.Bgr24;
            if (fmt == PixelFormats.Bgr565) return PixelFormats.Bgr24;

            if (fmt == PixelFormats.Bgra32) return PixelFormats.Bgra32;

            return fmt;
        }
    }

    // stores visual properties that we need to change so we can restore them.
    struct _VisualStateRecord
    {
        public _VisualStateRecord(Visual visual)
        {
            _HAlign = default;
            _VAlign = default;

            if (visual is FrameworkElement fe)
            {
                _HAlign = fe.HorizontalAlignment;
                _VAlign = fe.VerticalAlignment;
            }
        }

        HorizontalAlignment _HAlign;
        VerticalAlignment _VAlign;

        public void RestoreTo(Visual visual)
        {
            if (visual is FrameworkElement fe)
            {
                fe.HorizontalAlignment = _HAlign;
                fe.VerticalAlignment = _VAlign;
            }
        }
    }
}
