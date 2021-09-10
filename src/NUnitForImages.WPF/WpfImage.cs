using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NUnitForImages
{
    /// <summary>
    /// Represents the rendered bitmap of a WPF <see cref="Visual"/> control.
    /// </summary>    
    public partial class WpfImage : TestImage
    {
        #region factory

        public static WpfImage Load(string filePath)
        {
            using(var s = System.IO.File.OpenRead(filePath))
            {
                var bmp = BitmapFrame.Create(s, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return new WpfImage(bmp);
            }            
        }

        public static WpfImage Render(Visual visual)
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new InvalidOperationException("ApartmentState is not STA. Use:  [NUnit.Framework.Apartment(System.Threading.ApartmentState.STA)] on the test method or assembly level.");
            }

            var bmp = WpfRenderFactory.RenderToBitmap(visual);
            return new WpfImage(bmp);
        }        

        private WpfImage(BitmapSource bmp)
        {
            bmp = WpfRenderFactory.ConvertBitmap(bmp, PixelFormats.Bgra32);

            _Bitmap = bmp;            
            _Pixels = new Lazy<Byte[]>(() => WpfRenderFactory.GetBytesRGBA32(_Bitmap));
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly BitmapSource _Bitmap;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Lazy<Byte[]> _Pixels;

        #endregion

        #region properties
        public BitmapSource BitmapSource => _Bitmap;
        public override int Width => _Bitmap.PixelWidth;
        public override int Height => _Bitmap.PixelHeight;        

        #endregion

        #region API        

        protected override ReadOnlySpan<Byte> GetPixelsBytes()
        {
            return _Pixels.Value;
        }

        protected override void SaveToFile(string filePath)
        {
            WpfRenderFactory.SaveTo(_Bitmap, filePath);            
        }        

        #endregion
    }    
}
