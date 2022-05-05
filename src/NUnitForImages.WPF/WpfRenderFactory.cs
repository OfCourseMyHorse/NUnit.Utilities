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

        public static RenderTargetBitmap RenderToBitmap(Visual visual, double dpi = 96)
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
            
            var s = _PrepareContent(visual, tryShrink);

            var fmt = PixelFormats.Pbgra32; // PixelFormats.Default; also works.

            var rt = new RenderTargetBitmap((int)s.Width, (int)s.Height, dpi, dpi, fmt);

            rt.Render(visual);
            // rt.Freeze();

            return rt;
        }

        private static Size _PrepareContent(Visual visual, bool tryShrink)
        {
            // maximum available size
            var s = new Size(FIT_SIZE, FIT_SIZE);            

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
    }
}
