using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

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
            var properties = PathFormatAttribute._FindProperties<AttachmentPathFormatAttribute>(context, "AttachShowDirectoryLink");

            return properties != null && properties.Get("AttachShowDirectoryLink") is bool value && value;
        }

        public static string FindAttachmentPathFormat(this TestContext context)
        {
            var properties = PathFormatAttribute._FindProperties<AttachmentPathFormatAttribute>(context, "AttachmentPathFormat");

            return properties == null
                ? DefaultAttachmentFormat
                : properties.Get("AttachmentPathFormat") as string;
        }

        public static string FindResourcesPathFormat(this TestContext context)
        {
            var properties = PathFormatAttribute._FindProperties<ResourcePathFormatAttribute>(context, "ResourcePathFormat");

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
            format = format.Replace("*", DefaultAttachmentFormat);
            format = format.Replace("?", "{ID}");

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
            
            format = format.Replace("*", DefaultResourceFormat);

            return _FormatPath(format, context, context.TestDirectory);
        }

        private static string _FormatPath(string format, TestContext context, string defaultBasePath)
        {
            // // TODO: support {<<<} and {>>>} as macro for paths search

            //--------------------------------------------------------- absolute path macros (format can only have one of those):

            var slnDir = _GetFirstDirectoryWith(context.TestDirectory, finfo => finfo.Extension.ToLower().EndsWith("sln"));
            if (slnDir != null)
            {
                const string SLNDIRMACRO = "{SolutionDirectory}";

                var idx = format.IndexOf(SLNDIRMACRO);
                if (idx >= 0)
                {
                    format = format.Substring(idx + SLNDIRMACRO.Length);
                    format = slnDir + format;
                }
            }
            
            format = format.Replace("{CurrentDirectory}", Environment.CurrentDirectory);

            // input paths
            format = format.Replace("{TestDirectory}", context.TestDirectory);
            format = format.Replace("{AssemblyDirectory}", _GetAssemblyDirectory());

            // output paths
            format = format.Replace("{WorkDirectory}", context.WorkDirectory);
            format = format.Replace("{TempDirectory}", System.IO.Path.GetTempPath());


            //--------------------------------------------------------- relative path macros:

            format = format.Replace("{Date}", DateTime.Now.ToString("yyyyMMdd"));
            format = format.Replace("{Time}", DateTime.Now.ToString("hhmmss"));

            format = format.Replace("{Id}", context.Test.ID);
            format = format.Replace("{ID}", context.Test.ID);
            format = format.Replace("{Name}", context.Test.Name);
            format = format.Replace("{FullName}", context.Test.FullName);
            format = format.Replace("{ClassName}", context.Test.ClassName);
            format = format.Replace("{MethodName}", context.Test.MethodName);

            format = format.Replace("{CurrentRepeatCount}", context.CurrentRepeatCount.ToString());
            format = format.Replace("{WorkerId}", context.WorkerId);

            format = format.Replace("{Category}", context.GetCurrentCategory());


            //--------------------------------------------------------- path sanitization:

            format = format.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            if (defaultBasePath != null && !System.IO.Path.IsPathRooted(format))
            {
                format = System.IO.Path.Combine(defaultBasePath, format);
            }            

            return format;
        }

        #endregion

        #region internal

        private static string _GetAssemblyDirectory()
        {
            // in Net471 unit testing, GetEntryAssembly returns null.

            var assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetExecutingAssembly();
            
            var location = assembly?.Location;
            if (location == null) return null;

            return System.IO.Path.GetDirectoryName(location);
        }

        private static string GetCurrentCategory(this TestContext context)
        {
            return context.Test.Properties.ContainsKey("Category")
                ? (string)context.Test.Properties.Get("Category")
                : "Default";
        }

        private static string _GetFirstDirectoryWith(string fromDirectoryPath, Predicate<System.IO.FileInfo> predicate)
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
            if (format.Contains("..\\")) throw new ArgumentException("Must not redirect to outside directories.", nameof(format));
            if (format.Contains("..//")) throw new ArgumentException("Must not redirect to outside directories.", nameof(format));

            var invalidChars = System.IO.Path.GetInvalidPathChars();

            if (format.Any(invalidChars.Contains)) throw new ArgumentException("Must not contain invalid chars", nameof(format));
        }

        #endregion
    }
}
