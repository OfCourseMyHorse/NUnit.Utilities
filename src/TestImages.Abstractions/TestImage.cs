using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestImages
{
    public delegate T TestImageEvaluator<T>(TestImage other);

    /// <summary>
    /// Represents a bitmap that can be used for testing purposes.
    /// </summary>
    /// <remarks>
    /// The framework image may have a different pixel format,
    /// representation or might contain rich metadata, so comparison operations
    /// are performed using an internal bitmap using RGBA32 color space.
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{Width}×{Height}")]
    public abstract partial class TestImage : IEquatable<TestImage>
    {
        #region lifecycle

        public static TestImage FromFile(string imagePath) { return new FileImage(imagePath); }

        public static TestImage FromFile(System.IO.FileInfo finfo) { return new FileImage(finfo); }

        protected TestImage()
        {
            _Rgba32Bitmap = new Lazy<Bitmaps.Bgra32.Bitmap>(CreateBitmapRgba32);
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _HashCode;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Lazy<Bitmaps.Bgra32.Bitmap> _Rgba32Bitmap;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!_HashCode.HasValue)
            {
                _HashCode = this.GetBitmapRgba32().GetHashCode();
            }

            return _HashCode.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is TestImage other && Equals(other);
        }

        /// <inheritdoc />
        public virtual bool Equals(TestImage other)
        {
            if (other == null) return false;

            if (Object.ReferenceEquals(this, other)) return true;

            var thisBitmap = this.GetBitmapRgba32();
            var otherBitmap = other.GetBitmapRgba32();
            return Bitmaps.Bgra32.Bitmap.AreEqual(thisBitmap, otherBitmap);
        }
        
        #endregion

        #region properties

        /// <summary>
        /// Gets the image width, in pixels.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// Gets the image height, in pixels.
        /// </summary>
        public abstract int Height { get; }

        #endregion

        #region API

        protected virtual void Invalidate()
        {
            _HashCode = null;
            _Rgba32Bitmap = new Lazy<Bitmaps.Bgra32.Bitmap>();
        }

        /// <summary>
        /// used to convert from the framework image being tested to our internal bitmap format.
        /// </summary>
        /// <returns>A <see cref="Bitmaps.Bgra32.Bitmap"/> object.</returns>
        protected abstract Bitmaps.Bgra32.Bitmap CreateBitmapRgba32();

        /// <summary>
        /// Gets the underlaying bitmap, converted to Rgba32 format.
        /// </summary>
        /// <returns>A <see cref="Bitmaps.Bgra32.Bitmap"/> object.</returns>
        internal protected Bitmaps.Bgra32.Bitmap GetBitmapRgba32()
        {
            return _Rgba32Bitmap.Value;
        }

        /// <summary>
        /// Writes the framework image to a stream.
        /// </summary>        
        protected virtual void WriteTo(System.IO.FileInfo finfo)
        {
            GetBitmapRgba32().SaveTo(finfo);
        }

        /// <summary>
        /// Writes the framework image to a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Fluent self.</returns>
        public TestImage SaveTo(string filePath)
        {
            var finfo = new System.IO.FileInfo(filePath);

            WriteTo(finfo);

            return this;
        }        

        public TestImage SaveTo(System.IO.FileInfo finfo)
        {
            WriteTo(finfo);

            return this;
        }

        /// <summary>
        /// Writes the image to a writing callback.
        /// </summary>
        /// <param name="writeCallback">The callback.</param>
        /// <returns>Self</returns>
        /// <remarks>
        /// The callback is a placeholder for TestAttachments.NUnit's <see cref="AttachmentInfo"/> object
        /// </remarks>
        public TestImage SaveTo(Action<Action<System.IO.FileInfo>> writeCallback)
        {
            writeCallback(finfo => SaveTo(finfo));

            return this;
        }

        public TestImage Crop(int x, int y, int w, int h)
        {
            var rect = new System.Drawing.Rectangle(x, y, w, h);
            return new CroppedImage(this, rect);
        }

        #endregion
    }
}
