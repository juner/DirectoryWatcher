using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryWatcher
{
    public class DirectoryWatcher : IDisposable
    {
        public string Path => Watcher?.Path;
        public string Filter => Watcher?.Filter;
        public System.IO.NotifyFilters NotifyFilters => Watcher?.NotifyFilter ?? default;
        public System.ComponentModel.ISynchronizeInvoke SynchronizingObject => Watcher?.SynchronizingObject ?? default;
        System.IO.FileSystemWatcher Watcher = null;
        public DirectoryWatcher(Action<object, System.IO.FileSystemEventArgs> Changed = default, Action<object, System.IO.RenamedEventArgs> Renamed = default
            , string Path = default, string Filter = default, System.IO.NotifyFilters NotifyFilters = default, System.ComponentModel.ISynchronizeInvoke SynchronizingObject = default) 
            : this(Changed, Changed, Changed, Renamed, Path, Filter, NotifyFilters, SynchronizingObject){ }

        public DirectoryWatcher(Action<object, System.IO.FileSystemEventArgs> Changed = default, Action<object, System.IO.FileSystemEventArgs> Created = default, Action<object, System.IO.FileSystemEventArgs> Deleted = default, Action<object, System.IO.RenamedEventArgs> Renamed = default
            , string Path = default, string Filter = default, System.IO.NotifyFilters NotifyFilters = default, System.ComponentModel.ISynchronizeInvoke SynchronizingObject = default)
        {
            Watcher = new System.IO.FileSystemWatcher()
            {
                Path = Path,
                Filter = Filter,
                NotifyFilter = NotifyFilters,
                SynchronizingObject = SynchronizingObject,
            };
            if (Changed != null)
                Watcher.Changed += new System.IO.FileSystemEventHandler(Changed);
            if (Created != null)
                Watcher.Created += new System.IO.FileSystemEventHandler(Created);
            if (Deleted != null)
                Watcher.Deleted += new System.IO.FileSystemEventHandler(Deleted);
            if (Renamed != null)
                Watcher.Renamed += new System.IO.RenamedEventHandler(Renamed);
            Watcher.EnableRaisingEvents = true;
        }
        public static Task StartAsync(CancellationToken Token, Action<object, System.IO.FileSystemEventArgs> Changed = default, Action<object, System.IO.RenamedEventArgs> Renamed = default
            , string Path = default, string Filter = default, System.IO.NotifyFilters NotifyFilters = default, System.ComponentModel.ISynchronizeInvoke SynchronizingObject = default)
            => StartAsync(Token, Changed, Changed, Changed, Renamed, Path, Filter, NotifyFilters, SynchronizingObject);
        public static async Task StartAsync(CancellationToken Token, Action<object, System.IO.FileSystemEventArgs> Changed = default, Action<object, System.IO.FileSystemEventArgs> Created = default, Action<object, System.IO.FileSystemEventArgs> Deleted = default, Action<object, System.IO.RenamedEventArgs> Renamed = default
            , string Path = default, string Filter = default, System.IO.NotifyFilters NotifyFilters = default, System.ComponentModel.ISynchronizeInvoke SynchronizingObject = default)
        {
            try
            {
                using (new DirectoryWatcher(Changed, Created, Deleted, Renamed, Path, Filter, NotifyFilters, SynchronizingObject))
                    await Task.Delay(Timeout.Infinite, Token);
            }
            catch (ObjectDisposedException) { }
            catch (TaskCanceledException) { }
        }
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if( Watcher is System.IO.FileSystemWatcher _Watcher)
                    {
                        _Watcher.EnableRaisingEvents = false;
                        _Watcher.Dispose();
                        Watcher = null;
                    }
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~DirectoryWatcher() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
