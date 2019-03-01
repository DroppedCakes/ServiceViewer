using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MiniRisViewer.Domain.Model
{
    public class ServiceManager
    {
        // サービス名称
        public const string UsFileImporterServiceName = "UsFileImporter";
        public const string UsMwmResponderServiceName = "UsMwmResponder";
        public const string UsAscServiceName = "UsAscService";
        public const string UsScpCoreServiceName = "UsScpCore";
        public const string UsMppsReceiverServiceName = "UsMppsReceiver";

        /// <summary>
        /// サービスの名称
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// 実行ファイルのパス
        /// </summary>
        public string ServicePath { get; }

        /// <summary>
        /// サービスの状態
        /// </summary>
        public ServiceControllerStatus State { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceManager(string serviceName)
        {
            ServiceName = serviceName;
        }
    }
}
