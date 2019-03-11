using MiniRisViewer.Domain.Service;

namespace MiniRisViewer.Domain.Model
{
    public class LogManager
    {
        private readonly string ImporterLogPath;

        private readonly string ResponderLogPath;

        private readonly string AscLogPath;

        private readonly string ScpCoreLogPath;

        private readonly string MppsLogPath;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogManager()
        {
            var log = ConfigLoader.LoadLogConfig();
            ImporterLogPath = log.ImporterLogPath;
            ResponderLogPath = log.ResponderLogPath;
            AscLogPath = log.AscLogPath;
            ScpCoreLogPath = log.ScpCoreLogPath;
            MppsLogPath = log.MppsLogPath;
        }

        /// <summary>
        /// FileImporterのLogフォルダを表示する
        /// </summary>
        public void ShowImporterLogFolder()
        {
            StartExplorer(ImporterLogPath);
        }

        public void ShowResponderLogFolder()
        {
            StartExplorer(ResponderLogPath);
        }

        public void ShowAscLogFolder()
        {
            StartExplorer(AscLogPath);
        }

        public void ShowScpCoreLogFolder()
        {
            StartExplorer(ScpCoreLogPath);
        }

        public void ShowMppsLogFolder()
        {
            StartExplorer(MppsLogPath);
        }

        /// <summary>
        /// 与えられたパスをエクスプローラで開く
        /// </summary>
        /// <param name="folderPath"></param>
        private void StartExplorer(string folderPath)
        {
            System.Diagnostics.Process.Start(
                "EXPLORER.EXE", folderPath);
        }
    }
}