using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestImages
{
    /// <summary>
    /// Represents the rendered bitmap of a WPF <see cref="Visual"/> control.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("TestImage<{BitmapSource.Format}>: {Width}×{Height}")]
    public partial class WpfTestImage : TestImage
    {
        #region factory

        public static WpfTestImage Load(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) throw new System.IO.FileNotFoundException(nameof(filePath), filePath);

            const BitmapCreateOptions createOptions = BitmapCreateOptions.None;
            const BitmapCacheOption cacheOptions = BitmapCacheOption.None; // we're running without an actual WPF context, so we can't cache anything

            if (Uri.TryCreate(filePath,UriKind.RelativeOrAbsolute,out var imageUri))
            {
                var bmp = BitmapFrame.Create(imageUri, createOptions, cacheOptions);
                return new WpfTestImage(bmp);
            }

            using(var s = System.IO.File.OpenRead(filePath))
            {
                var bmp = BitmapFrame.Create(s, createOptions, cacheOptions);
                return new WpfTestImage(bmp);
            }            
        }

        public static WpfTestImage FromRender(Visual visual, System.Windows.Size? renderSize = null)
        {
            if (visual == null) throw new ArgumentNullException(nameof(visual));

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new InvalidOperationException("ApartmentState is not STA. Use:  [NUnit.Framework.Apartment(System.Threading.ApartmentState.STA)] on the test method or assembly level.");
            }

            var bmp = WpfRenderFactory.RenderToBitmap(visual, renderSize);
            return new WpfTestImage(bmp);
        }        

        public static WpfTestImage FromBitmapSource(BitmapSource bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            return new WpfTestImage(bitmap);
        }

        private WpfTestImage(BitmapSource bmp)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            _Bitmap = WpfRenderFactory.ConvertBitmap(bmp, PixelFormats.Bgra32);
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly BitmapSource _Bitmap;        

        #endregion

        #region properties
        public BitmapSource BitmapSource => _Bitmap;
        public override int Width => _Bitmap.PixelWidth;
        public override int Height => _Bitmap.PixelHeight;

        #endregion

        #region API        

        protected override Bitmaps.Bgra32.Bitmap CreateBitmapRgba32()
        {
            if (_Bitmap == null) return null;
            var pixels = WpfRenderFactory.GetBytesRGBA32(_Bitmap);
            return new Bitmaps.Bgra32.Bitmap(pixels, _Bitmap.PixelWidth, _Bitmap.PixelHeight, 0);
        }

        protected override void WriteTo(System.IO.FileInfo finfo)
        {
            WpfRenderFactory.SaveTo(_Bitmap, finfo);            
        }        

        #endregion
    }    
}
