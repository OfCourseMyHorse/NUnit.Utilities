using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestImages
{
    /// <summary>
    /// represents the 2nd operator of a Image Test Assert
    /// </summary>
    /// <remarks>
    /// Usage: <see cref="TestImage.AssertThat(ImageComparison(x).xxx)"/>
    /// </remarks>
    public sealed class ImageComparison
    {
        #region lifecycle
        public static ImageComparison With(string imagePath)
        {
            var image = TestImage.FromFile(imagePath);
            return new ImageComparison(image);
        }

        public static ImageComparison With(System.IO.FileInfo finfo)
        {
            var image = TestImage.FromFile(finfo);
            return new ImageComparison(image);
        }

        public static ImageComparison With(TestImage image)
        {
            return new ImageComparison(image);
        }

        private ImageComparison(TestImage img) { _Image = img; }

        #endregion

        #region data

        private readonly TestImage _Image;

        #endregion

        #region properties

        public TestImageEvaluator<bool> ByExactPixels => other => new TestImageComparingPair(other, _Image).ByExactPixels;

        public TestImageEvaluator<double> ByStandardDeviation => other => new TestImageComparingPair(other, _Image).ByStandardDeviation;

        public TestImageEvaluator<double> ByVariance => other => new TestImageComparingPair(other, _Image).ByVariance;

        public TestImageEvaluator<int> ByPixelWidth => other => new TestImageComparingPair(other, _Image).ByPixelWidth;

        public TestImageEvaluator<int> ByPixelHeight => other => new TestImageComparingPair(other, _Image).ByPixelHeight;

        public TestImageEvaluator<int> ByPixelArea => other => new TestImageComparingPair(other, _Image).ByPixelArea;

        /// <summary>
        /// Gets the number of times the smaller image is within the larger image
        /// </summary>
        public TestImageEvaluator<int> ByOccurrences => other => new TestImageComparingPair(other, _Image).ByOccurrences;

        #endregion
    }


}
