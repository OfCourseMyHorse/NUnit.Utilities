using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using TestAttachments;

namespace TUnit
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

        public static ResourceInfo From(params string[] fileName) { return new ResourceInfo(fileName); }

        public static DIRINFO ResolveDirectory(params string[] parts)
        {
            return ResolveDirectory(TESTCONTEXT.Current, parts);
        }

        public static DIRINFO ResolveDirectory(TESTCONTEXT context, params string[] parts)
        {
            return context.GetResourceDirectoryInfo(parts);
        }        

        public static IEnumerable<ResourceInfo> EnumerateFromDirectory(string mask, SearchOption options)
        {
            return TESTCONTEXT
                .Current
                .GetResourceDirectoryInfo()
                .EnumerateFiles(mask, options)
                .Select(item => new ResourceInfo(item));
        }
        
        public static IEnumerable<ResourceInfo> EnumerateFromDirectory(string mask, EnumerationOptions options)
        {
            return TESTCONTEXT
                .Current
                .GetResourceDirectoryInfo()
                .EnumerateFiles(mask, options)
                .Select(item => new ResourceInfo(item));
        }        

        public ResourceInfo(params string[] fileName)
            : this(TESTCONTEXT.Current.GetResourceFileInfo(fileName)) { }        

        public ResourceInfo(TESTCONTEXT context, params string[] fileName)
            : this(context.GetResourceFileInfo(fileName)) { }

        private ResourceInfo(FILEINFO FILEINFO)
        {
            _File = new Lazy<FileInfo>(()=>_ResolveFileLink(FILEINFO));
        }

        private static FILEINFO _ResolveFileLink(FILEINFO FILEINFO)
        {            
            if (FILEINFO == null) return null;

            if (FILEINFO.TryResolveShortcutFile(out var final)) return final;

            return FILEINFO;
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

        public string ReadAllText() { return File.ReadAllText(); }

        public ArraySegment<Byte> ReadAllBytes() { return File.ReadAllBytes(); }        

        #endregion
    }
}