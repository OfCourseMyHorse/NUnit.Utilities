using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitForImages
{
    /// <summary>
    /// Represents a bitmap that can be used for testing purposes.
    /// </summary>
    /// <remarks>
    /// The underlaying image may have a different pixel format,
    /// representation or might contain rich metadata.
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{Width}×{Height}")]
    public abstract partial class TestImage : IEquatable<TestImage>
    {
        #region lifecycle

        protected TestImage()
        {
            _Rgba32Bitmap = new Lazy<Rgba32.Bitmap>(CreateBitmapRgba32);
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _HashCode;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly Lazy<Rgba32.Bitmap> _Rgba32Bitmap;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!_HashCode.HasValue)
            {
                _HashCode = GetBitmapRgba32().GetHashCode();
            }

            return _HashCode.Value;
        }

        /// <inheritdoc />
        public virtual bool Equals(TestImage other)
        {
            return AreEqual(this, other);
        }

        /// <summary>
        /// Indicates whether the the two images are equal by comparing the size and pixels.
        /// </summary>
        /// <param name="a">The first image to compare.</param>
        /// <param name="b">The second image to compare.</param>
        /// <returns>true if both images are equal.</returns>
        public static bool AreEqual(TestImage a, TestImage b)
        {
            var aa = a.GetBitmapRgba32();
            var bb = b.GetBitmapRgba32();
            return Rgba32.Bitmap.AreEqual(aa, bb);
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

        protected abstract Rgba32.Bitmap CreateBitmapRgba32();

        /// <summary>
        /// Gets the underlaying bitmap, converted to Rgba32 format.
        /// </summary>
        /// <returns>The bitmap.</returns>
        protected Rgba32.Bitmap GetBitmapRgba32()
        {
            return _Rgba32Bitmap.Value;
        }

        /// <summary>
        /// Writes the underlaying image to a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        protected abstract void SaveToFile(string filePath);        

        /// <summary>
        /// Writes the underlaying image to a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Fluent self.</returns>
        public TestImage SaveTo(string filePath)
        {
            SaveToFile(filePath);

            _LastSavedPath = filePath;

            return this;
        }        

        public TestImage SaveTo(NUnit.Framework.AttachmentInfo ainfo)
        {
            ainfo.WriteFile(finfo => SaveTo(finfo.FullName));

            return this;
        }

        #endregion
    }
}
