using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// Utility class used to create .url shortcuts
    /// </summary>
    static class ShortcutUtils
    {
        // https://stackoverflow.com/questions/18976742/the-url-file-doesnt-load-the-icon
        public static string CreateLink(string localLinkPath, string targetPath)
        {
            if (string.IsNullOrWhiteSpace(localLinkPath)) throw new ArgumentNullException(nameof(localLinkPath));
            if (string.IsNullOrWhiteSpace(targetPath)) throw new ArgumentNullException(nameof(targetPath));

            var content = CreateLinkContent(targetPath);

            localLinkPath = System.IO.Path.ChangeExtension(localLinkPath, ".url");

            System.IO.File.WriteAllText(localLinkPath, content);

            return localLinkPath;
        }

        public static string CreateLinkContent(string targetPath)
        {
            if (!Uri.TryCreate(targetPath, UriKind.Absolute, out Uri uri)) throw new UriFormatException(nameof(targetPath));

            var sb = new StringBuilder();

            sb.AppendLine("[{000214A0-0000-0000-C000-000000000046}]");
            sb.AppendLine("Prop3=19,11");
            sb.AppendLine("[InternetShortcut]");
            sb.AppendLine("IDList=");
            sb.AppendLine($"URL={uri.AbsoluteUri}");

            if (uri.IsFile)
            {
                sb.AppendLine("IconIndex=1");
                string icon = targetPath.Replace('\\', '/');
                sb.AppendLine("IconFile=" + icon);
            }
            else
            {
                sb.AppendLine("IconIndex=0");
            }

            return sb.ToString();
        }

        public static string TryGetSystemPathFromFile(string urlFilePath, bool recursive = false)
        {
            while (true)
            {
                urlFilePath = _TryGetSystemPathFromFile(urlFilePath);
                if (!recursive) return urlFilePath;
                if (!urlFilePath.ToLower().EndsWith(".url")) return urlFilePath;
            }
        }

        private static string _TryGetSystemPathFromFile(string urlFilePath)
        {
            var text = System.IO.File.ReadAllLines(urlFilePath);

            var line = text.FirstOrDefault(item => item.Trim().StartsWith("URL="));

            if (line == null) return null;
            line = line.Trim();
            if (line.Length < 5) return null;

            line = line.Substring(4); // remove "URL="

            line = line.Trim();

            if (Uri.TryCreate(line, UriKind.Absolute, out Uri uri))
            {
                return uri.LocalPath;
            }

            return null;
        }
    }
}
