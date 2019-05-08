using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MiniRisViewer.Domain.Service
{
    public class ConfigLoader
    {
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

        /// <summary>
        /// XML Serializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="savePath"></param>
        /// <param name="graph"></param>
        private static void Serialize<T>(string savePath, T graph)
        {
            using (var sw = new StreamWriter(savePath, false, Encoding.UTF8))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                new XmlSerializer(typeof(T)).Serialize(sw, graph, ns);
            }
        }

        /// <summary>
        /// XML Deserializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadPath"></param>
        /// <returns></returns>
        private static T Deserialize<T>(string loadPath)
        {
            using (var sr = new StreamReader(loadPath))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(sr);
            }
        }
    }
}