using System;

using SixLabors.ImageSharp;

namespace TestImages
{
    [System.Diagnostics.DebuggerDisplay("TestImage<{PixelType.Name,nq}>: {Width}×{Height}")]
    partial class _ImageSharpTestImage : TestImage
    {
        #region factory               

        public _ImageSharpTestImage(Image bmp)
        {
            Image = bmp;
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public Image Image { get; }

        #endregion

        #region properties

        public Type PixelType => Image.GetPixelType();
        
        public override int Width => Image.Width;
        public override int Height => Image.Height;

        #endregion

        #region API        

        protected override Bitmaps.Bgra32.Bitmap CreateBitmapRgba32()
        {
            if (Image == null) return null;

            var buffer = new Byte[Image.Width * Image.Height * 4];
            var slisbf = System.Runtime.InteropServices.MemoryMarshal.Cast<Byte, SixLabors.ImageSharp.PixelFormats.Bgra32>(buffer);

            Image.CopyPixelsTo(slisbf);

            return new Bitmaps.Bgra32.Bitmap(buffer, Image.Width, Image.Height);
        }

        protected override void WriteTo(System.IO.FileInfo finfo)
        {
            Image.Save(finfo.FullName);
        }

        #endregion
    }
}
