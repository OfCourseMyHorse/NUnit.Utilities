using System;
using System.Collections.Generic;
using System.Threading;

using Avalonia;

namespace TestImages
{
    /// <summary>
    /// Represents the rendered bitmap of a WPF <see cref="Visual"/> control.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("TestImage<{BitmapSource.Format}>: {Width}×{Height}")]
    public partial class AvaloniaTestImage : TestImage
    {
        #region factory

        public static AvaloniaTestImage Load(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) throw new System.IO.FileNotFoundException(nameof(filePath), filePath);

            var bmp = new Avalonia.Media.Imaging.Bitmap(filePath);
            return new AvaloniaTestImage(bmp);
        }

        public static AvaloniaTestImage FromRender(Visual visual, Avalonia.PixelSize? renderSize = null)
        {
            if (visual == null) throw new ArgumentNullException(nameof(visual));

            /*
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new InvalidOperationException("ApartmentState is not STA. Use:  [NUnit.Framework.Apartment(System.Threading.ApartmentState.STA)] on the test method or assembly level.");
            }*/

            var bmp = AvaloniaRenderFactory.RenderToBitmap(visual, renderSize);
            return new AvaloniaTestImage(bmp);
        }        

        public static AvaloniaTestImage FromBitmapSource(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            return new AvaloniaTestImage(bitmap);
        }

        private AvaloniaTestImage(Avalonia.Media.Imaging.Bitmap bmp)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            _Bitmap = AvaloniaRenderFactory.ConvertBitmap(bmp, Avalonia.Platform.PixelFormat.Bgra8888);
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Avalonia.Media.Imaging.Bitmap _Bitmap;        

        #endregion

        #region properties
        public Avalonia.Media.Imaging.Bitmap BitmapSource => _Bitmap;
        public override int Width => _Bitmap.PixelSize.Width;
        public override int Height => _Bitmap.PixelSize.Height;

        #endregion

        #region API        

        protected override Bitmaps.Bgra32.Bitmap CreateBitmapRgba32()
        {
            if (_Bitmap == null) return null;
            var pixels = AvaloniaRenderFactory.GetBytesRGBA32(_Bitmap);
            return new Bitmaps.Bgra32.Bitmap(pixels, _Bitmap.PixelSize.Width, _Bitmap.PixelSize.Height, 0);
        }

        protected override void WriteTo(System.IO.FileInfo finfo)
        {
            AvaloniaRenderFactory.SaveTo(_Bitmap, finfo);            
        }        

        #endregion
    }    
}
