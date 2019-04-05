using MiniRisViewer.ServiceStatus.ViewModels;
using NLog;
using Prism.Ioc;
using Prism.Modularity;

namespace MiniRisViewer.ServiceStatus
{
    public class ServiceStatusModule : IModule
    {
        /// <summary>
        ///
        /// </summary>
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ServiceStatusViewModel>();
            _logger.Info("containerRegistry.RegisterSingleton<ServiceStatusViewModel>();");

            containerRegistry.RegisterForNavigation<Views.ServiceStatus>();
            _logger.Info("containerRegistry.RegisterForNavigation<Views.ServiceStatus>();");
        }
    }
}