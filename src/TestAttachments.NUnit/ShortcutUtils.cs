using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Framework
{
    /// <summary>
    /// Utility class used to create .url shortcuts
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/lwef/internet-shortcuts"/>
    /// <see href="https://github.com/tpn/winsdk-10/blob/9b69fd26ac0c7d0b83d378dba01080e93349c2ed/Include/10.0.10240.0/um/ShlObj.h#L266"/>
    /// <see href="https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-shllink/c3376b21-0931-45e4-b2fc-a48ac0e60d15"/>
    /// <see href="https://stackoverflow.com/questions/34602411/internet-shortcut-idlist-value-decoding"/>
    /// 
    /// <see href="https://github.com/securifybv/ShellLink"/> is able to create shell links, but it really doesn't work.
    /// </remarks>
    static class ShortcutUtils
    {
        // https://stackoverflow.com/questions/18976742/the-url-file-doesnt-load-the-icon
        public static string CreateLink(string localLinkPath, string targetPath)
        {
            if (string.IsNullOrWhiteSpace(localLinkPath)) throw new ArgumentNullException(nameof(localLinkPath));
            if (string.IsNullOrWhiteSpace(targetPath)) throw new ArgumentNullException(nameof(targetPath));

            if (localLinkPath.EndsWith(".url", StringComparison.OrdinalIgnoreCase))
            {
                var content = CreateLinkContent(targetPath);

                System.IO.File.WriteAllText(localLinkPath, content);

                return localLinkPath;
            }

            if (localLinkPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                byte[] content = null;

                if (System.IO.Directory.Exists(targetPath))
                {
                    content = new ShellLink.Shortcut()
                    {
                        LinkTargetIDList = new ShellLink.Structures.LinkTargetIDList()
                        {
                            Path = targetPath
                        }
                    }.GetBytes();
                }

                if (System.IO.File.Exists(targetPath))
                {
                    content = ShellLink.Shortcut.CreateShortcut(targetPath).GetBytes();
                }

                if (content == null) throw new ArgumentException("target path must be an existing file or directory", nameof(targetPath));

                System.IO.File.WriteAllBytes(localLinkPath, content);

                return localLinkPath;
            }

            throw new ArgumentException("extension must be .url or .lnk", nameof(localLinkPath));
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
