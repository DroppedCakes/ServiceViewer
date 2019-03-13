using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MiniRisViewer.Domain.Service
{
    public class ConfigLoader
    {
        /// <summary>
        ///
        /// </summary>
        private static readonly string BaseKey = @"SYSTEM\CurrentControlSet\Services\";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigLoader()
        {
        }

        /// <summary>
        /// 設定ファイルからデシリアライズを行う
        /// </summary>
        /// <param name="configPath">設定ファイルのフルパス</param>
        /// <returns>デシリアライズしたServices</returns>
        public static Config LoadConfigFromFile(string configPath)
        {
            return Deserialize<Config>(configPath);
        }

        // ファイルに書き出すときに使う
        private static void Serialize<T>(string savePath, T graph)
        {
            using (var sw = new StreamWriter(savePath, false, Encoding.UTF8))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                new XmlSerializer(typeof(T)).Serialize(sw, graph, ns);
            }
        }

        // ファイルを読み取るときに使う
        private static T Deserialize<T>(string loadPath)
        {
            using (var sr = new StreamReader(loadPath))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(sr);
            }
        }

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