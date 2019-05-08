using MiniRisViewer.Domain;
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
using Unity;
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
        private static DialogService dialogService;

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
                dialogService.ShowMessage("起動できませんでした。\nアプリを終了します。", "Error");
                Environment.Exit(0);

            }
        }

        public Domain.Model.ServiceAdministrator ModelData;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            //DI登録時にエラー判定、ダイアログ表示をしたいので
            //DIコンテナに登録
            dialogService = new DialogService();
            containerRegistry.RegisterSingleton<IDialogService,DialogService >();

            string ConfigPath = @"C:\ProgramData\UsTEC\UsMiniRisViewer\Config.xml";
            Config config;

            //設定ファイルの読込
            try
            {
                config = ConfigLoader.LoadConfigFromFile(ConfigPath);             
               
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error , ex, "Config.xml読み込みエラー。アプリ終了");
                throw;
            }

            //設定ファイルからデータ作成
            try
            {
                ModelData = new ServiceAdministrator(config);

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Config.xml内容エラー。アプリ終了");
                dialogService.ShowMessage("設定ファイルを確認してください。\nアプリを終了します。", "Error");
                Environment.Exit(0);
            }

            containerRegistry.RegisterInstance<ServiceAdministrator>(ModelData);

        }

        public static IDialogService CreateDialogService()
        {

            if (App.dialogService == null) {
                App.dialogService = new DialogService();
            }

            return App.dialogService;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ServiceStatusModule>();

            //App.xaml.cs内でダイアログを使用したいため、上記RegisterTypesにて登録のためコメントアウト
            //moduleCatalog.AddModule<DomainModule>();

        }
    }
}