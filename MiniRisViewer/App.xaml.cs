using MiniRisViewer.Domain.Model;
using MiniRisViewer.Domain.Service;
using MiniRisViewer.ServiceStatus;
using MiniRisViewer.Views;
using NLog;
using Prism.Interactivity.InteractionRequest;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Windows;
using Unity.Injection;

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

        ////InteractionRequestクラスのプロパティ
        //public InteractionRequest<Notification> TestNotificationRequest { get; } = new InteractionRequest<Notification>();

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            try
            {
                _logger.Info("起動");
                var region_manager = CommonServiceLocator.ServiceLocator.Current.GetInstance<IRegionManager>();
                region_manager.RequestNavigate("ContentRegion", nameof(ServiceStatus));

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Fatal, ex, "起動時エラー");

            }
        }

        public Domain.Model.ServiceAdministrator ModelData;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {


            string ConfigPath = @"C:\ProgramData\UsTEC\UsMiniRisViewer\Config.xml";

            try
            {
                var config = ConfigLoader.LoadConfigFromFile(ConfigPath);
                ModelData = new ServiceAdministrator(config);
                containerRegistry.RegisterInstance<ServiceAdministrator>(ModelData);


            }
            catch (Exception ex)
            {

                _logger.Log(LogLevel.Error , ex, "Config.xml読み込みエラー。アプリ終了");

                //MVVMを無視して直接表示・・・
                MessageBox.Show("設定ファイルが読み込めなかったため、アプリを終了します。", "Error");
                Environment.Exit(0);

                return;
                
            }
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ServiceStatusModule>();
        }
    }
}