using System;
using System.Collections.Generic;
using System.Text;



namespace TestAttachments
{
    internal static class _PrivateExtensions
    {
        public static bool IsRepositoryDatabase(this DIRINFO DIRINFO)
        {
            if (DIRINFO.Name == ".svn") return true;
            if (DIRINFO.Name == ".git") return true;
            return false;
        }

        public static Uri ToUri(this DIRINFO DIRINFO)
        {
            return DIRINFO == null
                ? null
                : new Uri(DIRINFO.FullName, UriKind.Absolute);
        }

        public static DIRINFO ToDirectoryInfo(this string directoryPath, bool resolveLink = false)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return null;

            var result = new DIRINFO(directoryPath);

            return resolveLink && result.TryResolveShortcutDir(out var altDir)
                ? altDir
                : result;
        }
    }
}
