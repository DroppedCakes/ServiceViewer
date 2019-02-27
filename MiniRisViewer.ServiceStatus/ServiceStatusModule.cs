using MiniRisViewer.ServiceStatus.ViewModels;
using MiniRisViewer.ServiceStatus.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MiniRisViewer.ServiceStatus
{
    public class ServiceStatusModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ServiceStatusViewModel>();
            containerRegistry.RegisterForNavigation<Views.ServiceStatus>();
        }
    }
}