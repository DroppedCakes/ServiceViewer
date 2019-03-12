using MiniRisViewer.Domain.Service;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace MiniRisViewer.Domain.Model
{
    public class ServiceStatus : BindableBase
    {
        /// <summary>
        /// 各サービスの状態・操作を行う
        /// </summary>
        public ServiceManager[] ServiceManagers;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatus(Services services)
        {
            foreach (Service.Service s in services.ServicesList)
            {
                new ServiceManager(s.Name, s.Caption);
            }
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
        public async Task RestartService()
        {
            await Task.WhenAll(
                this.Services.Select(service => Task.Run(() => service.Restart()))
            );
        }
    }
}