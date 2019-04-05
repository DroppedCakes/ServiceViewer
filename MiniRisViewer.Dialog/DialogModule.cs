using MiniRisViewer.Dialog.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MiniRisViewer.Dialog
{
    public class DialogModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<DialogView>();

        }
    }
}