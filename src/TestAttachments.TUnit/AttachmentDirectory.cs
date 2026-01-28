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
            : this(string.Empty)
        { }

        public AttachmentDirectory(string relativePath)
        {
            var dinfo = TestContext.Current.GetAttachmentDirectoryInfo();

            if (!string.IsNullOrWhiteSpace(relativePath)) dinfo = dinfo.CreateSubdirectory(relativePath);

            dinfo.Create();

            _SetWatcher(dinfo.FullName);
        }

        public AttachmentDirectory(System.IO.DirectoryInfo dir)
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
            var finfo = new System.IO.FileInfo(e.FullPath);
            if (finfo.Exists)
            {
                TestContext.Current.GetDefaultLogger().LogInformation($"Attach {finfo.Name}");

                _Files.TryAdd(finfo.FullName, null);
            }
            else
            {
                TestContext.Current.GetDefaultLogger().LogInformation($"Detach {finfo.Name}");

                _Files.TryRemove(finfo.FullName, out _);
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

        public DINFO Directory
        {
            get
            {
                if (_Watcher == null) throw new ObjectDisposedException(nameof(AttachmentDirectory));

                return new System.IO.DirectoryInfo(_Watcher.Path);
            }
        }

        public IReadOnlyList<System.IO.FileInfo> UpdatedFiles
        {
            get
            {
                if (_Watcher == null) throw new ObjectDisposedException(nameof(AttachmentDirectory));

                _Watcher.EnableRaisingEvents = false;

                var files = _Files
                    .Keys
                    .Select(item => new System.IO.FileInfo(item))
                    .Where(item => item.Exists)
                    .ToList();

                _Watcher.EnableRaisingEvents = true;

                return files;
            }
        }

        #endregion

        #region API

        /// <summary>
        /// Sets the current directory to the directory pointed by this <see cref="AttachmentDirectory"/>
        /// </summary>
        [Obsolete("This is a dangerous method because multiple tests running concurrently can steal the current directory from each other.")]
        public void SetAsCurrentDirectory()
        {
            if (_Watcher == null) throw new ObjectDisposedException(nameof(AttachmentDirectory));

            _PreviousWorkingDirectory ??= Environment.CurrentDirectory;
            Environment.CurrentDirectory = _Watcher.Path;
        }

        public FINFO GetFileInfo(string filePath)
        {
            if (_Watcher == null) throw new ObjectDisposedException(nameof(AttachmentDirectory));

            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            filePath = System.IO.Path.Combine(_Watcher.Path, filePath);

            return new System.IO.FileInfo(filePath);
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
