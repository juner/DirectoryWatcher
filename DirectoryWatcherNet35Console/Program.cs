using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryWatcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var listener = new TextWriterTraceListener(Console.Out);
            Trace.Listeners.Add(listener);
            try
            {
                var Parser = new CommandParser();
                Parser.Parse(args);
                using (var TokenSource = new CancellationTokenSource())
                using (SetConsoleCtrl.Create(type =>
                {
                    TokenSource?.Cancel();
                    return false;
                }))
                {
                    Console.WriteLine($"Path: {Parser.Path}");
                    Console.WriteLine($"Filter: {Parser.Filter}");
                    Console.WriteLine("watcher start.");
                    await DirectoryWatcher.StartAsync(TokenSource.Token
                        , Changed: (sender, e) => Console.WriteLine($"{e.ChangeType} {e.FullPath}")
                        , Renamed: (sender, e) => Console.WriteLine($"{e.ChangeType} {e.OldFullPath} -> {e.FullPath}")
                        , Path: Parser.Path, Filter: Parser.Filter, NotifyFilters: Parser.NotifyFilters
                        , SynchronizingObject: default);
                    Console.WriteLine("watcher stop.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    public class SetConsoleCtrl : IDisposable
    {
        public static SetConsoleCtrl Create(Func<CtrlTypes, bool> ConsoleCtrl) => new SetConsoleCtrl(ConsoleCtrl);
        readonly NativeMethods.HandlerRoutine ConsoleCtrl;
        protected SetConsoleCtrl(Func<CtrlTypes, bool> ConsoleCtrl)
        {
            this.ConsoleCtrl = new NativeMethods.HandlerRoutine(ConsoleCtrl);
            NativeMethods.SetConsoleCtrlHandler(this.ConsoleCtrl, true);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var result = NativeMethods.SetConsoleCtrlHandler(ConsoleCtrl, false);
                }
                disposedValue = true;
            }
        }
        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose() => Dispose(true);
        #endregion
    }
    static class NativeMethods
    {
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        /// <summary>
        /// コンソールのコントロールが呼ばれた時のハンドラ
        /// </summary>
        /// <param name="Handler"></param>
        /// <param name="Add"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
    }
    /// <summary>
    /// 終了を試みたタイプ
    /// </summary>
    public enum CtrlTypes
    {
        /// <summary>
        /// Ctrl+Cが押された
        /// </summary>
        CTRL_C_EVENT = 0,
        /// <summary>
        /// ブレイクイベントが起こった
        /// </summary>
        CTRL_BREAK_EVENT = 1,
        /// <summary>
        /// 閉じようとした
        /// </summary>
        CTRL_CLOSE_EVENT = 2,
        /// <summary>
        /// ログオフしようとした
        /// </summary>
        CTRL_LOGOFF_EVENT = 5,
        /// <summary>
        /// シャットダウンを試みられた
        /// </summary>
        CTRL_SHUTDOWN_EVENT = 6,
    }
}
