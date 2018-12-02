namespace DirectoryWatcher
{
    public class CommandParser
    {
        /// <summary>
        /// パラメータを処理する
        /// </summary>
        /// <param name="Arguments"></param>
        public void Parse(string[] Arguments)
        {
            foreach (var param in Arguments)
            {
                if (string.IsNullOrEmpty(param))
                    continue;
                var p = param;
                var IsFlag = false;
                if (param.Length > 1 && (param[0] == '-' || param[0] == '/'))
                {
                    IsFlag = true;
                    p = param.Substring(1);
                }
                ParseParam(p, IsFlag);
            }
        }
        /// <summary>
        /// ヘルプの要求
        /// </summary>
        public string Path { private set; get; } = null;
        /// <summary>
        /// サービスが起動していない場合はサービスを開始する
        /// </summary>
        public string Filter { private set; get; } = null;

        public System.IO.NotifyFilters NotifyFilters = System.IO.NotifyFilters.LastAccess
            | System.IO.NotifyFilters.LastWrite
            | System.IO.NotifyFilters.FileName
            | System.IO.NotifyFilters.DirectoryName;
        public bool RequestedHelp { private set; get; } = false;
        /// <summary>
        /// 次のコマンド処理モード定義
        /// </summary>
        protected enum NextCommandType
        {
            None = 0,
            SetPath,
            SetFilter,
        }
        /// <summary>
        /// 次のコマンド処理モード
        /// </summary>
        protected NextCommandType NextCommand = default;

        public CommandParser()
        {
        }

        /// <summary>
        /// パラメータの分解された値を処理する
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="IsFlag"></param>
        protected void ParseParam(string Param, bool IsFlag)
        {
            if (NextCommand == NextCommandType.None || IsFlag)
            {
                NextCommand = NextCommandType.None;
                if (Param[0] == 'P' || Param[0] == 'p')
                {
                    NextCommand = NextCommandType.SetPath;
                }
                else if (Param[0] == 'F' || Param[0] == 'F')
                {
                    NextCommand = NextCommandType.SetFilter;
                }
                else if (Param[0] == 'H' || Param[1] == 'h')
                {
                    RequestedHelp = true;
                }
            }
            else
            {
                switch (NextCommand)
                {
                    case NextCommandType.SetPath:
                        Path = Param;
                        break;
                    case NextCommandType.SetFilter:
                        Filter = Filter;
                        break;
                }
            }
        }
    }
}
