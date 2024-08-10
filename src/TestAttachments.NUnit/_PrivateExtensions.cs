using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework
{
    internal static class _PrivateExtensions
    {
        #if NETFRAMEWORK

        public static string Replace(this string current, string oldValue, string newValue, StringComparison stringComparison)
        {
            if (stringComparison ==  StringComparison.OrdinalIgnoreCase || stringComparison == StringComparison.InvariantCultureIgnoreCase)
            {
                throw new NotImplementedException();
            }

            return current.Replace(oldValue, newValue);
        }

        #endif

        public static bool IsRepositoryDatabase(this System.IO.DirectoryInfo dinfo)
        {
            if (dinfo.Name == ".svn")
            {
                return true;
            }

            if (dinfo.Name == ".git")
            {
                return true;
            }

            return false;
        }

        public static Uri ToUri(this System.IO.DirectoryInfo dinfo)
        {
            return dinfo == null
                ? null
                : new Uri(dinfo.FullName, UriKind.Absolute);
        }

        public static System.IO.DirectoryInfo ToDirectoryInfo(this string directoryPath, bool resolveLink = false)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return null;

            if (resolveLink)
            {
                var final = ShortcutUtils.TryGetSystemPathFromFile(directoryPath, true);
                if (final != null) directoryPath = final;
            }

            return new System.IO.DirectoryInfo(directoryPath);
        }

        public static System.IO.FileInfo ConcatenateToFile(this System.IO.DirectoryInfo dinfo, params string[] relativePath)
        {
            if (relativePath == null) throw new ArgumentNullException(nameof(relativePath));

            if (relativePath.Length == 1)
            {
                if (System.IO.Path.IsPathRooted(relativePath[0])) return new System.IO.FileInfo(relativePath[0]);
            }            

            var finalPath = _PathUtils.ConcatenatePaths(dinfo.FullName, relativePath);

            return new System.IO.FileInfo(finalPath);
        }

        public static System.IO.DirectoryInfo ConcatenateToDirectory(this System.IO.DirectoryInfo dinfo, params string[] relativePath)
        {
            if (relativePath == null) throw new ArgumentNullException(nameof(relativePath));

            if (relativePath.Length == 1)
            {
                if (System.IO.Path.IsPathRooted(relativePath[0])) return new System.IO.DirectoryInfo(relativePath[0]);
            }

            var finalPath = _PathUtils.ConcatenatePaths(dinfo.FullName, relativePath);

            return new System.IO.DirectoryInfo(finalPath);
        }
    }
}
