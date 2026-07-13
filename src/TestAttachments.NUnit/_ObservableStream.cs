using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using __STREAM = System.IO.Stream;

namespace TestAttachments
{
    /// <summary>
    /// A <see cref="__STREAM"/> that, when closed, reports back to the host
    /// </summary>
    sealed class _ObservableStream : __STREAM, IServiceProvider
    {
        #region lifecycle

        public static bool TryAddObserver(__STREAM stream, Action onDispose)
        {
            switch (stream)
            {
                case null: return false;

                case _ObservableStream observable:
                    observable.AddObserver(onDispose);
                    return true;

                // this is a special use case when we're consuming the _ObservableStream produced by another library using this generator
                case IServiceProvider srv when stream.GetType().Name == nameof(_ObservableStream):
                    if (srv.GetService(typeof(List<Action>)) is List<Action> actions)
                    {
                        actions.Add(onDispose);
                        return true;
                    }
                    break;
            }

            return false;
        }

        public _ObservableStream(__STREAM strean, Action onDispose)
        {
            _Stream = strean;
            _OnDispose.Add(onDispose);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    var lambdas = System.Threading.Interlocked.Exchange(ref _OnDispose, null);
                    if (lambdas != null)
                    {
                        foreach (var lambda in lambdas) lambda.Invoke();
                    }
                }
                finally
                {
                    var stream = System.Threading.Interlocked.Exchange(ref _Stream, null);
                    stream?.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region data

        private List<Action> _OnDispose = new List<Action>();
        private __STREAM _Stream;

        #endregion

        #region properties

        public override bool CanRead => _BaseStream().CanRead;
        public override bool CanSeek => _BaseStream().CanSeek;
        public override bool CanWrite => _BaseStream().CanWrite;
        public override long Length => _BaseStream().Length;

        public override long Position
        {
            get => _BaseStream().Position;
            set => _BaseStream().Position = value;
        }

        #endregion

        #region API

        public void AddObserver(Action onDispose)
        {
            if (_OnDispose == null) throw new ObjectDisposedException(nameof(_OnDispose));

            _OnDispose.Add(onDispose);
        }

        
        private __STREAM _BaseStream()
        {
            var stream = _Stream;

            return stream == null
                ? throw new ObjectDisposedException(nameof(_Stream))
                : stream;
        }

        public override long Seek(long offset, SeekOrigin origin) { return _BaseStream().Seek(offset, origin); }
        public override void SetLength(long value) { _BaseStream().SetLength(value); }
        
        public override int Read(byte[] buffer, int offset, int count) { return _BaseStream().Read(buffer, offset, count); }
        
        public override void Write(byte[] buffer, int offset, int count) { _BaseStream().Write(buffer, offset, count); }
        public override void Flush() { _BaseStream().Flush(); }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(List<Action>)) return _OnDispose;
            return null;
        }

        #endregion
    }
}
