using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestImages
{
    internal class FileImage : TestImage
    {
        public FileImage(string imagePath) : this(new FileInfo(imagePath)) { }

        public FileImage(FileInfo finfo) { _File = finfo; }

        private readonly System.IO.FileInfo _File;

        public override int Width => this.BitmapRgba32.Width;

        public override int Height => this.BitmapRgba32.Height;

        protected override Bitmaps.Bgra32.Bitmap CreateBitmapRgba32()
        {
            return Bitmaps.Bgra32.Bitmap.LoadFrom(_File);
        }        
    }
}
