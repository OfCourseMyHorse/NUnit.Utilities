using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestImages
{
    [System.Diagnostics.DebuggerDisplay("Cropped:{_Bounds.Left},{_Bounds.Top} TestImage: {Width}×{Height}")]
    internal class CroppedImage : TestImage
    {
        #region lifecycle
        public CroppedImage(TestImage source, System.Drawing.Rectangle rect)
        {
            _Source = source;
            _Bounds = rect;
        }

        #endregion

        #region data

        private readonly TestImage _Source;
        private readonly System.Drawing.Rectangle _Bounds;

        #endregion

        #region API

        public override int Width => _Bounds.Width;

        public override int Height => _Bounds.Height;

        protected override Bitmaps.Bgra32.Bitmap CreateBitmapRgba32()
        {
            return _Source
                ._GetBitmapRgba32()
                .Crop(_Bounds.Left, _Bounds.Top, _Bounds.Width, _Bounds.Height);
        }

        #endregion
    }

    [System.Diagnostics.DebuggerDisplay("{_File.FullName}")]
    internal class FileImage : TestImage
    {
        #region lifecycle
        public FileImage(string imagePath) : this(new FileInfo(imagePath)) { }

        public FileImage(FileInfo finfo) { _File = finfo; }

        #endregion

        #region data

        private readonly System.IO.FileInfo _File;

        #endregion

        #region API

        public override int Width => this._GetBitmapRgba32().Width;

        public override int Height => this._GetBitmapRgba32().Height;

        protected override Bitmaps.Bgra32.Bitmap CreateBitmapRgba32()
        {
            return Bitmaps.Bgra32.Bitmap.LoadFrom(_File);
        }

        #endregion
    }
}
