﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TestImages
{
    /// <summary>
    /// General image properties, and pixel properties in BGRA32 space.
    /// </summary>
    public static class ImageProperty
    {
        public static int PixelWidth(TestImage image) => image.Width;
        public static int PixelHeight(TestImage image) => image.Height;
        public static int PixelArea(TestImage image) => image.Width * image.Height;

        [Obsolete("Use CheckSum", true)]
        public static int PixelsHashCode(TestImage image) => image.GetHashCode();

        public static uint CheckSum(TestImage image) => image.CheckSum;

        public static string PixelsSha256Hex(TestImage image) => image.PixelsSha256Hex;

        /// <summary>
        /// Gets the average brightness of the image.
        /// </summary>        
        public static double AverageBrightness(TestImage image)
        {
            return image._GetBitmapRgba32()
                .EnumeratePixels()
                .Select(pixel => (double)pixel.Brightness)
                .Average();
        }

        /// <summary>
        /// Gets the number of pixels that are fully transparent.
        /// </summary>
        public static int TransparentPixelsCount(TestImage image)
        {
            return image._GetBitmapRgba32()
                .EnumeratePixels()
                .Count(pixel => pixel.A == 0);
        }

        /// <summary>
        /// Gets the number of pixels that are fully opaque.
        /// </summary>
        public static int OpaquePixelsCount(TestImage image)
        {
            return image._GetBitmapRgba32()
                .EnumeratePixels()
                .Count(pixel => pixel.A == 255);
        }

        /// <summary>
        /// Gets the number of pixels that are not fully opaque.
        /// </summary>
        public static int NotOpaquePixelsCount(TestImage image)
        {
            return image._GetBitmapRgba32()
                .EnumeratePixels()
                .Count(pixel => pixel.A != 255);
        }

        public static TestImageEvaluator<int> Count(Bitmaps.Bgra32 color)
        {
            int countColors(TestImage image)
            {
                return image._GetBitmapRgba32().EnumeratePixels().Count(pixel => pixel == color);
            }

            return countColors;
        }
    }
}
