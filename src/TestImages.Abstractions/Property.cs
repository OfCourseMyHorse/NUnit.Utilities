using System;
using System.Collections.Generic;
using System.Text;

namespace TestImages
{
    public static class Property
    {
        public static int PixelWidth(TestImage image) => image.Width;
        public static int PixelHeight(TestImage image) => image.Height;
        public static int PixelArea(TestImage image) => image.Width * image.Height;
        public static int PixelsHashCode(TestImage image) => image.GetHashCode();
    }
}
