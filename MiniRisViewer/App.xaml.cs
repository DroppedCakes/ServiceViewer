using MiniRisViewer.ServiceStatus;
using MiniRisViewer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Windows;

namespace MiniRisViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var region_manager = CommonServiceLocator.ServiceLocator.Current.GetInstance<IRegionManager>();
            region_manager.RequestNavigate("ContentRegion", nameof(ServiceStatus));


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