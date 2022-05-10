using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows
{
    public static class TestImageExtensions
    {
        public static TestImages.TestImage ToTestImage(this Media.Imaging.BitmapSource bitmap)
        {            
            return TestImages.WpfTestImage.FromBitmapSource(bitmap);
        }

        public static TestImages.TestImage ToTestImage(this Media.Visual visual)
        {
            return TestImages.WpfTestImage.FromRender(visual);
        }

        /// <summary>
        /// Saves an image as a test file attachment
        /// </summary>
        /// <param name="bitmap">The bitmap to save</param>
        /// <param name="attachmentCallback">An instance of TestImages.AttachmentInfo.</param>
        public static void SaveAsAttachment(this System.Windows.Media.Imaging.BitmapSource bitmap, Action<Action<System.IO.FileInfo>> attachmentCallback)
        {
            attachmentCallback(finfo => TestImages.WpfRenderFactory.SaveTo(bitmap, finfo));
        }
    }
}
