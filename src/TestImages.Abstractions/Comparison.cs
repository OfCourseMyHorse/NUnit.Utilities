using System;
using System.Collections.Generic;
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
                return other => img.GetBitmapRgba32().Equals(other.GetBitmapRgba32());
            }
        }

        public TestImageEvaluator<double> ByStandardDeviation
        {
            get
            {
                var img = _Image;
                return other => Bgra32.Bitmap.GetStandardDeviation(img.GetBitmapRgba32(), other.GetBitmapRgba32());
            }
        }

        public TestImageEvaluator<double> ByVariance
        {
            get
            {
                var img = _Image;
                return other => Bgra32.Bitmap.GetVariance(img.GetBitmapRgba32(), other.GetBitmapRgba32());
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
    }


}
