using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestImages
{
    public readonly struct Comparison
    {
        public static Comparison With(string imagePath)
        {
            var image = TestImage.FromFile(imagePath);
            return new Comparison(image);
        }

        public static Comparison With(System.IO.FileInfo finfo)
        {
            var image = TestImage.FromFile(finfo);
            return new Comparison(image);
        }

        public static Comparison With(TestImage image)
        {
            return new Comparison(image);
        }

        private Comparison(TestImage img) { _Image = img; }

        private readonly TestImage _Image;

        public TestImageEvaluator<bool> ByExactPixels
        {
            get
            {
                var img = _Image;
                return other => img.BitmapRgba32.Equals(other.BitmapRgba32);
            }
        }

        public TestImageEvaluator<double> ByStandardDeviation
        {
            get
            {
                var img = _Image;
                return other => Bitmaps.Bgra32.Bitmap.GetStandardDeviation(img.BitmapRgba32, other.BitmapRgba32);
            }
        }

        public TestImageEvaluator<double> ByVariance
        {
            get
            {
                var img = _Image;
                return other => Bitmaps.Bgra32.Bitmap.GetVariance(img.BitmapRgba32, other.BitmapRgba32);
            }
        }

        public TestImageEvaluator<int> ByPixelWidth
        {
            get
            {
                var img = _Image;
                return other => Math.Abs(img.Width - other.Width);
            }
        }

        public TestImageEvaluator<int> ByPixelHeight
        {
            get
            {
                var img = _Image;
                return other => Math.Abs(img.Height - other.Height);
            }
        }

        public TestImageEvaluator<int> ByPixelArea
        {
            get
            {
                var img = _Image;
                return other => Math.Abs( (img.Width * img.Height) - (other.Width * other.Height) );
            }
        }

        /// <summary>
        /// Gets the number of times the smaller image is within the larger image
        /// </summary>
        public TestImageEvaluator<int> ByOccurrences
        {
            get
            {
                var img = _Image;
                return other => other.BitmapRgba32.FindOccurences(img.BitmapRgba32).Count();
            }
        }
    }


}
