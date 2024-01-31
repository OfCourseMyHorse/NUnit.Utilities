using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using FILEINFO = System.IO.FileInfo;

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
        #region diagnostics

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

        public static System.IO.DirectoryInfo ResolveDirectory(params string[] parts)
        {
            return ResolveDirectory(TestContext.CurrentContext, parts);
        }

        public static System.IO.DirectoryInfo ResolveDirectory(TestContext context, params string[] parts)
        {
            return context.GetResourceDirectoryInfo(parts);
        }

        public static ResourceInfo From(params string[] fileName) { return new ResourceInfo(fileName); }        

        public static IEnumerable<ResourceInfo> EnumerateFromDirectory(string mask, SearchOption options)
        {
            return TestContext.CurrentContext
                .GetResourceDirectoryInfo()
                .EnumerateFiles(mask, options)
                .Select(item => new ResourceInfo(item));
        }

        #if !NETFRAMEWORK
        public static IEnumerable<ResourceInfo> EnumerateFromDirectory(string mask, EnumerationOptions options)
        {
            return TestContext.CurrentContext
                .GetResourceDirectoryInfo()
                .EnumerateFiles(mask, options)
                .Select(item => new ResourceInfo(item));
        }
        #endif

        public ResourceInfo(params string[] fileName)
            : this(TestContext.CurrentContext, fileName) { }
        

        // this would be useful to initialize static class fields
        private ResourceInfo(Type context, string fileName)
        {
            throw new NotImplementedException("we need a way to get information equivalent to a TestContext");            
        }

        public ResourceInfo(TestContext context, params string[] fileName)
        {
            var finfo = context.GetResourceFileInfo(fileName);
            _File = new Lazy<FileInfo>(() => _ResolveFileLink(finfo));
        }

        private ResourceInfo(FILEINFO finfo)
        {
            _File = new Lazy<FileInfo>(()=>_ResolveFileLink(finfo));
        }

        private static FILEINFO _ResolveFileLink(FILEINFO finfo)
        {
            if (finfo == null) return null;

            var final = ShortcutUtils.TryGetSystemPathFromFile(finfo.FullName, true);

            // TODO: if final is a network address, download it to a cache

            return final != null
                ? new FILEINFO(final)
                : finfo;
        }

        #endregion

        #region data

        private readonly Lazy<FILEINFO> _File;        

        #endregion

        #region properties

        public FILEINFO File => _File.Value;

        public string Name => File?.Name ?? string.Empty;

        public string FilePath => File.FullName;

        #endregion

        #region operators

        public static implicit operator FILEINFO(ResourceInfo rinfo) { return rinfo.File; }

        public static implicit operator string(ResourceInfo rinfo) { return rinfo.File.FullName; }

        #endregion

        #region API

        public System.IO.Stream OpenRead() => File.OpenRead();

        public T OpenRead<T>(Func<System.IO.Stream,T> readFunc)
        {
            using(var s = OpenRead())
            {
                return readFunc(s);
            }
        }

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