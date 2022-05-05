﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestImages
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

        protected override Rgba32.Bitmap CreateBitmapRgba32()
        {
            if (_Bitmap == null) return null;
            var pixels = WpfRenderFactory.GetBytesRGBA32(_Bitmap);
            return new Rgba32.Bitmap(pixels, _Bitmap.PixelWidth, _Bitmap.PixelHeight, 0);
        }

        protected override void WriteTo(System.IO.FileInfo finfo)
        {
            WpfRenderFactory.SaveTo(_Bitmap, finfo);            
        }        

        #endregion
    }    
}
