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
        #region data
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _HashCode;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!_HashCode.HasValue)
            {
                var bytes = GetPixelsBytes();

                int h = 0;

                for (int x = 0; x < bytes.Length; ++x)
                {
                    h += bytes[x];
                    h <<= 3;
                }

                h += this.Width * this.Height;

                _HashCode = h;
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
            if (a.Width != b.Width) return false;
            if (a.Height != b.Height) return false;

            var aBytes = a.GetPixelsBytes();
            var bBytes = b.GetPixelsBytes();

            return aBytes.SequenceEqual(bBytes);
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

        /// <summary>
        /// Gets the pixel data.
        /// </summary>
        /// <returns>a read only sequence of bytes.</returns>
        /// <remarks>
        /// This is a temporary method, awaiting for a better definition
        /// on how to get the data in a specific format.
        /// </remarks>
        protected abstract ReadOnlySpan<Byte> GetPixelsBytes();

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

        #endregion
    }
}
