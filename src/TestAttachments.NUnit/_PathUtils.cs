using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestAttachments
{
    internal static class _PathUtils
    {
        private static readonly char[] _DirectorySeparators = _GetDirectorySeparators();
        private static readonly char[] _InvalidNameChars = System.IO.Path.GetInvalidFileNameChars();
        private static readonly char[] _InvalidPathChars = System.IO.Path.GetInvalidPathChars();

        #if !NET

        /// <summary>
		/// Checks whether a <see cref="SYSTEMENTRY"/> is not null.
		/// </summary>        
		/// <exception cref="ArgumentNullException"></exception>
		public static void GuardIsValidFileName(string fileName, bool checkForInvalidNames, string name = null)
        {
            name ??= nameof(fileName);
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(name);
            if (fileName.IndexOfAny(_InvalidNameChars) >= 0) throw new ArgumentException($"{fileName} contains invalid chars", name);
            if (checkForInvalidNames)
            {
                if (fileName == "." || fileName == "..") throw new ArgumentException($"{fileName} is an invalid file name", name);
            }
        }        

        #else

        /// <summary>
        /// Checks whether a <see cref="SYSTEMENTRY"/> is not null.
        /// </summary>        
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuardIsValidFileName(string fileName, bool checkForInvalidNames, [CallerArgumentExpression("fileName")] string name = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (fileName.IndexOfAny(_InvalidNameChars) >= 0) throw new ArgumentException($"{fileName} contains invalid chars", nameof(fileName));
            if (checkForInvalidNames)
            {
                if (fileName == "." || fileName == "..") throw new ArgumentException($"{fileName} is an invalid file name", name);
            }
        }

        #endif

        private static char[] _GetDirectorySeparators()
        {
            return System.IO.Path.DirectorySeparatorChar == System.IO.Path.AltDirectorySeparatorChar
                ? new char[] { System.IO.Path.DirectorySeparatorChar }
                : new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar };
        }

        public static bool IsDirectorySeparatorChar(Char character)
        {
            return character == System.IO.Path.DirectorySeparatorChar || character == System.IO.Path.AltDirectorySeparatorChar;
        }

        public static bool PathStartsWithNetworkDrivePrefix(string path)
        {
            if (path == null || path.Length < 2) return false;

            if (!IsDirectorySeparatorChar(path[0])) return false;
            if (!IsDirectorySeparatorChar(path[1])) return false;
            return true;
        }

        public static string[] SplitPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            // subtract network prefix

            string networkPrefix = null;

            if (PathStartsWithNetworkDrivePrefix(path))
            {
                networkPrefix = path.Substring(0, 2);
                path = path.Substring(2);
            }

            // sanitize

            path = path.Trim(_DirectorySeparators);
            var parts = path.Split(_DirectorySeparators);

            // restore network prefix

            if (networkPrefix != null)
            {
                parts[0] = networkPrefix + parts[0];
            }

            return parts;
        }

        public static string ConcatenatePaths(string basePath, params string[] relativePath)
        {
            if (string.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath));

            if (relativePath == null || relativePath.Length == 0) return basePath;

            if (relativePath.Length == 1)
            {
                var rp = relativePath[0].Trim(_DirectorySeparators);
                if (rp.IndexOfAny(_DirectorySeparators) >= 0)
                {
                    var parts = SplitPath(rp);
                    if (parts.Length != 1) return ConcatenatePaths(basePath, parts);
                }
            }

            var path = basePath.TrimEnd(_DirectorySeparators);
            foreach (var part in relativePath)
            {
                GuardIsValidFileName(part, false, nameof(relativePath));

                if (part == ".") continue;

                if (part == "..")
                {
                    var idx = path.LastIndexOfAny(_DirectorySeparators);
                    if (idx < 0) throw new ArgumentException("invalid ..", nameof(relativePath));
                    path = path.Substring(0, idx);
                    continue;
                }

                path = System.IO.Path.Combine(path, part);
            }

            return path;
        }
    }
}
