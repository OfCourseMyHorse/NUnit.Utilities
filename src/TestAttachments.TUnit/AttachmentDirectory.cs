using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TUnit.Core.Logging;

namespace TUnit
{
    /// <summary>
    /// Creates a directory context that tracks all the changes that happen within it, so any
    /// file created inside the directory is automatically added as an attachment.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Directory.FullName")]
    public class AttachmentDirectory : IDisposable
    {
        #region lifecycle

        public AttachmentDirectory()
            : this(string.Empty) { }

        public AttachmentDirectory(string relativePath)
        {
            var DIRINFO = TestContext.Current.GetAttachmentDirectoryInfo();

            if (!string.IsNullOrWhiteSpace(relativePath)) DIRINFO = DIRINFO.CreateSubdirectory(relativePath);

            DIRINFO.Create();

            _SetWatcher(DIRINFO.FullName);
        }

        public AttachmentDirectory(DIRINFO dir)
        {
            dir.Create();

            _SetWatcher(dir.FullName);
        }

        public void Dispose()
        {            
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AttachmentDirectory() { Dispose(false); }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AttachAllFiles();

                System.Threading.Interlocked.Exchange(ref _Watcher, null)?.Dispose();

                var dir = System.Threading.Interlocked.Exchange(ref _PreviousWorkingDirectory, null);
                if (!string.IsNullOrEmpty(dir)) Environment.CurrentDirectory = dir;
            }
        }

        private void _SetWatcher(string directory, bool includeSubDir = false)
        {
            // http://www.techrepublic.com/article/use-the-net-filesystemwatcher-object-to-monitor-directory-changes-in-c/
            // Standard pattern when using Watcher: https://www.intertech.com/Blog/avoiding-file-concurrency-using-system-io-filesystemwatcher/
            _Watcher = new System.IO.FileSystemWatcher(directory);
            _Watcher.NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.Attributes | System.IO.NotifyFilters.Size;
            _Watcher.IncludeSubdirectories = includeSubDir;
            _Watcher.EnableRaisingEvents = true;

            _Watcher.Created += _OnUpdate;
            _Watcher.Deleted += _OnUpdate;
            _Watcher.Renamed += _OnUpdate;
            _Watcher.Changed += _OnUpdate;
        }

        private void _OnUpdate(object sender, System.IO.FileSystemEventArgs e)
        {
            var FILEINFO = new FILEINFO(e.FullPath);
            if (FILEINFO.Exists)
            {
                TestContext.Current.GetDefaultLogger().LogInformation($"Attach {FILEINFO.Name}");

                _Files.TryAdd(FILEINFO.FullName, null);
            }
            else
            {
                TestContext.Current.GetDefaultLogger().LogInformation($"Detach {FILEINFO.Name}");

                _Files.TryRemove(FILEINFO.FullName, out _);
            }
        }

        #endregion

        #region data        

        #pragma warning disable CA2213 // Disposable fields should be disposed
        private System.IO.FileSystemWatcher _Watcher;
        #pragma warning restore CA2213 // Disposable fields should be disposed

        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _Files = new System.Collections.Concurrent.ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private string _PreviousWorkingDirectory;

        #endregion

        #region properties

        public DIRINFO Directory
        {
            get
            {
                if (_Watcher == null) throw new ObjectDisposedException(nameof(AttachmentDirectory));

                return new DIRINFO(_Watcher.Path);
            }
        }

        public IReadOnlyList<FILEINFO> GetUpdatedFiles()
        {
            if (_Watcher == null) throw new ObjectDisposedException(nameof(AttachmentDirectory));

            _Watcher.EnableRaisingEvents = false;

            var files = _Files
                .Keys
                .Select(item => new FILEINFO(item))
                .Where(item => item.Exists)
                .ToList();

            _Watcher.EnableRaisingEvents = true;

            return files;
        }

        #endregion

        #region API        

        public FILEINFO GetFileInfo(string filePath)
        {
            ObjectDisposedException.ThrowIf(_Watcher == null, typeof(System.IO.FileSystemWatcher));

            ArgumentNullException.ThrowIfNull(filePath);

            filePath = System.IO.Path.Combine(_Watcher.Path, filePath);

            return new FILEINFO(filePath);
        }

        public void AttachAllFiles()
        {
            if (_Watcher != null) 
            {
                // wait for the watcher to catch up with
                // latest changes in the file system
                System.Threading.Thread.Sleep(1000);
            }            

            foreach (var key in _Files.Keys.ToList())
            {
                if (_Files.TryRemove(key, out _))
                {
                    TestContext.Current.Output.AttachArtifact(key);
                }
            }
        }        

        #endregion
    }
}
