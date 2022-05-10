using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestImages.Bitmaps
{
    public delegate ReadOnlySpan<T> BitmapRowEvaluator<T>(int y) where T : unmanaged;

    /// <summary>
    /// Represents a bitmap pixels bitmap.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Width}×{Height}")]
    public partial class Bitmap<T> : IEquatable<Bitmap<T>>
        where T: unmanaged, IEquatable<T>
    {
        #region lifecycle
        public Bitmap(Byte[] pixels, int width, int height, int byteStride = 0)
            : this(_GetRowEvaluator(pixels, width, height, byteStride), width, height)
        { }

        private static unsafe BitmapRowEvaluator<T> _GetRowEvaluator(Byte[] pixels, int width, int height, int byteStride = 0)
        {
            width *= sizeof(T); // pixels to bytes

            byteStride = Math.Max(byteStride, width);

            if (pixels.Length < (byteStride * (height - 1)) + width) throw new ArgumentException(nameof(pixels));

            ReadOnlySpan<T> getRow(int y)
            {
                var row = pixels.AsSpan(y * byteStride, width);
                return System.Runtime.InteropServices.MemoryMarshal.Cast<byte, T>(row);
            }

            return getRow;
        }

        private Bitmap(BitmapRowEvaluator<T> rows, int width, int height)
        {
            if (rows == null) throw new ArgumentNullException(nameof(rows));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));

            _Width = width;
            _Height = height;
            _RowEvaluator = rows;
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _Width;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _Height;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private BitmapRowEvaluator<T> _RowEvaluator;

        #endregion

        #region equality

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int h = 0;

            for (int y = 0; y < _Height; ++y)
            {
                var row = GetRow(y);

                for (int x = 0; x < _Width; ++x)
                {
                    h += row[x].GetHashCode();
                    h *= 17;
                }
            }

            return h;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) { return obj is Bitmap<T> other && AreEqual(this, other); }

        /// <inheritdoc />
        public bool Equals(Bitmap<T> other) { return AreEqual(this, other); }

        /// <summary>
        /// Indicates whether both objects are equal Pixel by Pixel.
        /// </summary>            
        public static bool AreEqual(Bitmap<T> left, Bitmap<T> right)
        {
            if (object.ReferenceEquals(left, right)) return true;
            if (object.ReferenceEquals(left, null)) return false;
            if (object.ReferenceEquals(left, null)) return false;

            if (left.Width != right.Width) return false;
            if (left.Height != right.Height) return false;

            // stride is an implementation detail,
            // so we don't take it into
            // account for bitmap comparison.

            for (int y = 0; y < left.Height; ++y)
            {
                var arow = left.GetRow(y);
                var brow = right.GetRow(y);

                if (!arow.SequenceEqual(brow)) return false;
            }

            return true;
        }

        /// <inheritdoc />
        public static bool operator ==(Bitmap<T> left, Bitmap<T> right) { return AreEqual(left, right); }

        /// <inheritdoc />
        public static bool operator !=(Bitmap<T> left, Bitmap<T> right) { return !AreEqual(left, right); }

        #endregion

        #region properties

        /// <summary>
        /// Image width in pixels
        /// </summary>
        public int Width => _Width;

        /// <summary>
        /// Image height in pixels
        /// </summary>
        public int Height => _Height;

        /// <summary>
        /// Gets the pixel at the given coordinates
        /// </summary>
        /// <param name="x">the horizontal pixel coordinate</param>
        /// <param name="y">the vertical pixel coordinate</param>
        /// <returns></returns>
        public T this[int x, int y]
        {
            get
            {
                if (x < 0) throw new ArgumentOutOfRangeException(nameof(x));
                else if (x >= _Width) throw new ArgumentOutOfRangeException(nameof(x));

                return GetRow(y)[x];
            }
        }

        #endregion

        #region API

        protected void CropFrom(Bitmap<T> source, int x, int y, int w, int h)
        {
            if (w > source.Width) throw new ArgumentOutOfRangeException(nameof(w));
            if (h > source.Height) throw new ArgumentOutOfRangeException(nameof(h));

            if (x < 0 || x >= source.Width - w) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= source.Height - h) throw new ArgumentOutOfRangeException(nameof(y));

            ReadOnlySpan<T> getRow(int yy)
            {
                return source.GetRow(yy + y).Slice(x, w);
            }

            this._RowEvaluator = getRow;
            this._Width = w;
            this._Height = h;
        }

        public Bitmap<T> Crop(int x, int y, int w, int h)
        {
            var bmp = new Bitmap<T>(Array.Empty<Byte>(), 0, 0);
            bmp.CropFrom(this, x, y, w, h);
            return bmp;
        }

        /// <summary>
        /// Gets a row of pixels for the given row index.
        /// </summary>
        /// <param name="y">The row index.</param>
        /// <returns>A span of pixels.</returns>            
        public ReadOnlySpan<T> GetRow(int y)
        {
            if (y < 0 || y >= _Height) throw new ArgumentOutOfRangeException(nameof(y));
            var row = _RowEvaluator(y);
            if (row.Length != _Width) throw new InvalidOperationException($"Expected {_Width}, found {row.Length}");
            return row;
        }

        public static IEnumerable<(T Left, T Right)> ZipPixels(Bitmap<T> leftBitmap, Bitmap<T> rightBitmap)
        {
            if (leftBitmap.Width != rightBitmap.Width || leftBitmap.Height != rightBitmap.Height) throw new ArgumentException("dimensions mismatch", nameof(rightBitmap));

            return leftBitmap.EnumeratePixels().Zip(rightBitmap.EnumeratePixels(), (l,r) => (l,r) );
        }

        public IEnumerable<T> EnumeratePixels()
        {
            var row = new T[Width];

            for (int y = 0; y < Height; ++y)
            {
                GetRow(y).CopyTo(row);                

                for (int x = 0; x < row.Length; ++x)
                {
                    yield return row[x];
                }
            }
        }

        public IEnumerable<System.Drawing.Point> FindOccurences(Bitmap<T> other)
        {
            if (other.Width > this.Width) yield break;
            if (other.Height > this.Height) yield break;            

            for (int y = 0; y < this.Height - other.Height; ++y)
            {
                for (int x = 0; x < this.Width - other.Width; ++x)
                {
                    var thisCrop = this.Crop(x, y, other.Width, other.Height);

                    if (thisCrop.Equals(other)) yield return new System.Drawing.Point(x, y);
                }
            }
        }

        #endregion
    }
}
