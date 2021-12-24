using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitForImages
{
    partial struct Rgba32
    {
        public delegate ReadOnlySpan<Rgba32> BitmapRowEvaluator(int y);

        /// <summary>
        /// Represents a memory bitmap in Rgba32 format.
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("{Width}×{Height}")]
        public sealed class Bitmap : IEquatable<Bitmap>
        {
            #region lifecycle
            public Bitmap(Byte[] pixels, int width, int height, int stride = 0)
            {
                if (pixels == null) throw new ArgumentNullException(nameof(pixels));
                if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
                if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

                stride = Math.Max(stride, width * 4);
                
                if (pixels.Length < stride * height) throw new ArgumentException(nameof(pixels));                

                ReadOnlySpan<Rgba32> getRow(int y)
                {
                    var row = pixels.AsSpan(y * stride, _Width * 4);
                    return System.Runtime.InteropServices.MemoryMarshal.Cast<byte, Rgba32>(row);
                }

                _Width = width;
                _Height = height;
                _RowEvaluator = getRow;
            }

            public Bitmap(BitmapRowEvaluator rows, int width, int height)
            {
                if (rows == null) throw new ArgumentNullException(nameof(rows));
                if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
                if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

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
            private BitmapRowEvaluator _RowEvaluator;

            #endregion

            #region equality

            /// <inheritdoc />
            public override int GetHashCode()
            {
                int h = 0;

                for(int y=0; y < _Height; ++y)
                {
                    var row = GetRow(y);

                    for(int x=0; x < _Width; ++x)
                    {
                        h += row[x].GetHashCode();
                        h *= 17;
                    }
                }

                return h;
            }

            /// <inheritdoc />
            public override bool Equals(object obj) { return obj is Bitmap other && AreEqual(this, other); }

            /// <inheritdoc />
            public bool Equals(Bitmap other) { return AreEqual(this, other); }

            /// <summary>
            /// Indicates whether both objects are equal Pixel by Pixel.
            /// </summary>            
            public static bool AreEqual(Bitmap left, Bitmap right)
            {
                if (object.ReferenceEquals(left, right)) return true;
                if (object.ReferenceEquals(left, null)) return false;
                if (object.ReferenceEquals(left, null)) return false;

                if (left.Width != right.Width) return false;
                if (left.Height != right.Height) return false;

                // stride is an implementation detail,
                // so we don't take it into
                // account for bitmap comparison.

                for(int y=0; y < left.Height; ++y)
                {
                    var arow = left.GetRow(y);
                    var brow = right.GetRow(y);

                    if (!arow.SequenceEqual(brow)) return false;
                }

                return true;
            }

            /// <inheritdoc />
            public static bool operator ==(Bitmap left, Bitmap right) { return AreEqual(left, right); }

            /// <inheritdoc />
            public static bool operator !=(Bitmap left, Bitmap right) { return !AreEqual(left, right); }

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

            #endregion

            #region API            

            public Bitmap Crop(int x, int y, int w, int h)
            {
                if (w > _Width) throw new ArgumentOutOfRangeException(nameof(w));
                if (h > _Height) throw new ArgumentOutOfRangeException(nameof(h));

                if (x < 0 || x >= _Width - w) throw new ArgumentOutOfRangeException(nameof(x));
                if (y < 0 || y >= _Height - h) throw new ArgumentOutOfRangeException(nameof(y));

                ReadOnlySpan<Rgba32> getRow(int yy)
                {
                    return this.GetRow(yy + y).Slice(x, w);
                }

                return new Bitmap(getRow, w, h);
            }

            /// <summary>
            /// Gets a row of pixels for the given row index.
            /// </summary>
            /// <param name="y">The row index.</param>
            /// <returns>A span of pixels.</returns>            
            public ReadOnlySpan<Rgba32> GetRow(int y)
            {
                if (y < 0 || y >= _Height) throw new ArgumentOutOfRangeException(nameof(y));
                var row = _RowEvaluator(y);
                if (row.Length != _Width) throw new InvalidOperationException($"Expected {_Width}, found {row.Length}");
                return row;
            }

            #endregion
        }
    }
}
