using System;
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

            return _FormatPath(format, context, context.TestDirectory);
        }

        private static string _FormatPath(string format, TestContext context, string defaultBasePath)
        {
            // TODO:
            // - support {<<<} and {>>>} as macro for paths search
            // - support "./xyz" to concatenate with upper level formatting

            //--------------------------------------------------------- absolute path macros (format can only have one of those):            

            _FindUpperFile(context, ref format, "{SolutionDirectory}", finfo => finfo.Extension.ToLower().EndsWith("sln"));
            _FindUpperFile(context, ref format, "{ProjectDirectory}", finfo => finfo.Extension.ToLower().EndsWith("csproj"));

            if (format.Length > 4 && format.StartsWith("{\"")) // handle "{\"somefilename.ext\"}\whatever\whatever"
            {                
                var idx = format.IndexOf("\"}");
                if (idx >= 0)
                {
                    var key = format.Substring(0, idx + 2);
                    var val = key.Substring(2, key.Length - 4).ToLower();
                    _FindUpperFile(context, ref format, key, finfo => finfo.Name.ToLower() == val); // TODO: this is NOT cross platform
                }
            }

            _ReplacePrefix(ref format, "{CurrentDirectory}", Environment.CurrentDirectory);

            // input paths
            _ReplacePrefix(ref format, "{TestDirectory}", context.TestDirectory);
            _ReplacePrefix(ref format, "{AssemblyDirectory}", _GetAssemblyDirectory());
            _ReplacePrefix(ref format, "{ProcessDirectory}", _GetProcessDirectory());

            // output paths
            _ReplacePrefix(ref format, "{WorkDirectory}", context.WorkDirectory);
            _ReplacePrefix(ref format, "{TempDirectory}", System.IO.Path.GetTempPath());

            //--------------------------------------------------------- relative path macros:

            if (format.StartsWith("{")) throw new ArgumentException($"Invalid format: {format}", nameof(format));

            var casing = StringComparison.Ordinal;

            format = format.Replace("{Date}", DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture), casing);
            format = format.Replace("{Time}", DateTime.Now.ToString("hhmmss", System.Globalization.CultureInfo.InvariantCulture), casing);

            format = format.Replace("{Id}", context.Test.ID, casing);
            format = format.Replace("{ID}", context.Test.ID, casing);
            format = format.Replace("{Name}", context.Test.Name, casing);
            format = format.Replace("{FullName}", context.Test.FullName, casing);
            format = format.Replace("{ClassName}", context.Test.ClassName, casing);
            format = format.Replace("{MethodName}", context.Test.MethodName, casing);

            format = format.Replace("{CurrentRepeatCount}", context.CurrentRepeatCount.ToString(), casing);
            format = format.Replace("{WorkerId}", context.WorkerId, casing);

            format = format.Replace("{Category}", context.GetCurrentCategory(), casing);

            //--------------------------------------------------------- path normalization:            

            if (defaultBasePath != null && !System.IO.Path.IsPathRooted(format))
            {
                format = System.IO.Path.Combine(defaultBasePath, format);
            }

            format = format.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            return format;
        }

        #endregion

        #region internal        

        private static void _FindUpperFile(TestContext context, ref string format, string macro, Predicate<System.IO.FileInfo> predicate)
        {
            var dir = _FindUpperDirectoryWithFile(context.TestDirectory, predicate);
            if (dir == null) return;
            
            _ReplacePrefix(ref format, macro, dir);
        }

        private static void _ReplacePrefix(ref string format, string macro, string value)
        {
            var idx = format.IndexOf(macro);
            if (idx < 0) return;

            if (idx > 0) throw new ArgumentException($"{macro} must appear at the beginning of {format}", nameof(format));

            value = value.TrimEnd('\\');
            value = value.TrimEnd('/');

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
