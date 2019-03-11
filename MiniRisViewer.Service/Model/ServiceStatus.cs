using MiniRisViewer.Domain.Service;
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
        public ServiceManager[] Services;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatus()
        {
            var config = ConfigLoader.LoadDisplayConfig();

            Services = new ServiceManager[]{
            new ServiceManager(ServiceList.UsFileImporterServiceName,config.FileImporterName),
            new ServiceManager(ServiceList.UsMwmResponderServiceName,config.MwmResponderName),
            new ServiceManager(ServiceList.UsScpCoreServiceName,config.ScpCoreName),
            new ServiceManager(ServiceList.UsAscServiceName,config.AscName),
            new ServiceManager(ServiceList.UsMppsReceiverServiceName,config.MppsReceiverName),
        };
        }

        /// <summary>
        /// 各サービスの状態を更新
        /// </summary>
        public void ServiceStatusUpdate()
        {
            foreach (ServiceManager x in Services)
            {
                x.GetServiceState();
            }
        }

        ///<summary>
        ///全サービスの再起動を行う
        /// </summary>
        public void RestartService()
        {
            //再起動できたかどうかをListで持っておく（現状戻り値なしなので使い道なし）
            var result = new List<bool>();

            foreach (ServiceManager x in Services)
            {
                result.Add(x.Restart());
            }
        }
    }
}