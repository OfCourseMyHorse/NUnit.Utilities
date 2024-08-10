using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestImages
{
    /// <summary>
    /// represents a <see cref="TestImage"/> pair comparer
    /// </summary>    
    public readonly struct TestImageComparingPair
    {
        #region lifecycle        

        internal TestImageComparingPair(TestImage left, TestImage right)
        {
            _Left = left;
            _Right = right;
        }

        #endregion

        #region data

        private readonly TestImage _Left;
        private readonly TestImage _Right;

        #endregion

        #region properties

        public bool ByExactPixels => _Left._GetBitmapRgba32().Equals(_Right._GetBitmapRgba32());

        public double ByStandardDeviation => Bitmaps.Bgra32.Bitmap.GetStandardDeviation(_Left._GetBitmapRgba32(), _Right._GetBitmapRgba32());

        public double ByVariance => Bitmaps.Bgra32.Bitmap.GetVariance(_Left._GetBitmapRgba32(), _Right._GetBitmapRgba32());

        public int ByPixelWidth => _Left.Width - _Right.Width;

        public int ByPixelHeight => _Left.Height - _Right.Height;

        public int ByPixelArea
        {
            get
            {
                var larea = _Left.Width * _Left.Height;
                var rarea = _Right.Width * _Right.Height;
                return larea - rarea;
            }
        }

        /// <summary>
        /// Gets the number of times the smaller image is within the larger image
        /// </summary>
        public int ByOccurrences
        {
            get
            {
                return _Left._GetBitmapRgba32().FindOccurencesOf(_Right._GetBitmapRgba32()).Count();
            }
        }

        #endregion
    }    
}
