using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// Represents an resource file object.
    /// </summary>
    /// <remarks>    
    /// <para>Use <see cref="ResourcePathFormatAttribute"/> at the top of the test class to define the resource directory path</para>    
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{File.FullName}")]
    public class ResourceInfo
    {
        #region lifecycle

        public static implicit operator System.IO.FileInfo(ResourceInfo rinfo) { return rinfo.File; }

        public static implicit operator string(ResourceInfo rinfo) { return rinfo.File.FullName; }

        public static ResourceInfo From(string fileName) { return new ResourceInfo(fileName); }        

        public ResourceInfo(string fileName)
            : this(TestContext.CurrentContext, fileName) { }

        public ResourceInfo(TestContext context, string fileName)
        {
            File = context.GetResourceFileInfo(fileName);            
        }

        #endregion

        #region data

        public System.IO.FileInfo File { get; }

        #endregion

        #region API

        public System.IO.Stream OpenRead() => File.OpenRead();

        public string ReadAllText()
        {
            using(var s = OpenRead())
            {
                using(var t = new System.IO.StreamReader(s, true))
                {
                    return t.ReadToEnd();
                }
            }
        }

        public ArraySegment<Byte> ReadAllBytes()
        {
            using(var m = new System.IO.MemoryStream())
            {
                using (var s = OpenRead())
                {
                    s.CopyTo(m);
                }

                return m.TryGetBuffer(out var segment)
                    ? segment
                    : new ArraySegment<byte>(m.ToArray());
            }            
        }

        #endregion
    }
}