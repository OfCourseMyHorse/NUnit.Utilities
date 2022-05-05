using System;
using System.Collections.Generic;
using System.Text;

namespace TestImages
{
    /// <summary>
    /// Represents a 32 bit, pixel color in R,G,B,A format
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("R:{R} G:{G} B:{B} A:{A}")]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public readonly partial struct Rgba32 : IEquatable<Rgba32>
    {
        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.Runtime.InteropServices.FieldOffset(0)]
        public readonly UInt32 Packed;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public readonly Byte R;
        [System.Runtime.InteropServices.FieldOffset(1)]
        public readonly Byte G;
        [System.Runtime.InteropServices.FieldOffset(2)]
        public readonly Byte B;
        [System.Runtime.InteropServices.FieldOffset(3)]
        public readonly Byte A;

        #endregion

        #region equality

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (A == 0) return 0;
            return (int)Packed;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) { return obj is Rgba32 other && AreEqual(this, other); }

        /// <inheritdoc />
        public bool Equals(Rgba32 other) { return AreEqual(this, other); }

        /// <inheritdoc />
        public static bool operator ==(Rgba32 left, Rgba32 right) { return AreEqual(left, right); }

        /// <inheritdoc />
        public static bool operator !=(Rgba32 left, Rgba32 right) { return !AreEqual(left, right); }

        /// <summary>
        /// Indicates whether both pixels are equal.
        /// </summary>            
        public static bool AreEqual(Rgba32 left, Rgba32 right)
        {
            if (left.A == 0 && right.A == 0) return true;
            if (left.A == 0) return false;
            if (right.A == 0) return false;
            return left.Packed == right.Packed;
        }

        #endregion
    }
}
