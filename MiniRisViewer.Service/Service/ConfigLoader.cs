using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MiniRisViewer.Domain.Service
{
    public class ConfigLoader
    {
        /// <summary>
        ///
        /// </summary>
        private static readonly string ConfigPath = @"C:\ProgramData\UsTEC\MiniRisViewer\LogFolderPath.xml";

        /// <summary>
        ///
        /// </summary>
        private static readonly string BaseKey = @"SYSTEM\CurrentControlSet\Services\";

        /// <summary>
        ///
        /// </summary>
        public class LogFolderPath
        {
            public string ImporterLogPath;
            public string ResponderLogPath;
            public string ScpCoreLogPath;
            public string AscLogPath;
            public string MppsLogPath;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigLoader()
        {
        }

        /// <summary>
        /// 設定ファイルを読込、
        /// ログファイルのパスを設定する
        /// </summary>
        public static LogFolderPath LoadConfigFiles()
        {
            //var importerRegKey = CombineRegistryPath("UsFileImporter");
            //var responderRegKey = CombineRegistryPath("UsMwmResponder");
            //var scpCoreRegKey = CombineRegistryPath("UsScpCore");
            //var ascRegKey = CombineRegistryPath("UsAscService");
            //var mppsRegKey = CombineRegistryPath("UsMppsReceiver");

            XElement xDocument = XDocument.Load(ConfigPath).Element("LogPath");

            var instance = new LogFolderPath()
            {
                ImporterLogPath = xDocument.Element("FileImporterLogPath").Value,
                ResponderLogPath = xDocument.Element("MwmResponderLogPath").Value,
                ScpCoreLogPath = xDocument.Element("ScpCoreLogPath").Value,
                AscLogPath = xDocument.Element("AscLogPath").Value,
                MppsLogPath = xDocument.Element("MppsReceiverLogPath").Value,
            };
            return instance;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="regPath"></param>
        /// <returns></returns>
        private static string GetLogPathForLog4(string regPath)
        {
            var fileDirectory = GetFileDirectory(ReadRegistry(regPath));
            var xElement = XDocument.Load(Path.Combine(fileDirectory, "log4net.config"));
            var tmp = xElement.XPathSelectElement("//appender");
            return tmp.Element("file").Value;
        }

        /// <summary>
        /// 与えられたパスからディレクトリ名を取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetFileDirectory(string path)
            => Path.GetDirectoryName(path);

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string ReadRegistry(string serviceName)
        {
            using (var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(CombineRegistryPath(serviceName)))
            {
                return (string)regKey.GetValue("ImagePath");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static string CombineRegistryPath(string serviceName)
            => CombinePath(BaseKey, serviceName);

        /// <summary>
        /// 2つのパスを連結する
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        private static string CombinePath(string str1, string str2)
            => Path.Combine(str1, str2);
    }
}