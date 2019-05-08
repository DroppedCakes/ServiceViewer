using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MiniRisViewer.Domain
{
    public class DomainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //App.xaml.csにてDI登録
            containerRegistry.RegisterInstance<DialogService>(new DialogService());
        }
    }
}