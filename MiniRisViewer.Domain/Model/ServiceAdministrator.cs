using MiniRisViewer.Domain.Service;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniRisViewer.Domain.Model
{
    public class ServiceAdministrator : BindableBase
    {
        /// <summary>
        /// 各サービスの状態・操作を行う
        /// </summary>
        public List<ServiceManager> ServiceManagers = new List<ServiceManager>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceAdministrator(Config config)
        {
            foreach (Service.Service x in config.Services)
            {
                ServiceManagers.Add(new ServiceManager(x.Name, x.Caption, x.LogPath, System.Convert.ToBoolean(x.Visible)));
            }
        }

        /// <summary>
        /// 各サービスの状態を更新
        /// </summary>
        public void ServiceStatusUpdate()
        {
            foreach (ServiceManager x in ServiceManagers)
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
                ServiceManagers.Select(service => Task.Run(() => service.Restart()))
            );
        }
    }
}