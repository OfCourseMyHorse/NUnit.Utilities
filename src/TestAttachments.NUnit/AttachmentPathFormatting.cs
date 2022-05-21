using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace NUnit.Framework
{
    static class AttachmentPathFormatting
    {
        #region API

        public static string FindAttachmentPathFormat(this TestContext context)
        {
            var properties = context._FindAttachmentAttributeProperties();
            if (properties == null) return "{WorkDirectory}";

            return properties.Get("AttachmentPathFormat") as string;
        }

        /// <summary>
        /// formats the current path with.
        /// </summary>
        /// <param name="format">The format string taken from <see cref="AttachmentPathFormatAttribute"/></param>
        /// <param name="context">The current test context</param>
        /// <returns></returns>
        public static string FormatPath(this string format, TestContext context)
        {
            // defaults

            if (string.IsNullOrWhiteSpace(format)) format = "{WorkDirectory}";

            if (format == "?") format = "*/?";

            // shortcuts

            format = format.Replace("*", "{WorkDirectory}"); // this is the recommended path for tests: https://github.com/nunit/nunit/issues/1768#issuecomment-242454699
            format = format.Replace("?", "{ID}");

            // absolute path macross:            

            format = format.Replace("{WorkDirectory}", context.WorkDirectory);
            format = format.Replace("{TestDirectory}", context.TestDirectory);
            format = format.Replace("{TempDirectory}", System.IO.Path.GetTempPath());
            format = format.Replace("{CurrentDirectory}", Environment.CurrentDirectory);

            // relative path macross:

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

            format = format.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            return format;
        }

        #endregion

        #region internal

        private static Type _GetCurrentClassInstanceType()
        {
            // this is a hack, but it's the only way to retrieve class level information from the TestContext.
            
            // see https://github.com/nunit/nunit/issues/548

            var testObject = NUnit.Framework.Internal.TestExecutionContext.CurrentContext.TestObject;
            return testObject.GetType();
        }

        private static AttachmentPathFormatAttribute _TryGetAttachmentAttribute(this System.Reflection.ICustomAttributeProvider t)
        {
            return t.GetCustomAttributes(typeof(AttachmentPathFormatAttribute), true)
                .OfType<AttachmentPathFormatAttribute>()
                .FirstOrDefault();
        }

        private static TestContext.PropertyBagAdapter _FindAttachmentAttributeProperties(this TestContext context)
        {
            // look for attribute in current method:

            if (context.Test.Properties.ContainsKey("AttachmentPathFormat")) return context.Test.Properties;
            
            // look for attribute in current class:

            var instanceType = _GetCurrentClassInstanceType();            

            var attribute = instanceType._TryGetAttachmentAttribute();
            if (attribute != null) return new TestContext.PropertyBagAdapter(attribute.Properties);

            // look for attribute in current assembly:

            attribute = instanceType.Assembly._TryGetAttachmentAttribute();
            if (attribute != null) return new TestContext.PropertyBagAdapter(attribute.Properties);

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
