using System;
using System.Collections.Generic;
using System.Text;

namespace TestImages.Bitmaps
{
    /// <summary>
    /// Represents a 32 bit, pixel color in R,G,B,A format
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("R:{R} G:{G} B:{B} A:{A}")]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public readonly partial struct Bgra32 : IEquatable<Bgra32>
    {
        #region lifecycle

        public static implicit operator Bgra32(System.Drawing.Color color)
        {
            return new Bgra32(color.R, color.G, color.B, color.A);
        }

        public Bgra32(Byte r, Byte g, Byte b, Byte a) : this()
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.Runtime.InteropServices.FieldOffset(0)]
        public readonly UInt32 Packed;

        [System.Runtime.InteropServices.FieldOffset(2)]
        public readonly Byte B;        
        [System.Runtime.InteropServices.FieldOffset(1)]
        public readonly Byte G;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public readonly Byte R;
        [System.Runtime.InteropServices.FieldOffset(3)]
        public readonly Byte A;

        #endregion

        #region equality

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return A == 0 ? 0 : (int)Packed;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) { return obj is Bgra32 other && AreEqual(this, other); }

        /// <inheritdoc />
        public bool Equals(Bgra32 other) { return AreEqual(this, other); }

        /// <inheritdoc />
        public static bool operator ==(Bgra32 left, Bgra32 right) { return AreEqual(left, right); }

        /// <inheritdoc />
        public static bool operator !=(Bgra32 left, Bgra32 right) { return !AreEqual(left, right); }

        /// <summary>
        /// Indicates whether both pixels are equal.
        /// </summary>            
        public static bool AreEqual(Bgra32 left, Bgra32 right)
        {
            return (left.A == 0 && right.A == 0) || left.Packed == right.Packed;
        }

        #endregion

        #region properties

        public float Brightness
        {
            get
            {
                const uint RLuminanceWeight16 = 19562;
                const uint GLuminanceWeight16 = 38550;
                const uint BLuminanceWeight16 = 7424;
                const float rcp = 1f / (255f * 255f);

                uint accum = 0;
                accum += RLuminanceWeight16 * (uint)this.R;
                accum += GLuminanceWeight16 * (uint)this.G;
                accum += BLuminanceWeight16 * (uint)this.B;

                accum >>= 16;
                accum *= this.A;

                return (float)accum * rcp;
            }
        }        

        #endregion

        #region API

        public static float Distance(Bgra32 a, Bgra32 b)
        {
            if (a.A == 0 && b.A == 0) return 0;

            // pixel distance is better done in premultiplied space

            var va = new System.Numerics.Vector3(a.R, a.G, a.B) * a.A;
            var vb = new System.Numerics.Vector3(b.R, b.G, b.B) * b.A;            

            return System.Numerics.Vector3.Distance(va, vb) / (float)(255*255);
        }

        internal Bgra32 GetSwapRedAndblue() { return new Bgra32(B, G, R, A); }

        #endregion
    }
}
