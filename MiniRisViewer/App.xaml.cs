using MiniRisViewer.ServiceStatus;
using MiniRisViewer.Views;
using NLog;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows;

namespace MiniRisViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        ///
        /// </summary>
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _logger.Info("OnInitialized実行");

            try
            {
                var region_manager = CommonServiceLocator.ServiceLocator.Current.GetInstance<IRegionManager>();

                _logger.Info("region_manager");

                region_manager.RequestNavigate("ContentRegion", nameof(ServiceStatus));

                _logger.Info("ContentRegion");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Fatal, ex, "例外発生！！");
                // メッセージボックスだして、アプリを終了させる
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ServiceStatusModule>();
        }
    }
}