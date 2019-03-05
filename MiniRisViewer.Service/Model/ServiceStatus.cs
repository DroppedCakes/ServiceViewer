using Prism.Mvvm;
using System.Collections.Generic;
using System.ServiceProcess;

namespace MiniRisViewer.Domain.Model
{
    public class ServiceStatus : BindableBase
    {

        /// <summary>
        /// 各サービスの状態
        /// </summary>
        public ServiceManager[] Services =        {
            new  ServiceManager( ServiceList.UsFileImporterServiceName),
            new  ServiceManager(ServiceList.UsMwmResponderServiceName),
            new  ServiceManager(ServiceList.UsScpCoreServiceName ),
            new  ServiceManager(ServiceList.UsAscServiceName),
            new  ServiceManager(ServiceList.UsMppsReceiverServiceName )
        };


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatus()
        {
        }


        private ServiceControllerStatus importerStatus;
        public ServiceControllerStatus ImporterStatus
        {
            get { return importerStatus; }
            set { SetProperty(ref importerStatus, value); }
        }

        /// <summary>
        /// 各サービスの状態を更新
        /// </summary>
        public void ServiceStatusUpdate()
        {
            foreach ( ServiceManager  x in Services)
            {
                x.Status = GetServiceState(x.ServiceName);
                ImporterStatus = x.Status;
            }

        }


        /// <summary>
        /// 引数として与えられたサービス名から
        /// サービスの状態を取得する
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private ServiceControllerStatus GetServiceState(string serviceName)
        {
            ServiceControllerStatus retv;
            using (ServiceController sc = new ServiceController(serviceName))
            {
                retv = sc.Status;
            }
            return retv;
        }

        ///<summary>
        ///全サービスの再起動を行う
        /// </summary>
        public void RestartService()
        {
            ////再起動できたかどうかをListで持っておく（現状戻り値なしなので使い道なし）
            //var result = new List<bool>();
  
            //foreach (ServiceManager x in Services)
            //{
            //    result.Add(ServiceManager.Restart());
            //    x.Status = GetServiceState(x.ServiceName);
            //    ImporterStatus = x.Status;
            //}

        }

    }
}