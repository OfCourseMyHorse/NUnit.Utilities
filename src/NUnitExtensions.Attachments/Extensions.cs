using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NUnit.Framework
{
    public static class AttachmentExtensions
    {
        #region internal

        internal static AttachmentPathFormatAttribute TryGetAttachmentAttribute(this System.Reflection.ICustomAttributeProvider t)
        {
            return t.GetCustomAttributes(typeof(AttachmentPathFormatAttribute), true)
                .OfType<AttachmentPathFormatAttribute>()
                .FirstOrDefault();
        }

        internal static TestContext.PropertyBagAdapter _FindAttachmentAttributeProperties(this TestContext context)
        {
            // check test method

            if (context.Test.Properties.ContainsKey("AttachmentPathFormat")) return context.Test.Properties;            

            // this is a hack, but it's the only way to retrieve class level information from the TestContext.
            // see https://github.com/nunit/nunit/issues/548
            var testObject = Internal.TestExecutionContext.CurrentContext.TestObject;
            var instanceType = testObject.GetType();

            // check test class

            var attribute = instanceType.TryGetAttachmentAttribute();
            if (attribute != null) return new TestContext.PropertyBagAdapter(attribute.Properties);

            // check test assembly

            attribute = instanceType.Assembly.TryGetAttachmentAttribute();
            if (attribute != null) return new TestContext.PropertyBagAdapter(attribute.Properties);

            return null;
        }

        internal static string _FindAttachmentPathFormat(this TestContext context)
        {
            var properties = context._FindAttachmentAttributeProperties();
            if (properties == null) return "{WorkDirectory}";

            return properties.Get("AttachmentPathFormat") as string;
        }        

        internal static string FormatPath(this string format, TestContext context)
        {
            format = format.Replace("{WorkDirectory}", context.WorkDirectory);
            format = format.Replace("{TestDirectory}", context.TestDirectory);
            format = format.Replace("{TempDirectory}", System.IO.Path.GetTempPath());
            format = format.Replace("{CurrentDirectory}", Environment.CurrentDirectory);            

            format = format.Replace("{FullName}", context.Test.FullName);
            format = format.Replace("{ClassName}", context.Test.ClassName);
            format = format.Replace("{MethodName}", context.Test.MethodName);
            format = format.Replace("{Name}", context.Test.Name);
            format = format.Replace("{ID}", context.Test.ID);

            format = format.Replace("{CurrentRepeatCount}", context.CurrentRepeatCount.ToString());
            format = format.Replace("{WorkerId}", context.WorkerId);

            format = format.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            return format;
        }

        internal static void _GuardIsValidRelativePath(this string format)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (System.IO.Path.IsPathRooted(format)) throw new ArgumentException("Must be a relative path.", nameof(format));
            if (format.Contains("..\\")) throw new ArgumentException("Must not redirect to outside directories.", nameof(format));
            if (format.Contains("..//")) throw new ArgumentException("Must not redirect to outside directories.", nameof(format));

            var invalidChars = System.IO.Path.GetInvalidPathChars();

            if (format.Any(invalidChars.Contains)) throw new ArgumentException("Must not contain invalid chars", nameof(format));
        }

        #endregion

        #region API

        public static System.IO.FileInfo GetAttachmentPath(this TestContext context, string path)
        {
            var dir = context.GetAttachmentDirectory();
            dir.Create();
            path = System.IO.Path.Combine(dir.FullName, path);
            return new System.IO.FileInfo(path);
        }

        public static System.IO.DirectoryInfo GetAttachmentDirectory(this TestContext context)
        {
            var format = context._FindAttachmentPathFormat();
            var path = format.FormatPath(context);
            var dinfo = new System.IO.DirectoryInfo(path);            

            return dinfo;
        }        

        public static System.IO.FileInfo AttachText(this TestContext context, string fileName, string content, string description = null)
        {
            return context.AttachFile(fileName, finfo => System.IO.File.WriteAllText(finfo.FullName, content), description);
        }

        public static System.IO.FileInfo AttachFile(this TestContext context, string fileName, Action<System.IO.FileInfo> writeAction, string description = null)
        {
            var path = context.GetAttachmentDirectory().FullName;
            path = System.IO.Path.Combine(path, fileName);

            var finfo = new System.IO.FileInfo(path);

            finfo.Directory.Create();

            writeAction(finfo);

            if (finfo.Exists) TestContext.AddTestAttachment(path, description);

            return finfo;
        }

        public static void AttachFolderBrowserShortcut(this TestContext testInstance)
        {
            var tdir = testInstance.GetAttachmentDirectory();
            tdir.Create();

            var path = tdir.FullName;
            var scut = System.IO.Path.Combine(path, "📂 Show Directory.url");

            scut = ShortcutUtils.CreateLink(scut, path);

            TestContext.AddTestAttachment(scut, "Show Directory");
        }

        #endregion
    }
}
