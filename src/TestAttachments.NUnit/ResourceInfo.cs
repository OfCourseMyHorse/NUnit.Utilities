using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// Represents an resource file object.
    /// </summary>
    /// <remarks>    
    /// <para>Use <see cref="ResourcePathFormatAttribute"/> at the top of the test class to define the resource directory path</para>    
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
    public class ResourceInfo
    {
        #region debug

        internal string ToDebuggerDisplay()
        {
            if (File != null)
            {
                var text = File.Exists ? string.Empty : "⚠ NOT FOUND : ";

                return text += File.FullName;
            }

            if (string.IsNullOrWhiteSpace(Name)) return "⚠ NULL";

            return Name;
        }

        #endregion

        #region lifecycle

        public static implicit operator System.IO.FileInfo(ResourceInfo rinfo) { return rinfo.File; }

        public static implicit operator string(ResourceInfo rinfo) { return rinfo.File.FullName; }

        public static ResourceInfo From(string fileName) { return new ResourceInfo(fileName); }        

        public static IEnumerable<ResourceInfo> FromDirectory(string mask, SearchOption options)
        {
            return TestContext.CurrentContext
                .GetResourceDirectoryInfo()
                .EnumerateFiles(mask, options)
                .Select(item => new ResourceInfo(item));
        }

        #if !NETSTANDARD2_0
        public static IEnumerable<ResourceInfo> FromDirectory(string mask, EnumerationOptions options)
        {
            return TestContext.CurrentContext
                .GetResourceDirectoryInfo()
                .EnumerateFiles(mask, options)
                .Select(item => new ResourceInfo(item));
        }
        #endif

        public ResourceInfo(string fileName)
            : this(TestContext.CurrentContext, fileName) { }

        public ResourceInfo(TestContext context, string fileName)
        {
            var file = context.GetResourceFileInfo(fileName);
            _File = _ResolveFileLink(file);
        }

        // this would be useful to initialize static class fields
        private ResourceInfo(Type context, string fileName)
        {
            throw new NotImplementedException("we need a way to get information equivalent to a TestContext");            
        }

        private ResourceInfo(System.IO.FileInfo finfo)
        {
            _File = _ResolveFileLink(finfo);
        }

        private static Lazy<System.IO.FileInfo> _ResolveFileLink(System.IO.FileInfo finfo)
        {
            System.IO.FileInfo action()
            {
                if (finfo == null) return null;

                var final = ShortcutUtils.TryGetSystemPathFromFile(finfo.FullName, true);

                // TODO: if final is a network address, download it to a cache

                return final != null
                    ? new System.IO.FileInfo(final)
                    : finfo;
            }

            return new Lazy<FileInfo>(action);
        }

        #endregion

        #region data

        private readonly Lazy<System.IO.FileInfo> _File;

        public System.IO.FileInfo File => _File.Value;

        public string Name => File?.Name ?? string.Empty;

        public string FilePath => File.FullName;

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