using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitForImages
{
    partial struct Rgba32
    {
        /// <summary>
        /// Represents a memory bitmap in Rgba32 format.
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("{Width}×{Height}")]
        public sealed class Bitmap : IEquatable<Bitmap>
        {
            #region lifecycle
            public Bitmap(Byte[] pixels, int width, int height, int stride = 0)
            {
                stride = Math.Max(stride, width * 4);
                if (pixels == null) throw new ArgumentNullException(nameof(pixels));                
                if (pixels.Length < stride * height) throw new ArgumentException(nameof(pixels));

                _Width = width;
                _Height = height;
                _ByteStride = stride;                

                _Pixels = new ArraySegment<Byte>(pixels);
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private ArraySegment<Byte> _Pixels;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private int _ByteStride;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private int _Width;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private int _Height;

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
            public int Width => _Width;
            public int Height => _Height;

            #endregion

            #region API

            public ReadOnlySpan<Rgba32> GetRow(int y)
            {
                var row = _Pixels.AsSpan(y * _ByteStride, _Width * 4);
                return System.Runtime.InteropServices.MemoryMarshal.Cast<byte, Rgba32>(row);
            }

            #endregion
        }
    }
}
