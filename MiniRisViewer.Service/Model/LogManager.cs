﻿namespace MiniRisViewer.Domain.Model
{
    public class LogManager
    {
        private string ImporterLogPath;

        private string ResponderLogPath;

        private string AscLogPath;

        private string ScpCoreLogPath;

        private string MppsLogPath;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogManager()
        {
            ImporterLogPath = @"C:\WriteEnable\Logs\UsFileImporter";
            ResponderLogPath = @"C:\WriteEnable";
            AscLogPath = @"C:\WriteEnable";
            ScpCoreLogPath = @"C:\WriteEnable";
            MppsLogPath = @"C:WriteEnable";
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