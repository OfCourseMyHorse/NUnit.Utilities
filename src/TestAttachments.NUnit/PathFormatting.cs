﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{
    static class PathFormatting
    {
        #region constants

        // these are the recommended path for tests:
        // https://github.com/nunit/nunit/issues/1768#issuecomment-242454699

        private const string DefaultResourceFormat = "{TestDirectory}";
        private const string DefaultAttachmentFormat = "{WorkDirectory}";

        #endregion

        #region API

        public static bool FindAttachmentShowDirectoryLinkEnabled(this TestContext context)
        {
            var properties = PathFormatAttribute._FindProperties<AttachmentPathFormatAttribute>(context, "AttachShowDirectoryShortcut");

            return properties != null && properties.Get("AttachShowDirectoryShortcut") is bool value && value;
        }

        public static string FindAttachmentPathFormat(this TestContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var properties = PathFormatAttribute._FindProperties<AttachmentPathFormatAttribute>(context, "AttachmentPathFormat");

            return properties == null
                ? DefaultAttachmentFormat
                : properties.Get("AttachmentPathFormat") as string;
        }

        public static string FindResourcesPathFormat(this TestContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var properties = PathFormatAttribute._FindProperties<ResourcePathFormatAttribute>(context, "ResourcePathFormat");

            return properties == null
                ? DefaultResourceFormat
                : properties.Get("ResourcePathFormat") as string;
        }

        public static string FindResourcesPathFormat(this Type context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var properties = PathFormatAttribute._FindProperties<ResourcePathFormatAttribute>(context);

            return properties == null
                ? DefaultResourceFormat
                : properties.Get("ResourcePathFormat") as string;
        }

        /// <summary>
        /// formats the current path with.
        /// </summary>
        /// <param name="format">The format string taken from <see cref="AttachmentPathFormatAttribute"/></param>
        /// <param name="context">The current test context</param>
        /// <returns></returns>
        public static string FormatAttachmentPath(this string format, TestContext context)
        {
            // defaults

            if (string.IsNullOrWhiteSpace(format)) format = DefaultAttachmentFormat;

            if (format == "?") format = "*/?";

            // shortcuts

            // this is the recommended path for tests: https://github.com/nunit/nunit/issues/1768#issuecomment-242454699
            format = format.Replace("*", DefaultAttachmentFormat, StringComparison.Ordinal);
            format = format.Replace("{DefaultDirectory}", DefaultAttachmentFormat, StringComparison.Ordinal);
            format = format.Replace("?", "{ID}", StringComparison.Ordinal);

            return _FormatPath(format, context, context.WorkDirectory);
        }

        /// <summary>
        /// formats the current path with.
        /// </summary>
        /// <param name="format">The format string taken from <see cref="ResourcePathFormatAttribute"/></param>
        /// <param name="context">The current test context</param>
        /// <returns></returns>
        public static string FormatResourcePath(this string format, TestContext context)
        {
            // defaults

            if (string.IsNullOrWhiteSpace(format)) format = DefaultResourceFormat;

            // shortcuts
            
            format = format.Replace("*", DefaultResourceFormat, StringComparison.Ordinal);
            format = format.Replace("{DefaultDirectory}", DefaultResourceFormat, StringComparison.Ordinal);

            return _FormatPath(format, context, context.TestDirectory);
        }

        private static string _FormatPath(string currPath, TestContext context, string defaultBasePath)
        {
            // TODO:
            // - support {<<<} and {>>>} as macro for paths search
            // - support "./xyz" to concatenate with upper level formatting

            //--------------------------------------------------------- absolute path macros (format can only have one of those):            

            _FindUpperDirectory(context, ref currPath, "{RepositoryRoot}", dinfo => dinfo.IsRepositoryDatabase());

            _FindUpperFile(context, ref currPath, "{SolutionDirectory}", finfo => finfo.Extension.ToLower().EndsWith("sln"));
            _FindUpperFile(context, ref currPath, "{ProjectDirectory}", finfo => finfo.Extension.ToLower().EndsWith("csproj"));
            

            if (currPath.Length > 4 && currPath.StartsWith("{\"")) // handle "{\"somefilename.ext\"}\whatever\whatever"
            {                
                var idx = currPath.IndexOf("\"}");
                if (idx >= 0)
                {
                    var key = currPath.Substring(0, idx + 2);
                    var val = key.Substring(2, key.Length - 4).ToLower();
                    _FindUpperFile(context, ref currPath, key, finfo => finfo.Name.ToLower() == val); // TODO: this is NOT cross platform
                }
            }

            _ReplacePrefix(ref currPath, "{CurrentDirectory}", Environment.CurrentDirectory);

            // input paths
            _ReplacePrefix(ref currPath, "{TestDirectory}", context.TestDirectory);
            _ReplacePrefix(ref currPath, "{TestDirectoryRoot}", new System.IO.DirectoryInfo(context.TestDirectory).Root.FullName);
            _ReplacePrefix(ref currPath, "{AssemblyDirectory}", _GetAssemblyDirectory());
            _ReplacePrefix(ref currPath, "{ProcessDirectory}", _GetProcessDirectory());

            // output paths
            _ReplacePrefix(ref currPath, "{WorkDirectory}", context.WorkDirectory);
            _ReplacePrefix(ref currPath, "{WorkDirectoryRoot}", new System.IO.DirectoryInfo(context.WorkDirectory).Root.FullName);
            _ReplacePrefix(ref currPath, "{TempDirectory}", System.IO.Path.GetTempPath());

            //--------------------------------------------------------- relative path macros:

            if (currPath.StartsWith("{")) throw new ArgumentException($"Invalid format: {currPath}", nameof(currPath));

            var casing = StringComparison.Ordinal;

            currPath = currPath.Replace("{Date}", DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture), casing);
            currPath = currPath.Replace("{Time}", DateTime.Now.ToString("hhmmss", System.Globalization.CultureInfo.InvariantCulture), casing);

            currPath = currPath.Replace("{Id}", context.Test.ID, casing);
            currPath = currPath.Replace("{ID}", context.Test.ID, casing);
            currPath = currPath.Replace("{Name}", context.Test.Name, casing);
            currPath = currPath.Replace("{FullName}", context.Test.FullName, casing);
            currPath = currPath.Replace("{ClassName}", context.Test.ClassName, casing);
            currPath = currPath.Replace("{MethodName}", context.Test.MethodName, casing);
            currPath = currPath.Replace("{DisplayName}", context.Test.DisplayName, casing);            

            currPath = currPath.Replace("{CurrentRepeatCount}", context.CurrentRepeatCount.ToString(System.Globalization.CultureInfo.InvariantCulture), casing);
            currPath = currPath.Replace("{WorkerId}", context.WorkerId, casing);

            currPath = currPath.Replace("{Category}", context.GetCurrentCategory(), casing);

            //--------------------------------------------------------- redirections

            if (defaultBasePath != null && !System.IO.Path.IsPathRooted(currPath))
            {
                currPath = System.IO.Path.Combine(defaultBasePath, currPath);
            }

            // resolve dynamic resource link
            // this can be used both for attachments and resources

            if (currPath.EndsWith(".testpath.txt", StringComparison.OrdinalIgnoreCase))
            {                
                if (System.IO.File.Exists(currPath))
                {
                    currPath = System.IO.File.ReadAllText(currPath);
                    return _FormatPath(currPath, context, defaultBasePath);
                }
            }

            // sanitize

            currPath = currPath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            return currPath;
        }

        #endregion

        #region internal        

        private static void _FindUpperFile(TestContext context, ref string format, string macro, Predicate<System.IO.FileInfo> predicate)
        {
            var dir = _FindUpperDirectoryWithFile(context.TestDirectory, predicate);
            if (dir == null) return;
            
            _ReplacePrefix(ref format, macro, dir);
        }

        private static void _FindUpperDirectory(TestContext context, ref string format, string macro, Predicate<System.IO.DirectoryInfo> predicate)
        {
            var dir = _FindUpperDirectoryWithDirectory(context.TestDirectory, predicate);
            if (dir == null) return;

            _ReplacePrefix(ref format, macro, dir);
        }

        private static void _ReplacePrefix(ref string format, string macro, string value)
        {
            var idx = format.IndexOf(macro);
            if (idx < 0) return;

            if (idx > 0) throw new ArgumentException($"{macro} must appear at the beginning of {format}", nameof(format));

            value = value.TrimEnd(System.IO.Path.DirectorySeparatorChar);
            value = value.TrimEnd(System.IO.Path.AltDirectorySeparatorChar);

            format = format.Substring(macro.Length);
            format = value + format;
        }

        private static string _GetAssemblyDirectory()
        {
            // in Net471 unit testing, GetEntryAssembly returns null.

            var assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetExecutingAssembly();
            
            var location = assembly?.Location;
            if (location == null) return null;

            return System.IO.Path.GetDirectoryName(location);
        }

        private static string _GetProcessDirectory()
        {
            #if NET6_0_OR_GREATER
            var path = Environment.ProcessPath;
            #else
            var path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            #endif
            return System.IO.Path.GetDirectoryName(path);
        }

        private static string GetCurrentCategory(this TestContext context)
        {
            return context.Test.Properties.ContainsKey("Category")
                ? (string)context.Test.Properties.Get("Category")
                : "Default";
        }

        private static string _FindUpperDirectoryWithFile(string fromDirectoryPath, Predicate<System.IO.FileInfo> predicate)
        {
            var dir = new System.IO.DirectoryInfo(fromDirectoryPath);
            if (!dir.Exists) return null;

            while(dir != null)
            {
                if (dir.EnumerateFiles().Any(f=> predicate(f))) return dir.FullName;

                dir = dir.Parent;
            }

            return null;
        }

        private static string _FindUpperDirectoryWithDirectory(string fromDirectoryPath, Predicate<System.IO.DirectoryInfo> predicate)
        {
            var dir = new System.IO.DirectoryInfo(fromDirectoryPath);
            if (!dir.Exists) return null;

            while (dir != null)
            {
                if (dir.EnumerateDirectories().Any(f => predicate(f))) return dir.FullName;

                dir = dir.Parent;
            }

            return null;
        }

        private static void _GuardIsValidRelativePath(this string format)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (System.IO.Path.IsPathRooted(format)) throw new ArgumentException("Must be a relative path.", nameof(format));


            if (format.Contains(".." + System.IO.Path.DirectorySeparatorChar)) throw new ArgumentException("Must not redirect to outside directories.", nameof(format));
            if (format.Contains(".." + System.IO.Path.AltDirectorySeparatorChar)) throw new ArgumentException("Must not redirect to outside directories.", nameof(format));

            var invalidChars = System.IO.Path.GetInvalidPathChars();

            if (format.Any(invalidChars.Contains)) throw new ArgumentException("Must not contain invalid chars", nameof(format));
        }

        #endregion
    }
}
