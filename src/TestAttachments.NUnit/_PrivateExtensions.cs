using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework
{
    internal static class _PrivateExtensions
    {
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

        public static System.IO.FileInfo CombineWith(this System.IO.DirectoryInfo dinfo, string relativePath)
        {
            if (relativePath == null) throw new ArgumentNullException(nameof(relativePath));

            if (System.IO.Path.IsPathRooted(relativePath)) return new System.IO.FileInfo(relativePath);

            relativePath = System.IO.Path.Combine(dinfo.FullName, relativePath);

            return new System.IO.FileInfo(relativePath);
        }
    }
}
