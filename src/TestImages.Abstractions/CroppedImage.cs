using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using TestImages.Bitmaps;

namespace TestImages
{
    internal class CroppedImage : TestImage
    {
        public CroppedImage(TestImage source, System.Drawing.Rectangle rect)
        {
            _Source= source;
            _Bounds = rect;
        }

        private readonly TestImage _Source;
        private readonly System.Drawing.Rectangle _Bounds;

        public override int Width => _Bounds.Width;

        public override int Height => _Bounds.Height;

        protected override Bgra32.Bitmap CreateBitmapRgba32()
        {
            return _Source
                .GetBitmapRgba32()
                .Crop(_Bounds.Left, _Bounds.Top, _Bounds.Width, _Bounds.Height);
        }        
    }
}
