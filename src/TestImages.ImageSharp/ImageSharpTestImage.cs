using System;

using SixLabors.ImageSharp;

namespace TestImages
{   
    partial class _ImageSharpTestImage : TestImage
    {
        #region factory               

        public _ImageSharpTestImage(Image bmp)
        {
            _Bitmap = bmp;
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Image _Bitmap;

        #endregion

        #region properties
        
        public override int Width => _Bitmap.Width;
        public override int Height => _Bitmap.Height;

        #endregion

        #region API        

        protected override Bitmaps.Bgra32.Bitmap CreateBitmapRgba32()
        {
            if (_Bitmap == null) return null;

            var buffer = new Byte[_Bitmap.Width * _Bitmap.Height * 4];
            var slisbf = System.Runtime.InteropServices.MemoryMarshal.Cast<Byte, SixLabors.ImageSharp.PixelFormats.Bgra32>(buffer);

            _Bitmap.CopyPixelsTo(slisbf);

            return new Bitmaps.Bgra32.Bitmap(buffer, _Bitmap.Width, _Bitmap.Height);
        }

        protected override void WriteTo(System.IO.FileInfo finfo)
        {
            _Bitmap.Save(finfo.FullName);
        }

        #endregion
    }
}
