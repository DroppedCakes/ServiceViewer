using System.IO;
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
        public static Services LoadConfigFromFile(string configPath)
        {
            return Deserialize(configPath);
        }

        /// <summary>
        /// デシリアライザー
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static Services Deserialize(string configPath)
        {
            var serializer = new XmlSerializer(typeof(Services));
            var deserializedServices = (Services)serializer.Deserialize(new StringReader(configPath));
            return deserializedServices;
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