using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestImages
{
    public delegate T TestImageEvaluator<T>(TestImage other);

    /// <summary>
    /// Represents a bitmap that can be used for testing purposes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The framework image may have a different pixel format,
    /// representation or might contain rich metadata, so comparison operations
    /// are performed using an internal bitmap using RGBA32 color space.    
    /// </para>
    /// <para>
    /// Derived classes: <see cref="CroppedImage"/> , <see cref="FileImage"/>
    /// </para>
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("TestImage: {Width}×{Height}")]
    public abstract partial class TestImage : IEquatable<TestImage>
    {
        #region lifecycle

        public static TestImage FromFile(string imagePath) { return new FileImage(imagePath); }

        public static TestImage FromFile(System.IO.FileInfo finfo) { return new FileImage(finfo); }

        protected TestImage() { }        

        #endregion

        #region data        

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Bitmaps.Bgra32.Bitmap _Rgba32Bitmap;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _Rgba32HashCode;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private uint? _Rgba32CheckSum;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Byte[] _Rgba32Sha256;        

        protected virtual void Invalidate()
        {
            _Rgba32Bitmap = null;
            _Rgba32HashCode = null;
            _Rgba32CheckSum = null;
            _Rgba32Sha256 = null;            
        }

        /// <summary>
        /// Gets the underlaying bitmap, converted to Rgba32 format to simplify comparison across multiple pixel formats.
        /// </summary>        
        protected internal Bitmaps.Bgra32.Bitmap _GetBitmapRgba32()
        {
            return LazyInitializer.EnsureInitialized(ref _Rgba32Bitmap, CreateBitmapRgba32);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            _Rgba32HashCode ??= this._GetBitmapRgba32().GetHashCode();
            return _Rgba32HashCode.Value;
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

            var thisBitmap = this._GetBitmapRgba32();
            var otherBitmap = other._GetBitmapRgba32();
            return Bitmaps.Bgra32.Bitmap.AreEqual(thisBitmap, otherBitmap);
        }

        #endregion        

        #region properties        

        /// <summary>
        /// Gets the image width, in pixels.
        /// </summary>
        public virtual int Width => _GetBitmapRgba32().Width;

        /// <summary>
        /// Gets the image height, in pixels.
        /// </summary>
        public virtual int Height => _GetBitmapRgba32().Height;

        /// <summary>
        /// Gets the Sha256 of the pixels.
        /// </summary>
        #pragma warning disable CA1819 // Properties should not return arrays
        public virtual Byte[] PixelsSha256 => LazyInitializer.EnsureInitialized(ref _Rgba32Sha256, _GetBitmapRgba32().GetPixelsSha256);
        #pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets the Sha256 of the pixels as an hexadecimal string.
        /// </summary>
        public string PixelsSha256Hex
        {
            get
            {
                return string.Join("", PixelsSha256.Select(item => item.ToString("X2", System.Globalization.CultureInfo.InvariantCulture)));
            }
        }

        /// <summary>
        /// Gets the checksum of the properties and pixels of the bitmap.
        /// </summary>
        public uint CheckSum
        {
            get
            {
                _Rgba32CheckSum ??= this._GetBitmapRgba32().GetCheckSum();
                return _Rgba32CheckSum.Value;
            }
        }

        #endregion

        #region API        

        public TestImage Crop(int x, int y, int w, int h)
        {
            var rect = new System.Drawing.Rectangle(x, y, w, h);
            return new CroppedImage(this, rect);
        }

        /// <summary>
        /// used to convert from the framework image being tested to our internal bitmap format.
        /// </summary>
        /// <returns>A <see cref="Bitmaps.Bgra32.Bitmap"/> object.</returns>
        protected abstract Bitmaps.Bgra32.Bitmap CreateBitmapRgba32();

        #endregion

        #region serialization

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
        /// Writes the framework image to a file.
        /// </summary>        
        protected virtual void WriteTo(System.IO.FileInfo finfo)
        {
            _GetBitmapRgba32().SaveTo(finfo);
        }

        #endregion

        #region testing

        public TestImageComparingPair ComparedTo(TestImage other) { return new TestImageComparingPair(this, other); }

        #endregion
    }
}
