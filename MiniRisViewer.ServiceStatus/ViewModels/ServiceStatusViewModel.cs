using MiniRisViewer.Domain.Model;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace MiniRisViewer.ServiceStatus.ViewModels
{
    public class ServiceStatusViewModel : BindableBase, IDisposable
    {
        public Reactive.Bindings.Notifiers.BooleanNotifier InProgress { get; } = new Reactive.Bindings.Notifiers.BooleanNotifier(false);
        public ReactivePropertySlim<string> ProgressMessage { get; } = new ReactivePropertySlim<string>("");

        /// <summary>
        /// Model
        /// </summary>
        public Domain.Model.ServiceStatus Model;

        public LogManager LogManager;

        #region DisplayName

        private string importerDisplayName;

        public string ImporterDisplayName
        {
            get { return importerDisplayName; }
            set { SetProperty(ref importerDisplayName, value); }
        }

        private string responderDisplayName;

        public string ResponderDisplayName
        {
            get { return responderDisplayName; }
            set { SetProperty(ref responderDisplayName, value); }
        }

        private string ascDiplayName;

        public string AscDiplayName
        {
            get { return ascDiplayName; }
            set { SetProperty(ref ascDiplayName, value); }
        }

        private string scpCoreDisplayName;

        public string ScpCoreDisplayName
        {
            get { return scpCoreDisplayName; }
            set { SetProperty(ref scpCoreDisplayName, value); }
        }

        private string mppsDisplayName;

        public string MppsDisplayName
        {
            get { return mppsDisplayName; }
            set { SetProperty(ref mppsDisplayName, value); }
        }

        #endregion DisplayName

        /// <summary>
        ///  サービスの状態を保持するプロパティ
        /// </summary>

        #region ReactiveProperty

        #region Status

        public ReactiveProperty<ServiceControllerStatus> ImporterStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> ResponderStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> AscStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> ScpCoreStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> MppsStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();

        #endregion Status

        public ReactiveProperty<bool> CanStopImporter { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> CanStopResponder { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> CanStopAsc { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> CanStopScpCore { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> CanStopMpps { get; } = new ReactiveProperty<bool>();

        #endregion ReactiveProperty

        private void Initialize()
        {
        }

        #region ReactiveCommand

        #region Start/Stop

        public ReactiveCommand ImporterStartCommand { get; }
        public ReactiveCommand ImporterStopCommand { get; }
        public ReactiveCommand ResponderStartCommand { get; }
        public ReactiveCommand ResponderStopCommand { get; }
        public ReactiveCommand ScpCoreStartCommand { get; }
        public ReactiveCommand ScpCoreStopCommand { get; }
        public ReactiveCommand AscStartCommand { get; }
        public ReactiveCommand AscStopCommand { get; }
        public ReactiveCommand MppsStartCommand { get; }
        public ReactiveCommand MppsStopCommand { get; }

        #endregion Start/Stop

        /// <summary>
        /// 全てのサービスを再起動するコマンド
        /// </summary>
        public ReactiveCommand RestartServiceCommand { get; } = new ReactiveCommand();

        #region Log

        public ReactiveCommand ShowImporterLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowResponderLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowAscLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowScpCoreLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowMppsLogCommand { get; private set; } = new ReactiveCommand();

        #endregion Log

        #endregion ReactiveCommand

        /// <summary>
        /// サービスの状態を更新するタイマー
        /// </summary>
        public ReactiveTimer ScreenSynchronousTimer;

        /// <summary>
        /// IDisposableの実装部
        /// </summary>
        private CompositeDisposable DisposeCollection = new CompositeDisposable();

        #region IDisposable Support

        private bool disposedValue = false; // 重複する呼び出しを検出するには

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeCollection.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);

        #endregion IDisposable Support

        /// <summary>
        /// 非同期でサービスの再起動を行い、
        /// UIはBusyIndicatorでブロックする
        /// </summary>
        public async void RestartAllServiceAsync()
        {
            InProgress.TurnOn();
            ProgressMessage.Value = "サービス再起動中";
            try
            {
                await Task.Run(() => Model.RestartService());
                //await Task.Run(() => Thread.Sleep(3000));
            }
            finally
            {
                InProgress.TurnOff();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatusViewModel()
        {
            // 後でDIにする
            Model = new Domain.Model.ServiceStatus();

            LogManager = new LogManager();

            // Stop判定のM -> VMの接続
            CanStopImporter = Model.Services[((int)Ailias.Importer)].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            CanStopResponder = Model.Services[((int)Ailias.Responder)].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            CanStopAsc = Model.Services[((int)Ailias.Asc)].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            CanStopScpCore = Model.Services[((int)Ailias.ScpCore)].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            CanStopMpps = Model.Services[((int)Ailias.Mpps)].ObserveProperty(x => x.CanStop).ToReactiveProperty();

            // ステータスのM -> VMの接続
            ImporterStatus = Model.Services[((int)Ailias.Importer)].ObserveProperty(x => x.Status).ToReactiveProperty();
            AscStatus = Model.Services[((int)Ailias.Asc)].ObserveProperty(x => x.Status).ToReactiveProperty();
            ResponderStatus = Model.Services[((int)Ailias.Responder)].ObserveProperty(x => x.Status).ToReactiveProperty();
            ScpCoreStatus = Model.Services[((int)Ailias.ScpCore)].ObserveProperty(x => x.Status).ToReactiveProperty();
            MppsStatus = Model.Services[((int)Ailias.Mpps)].ObserveProperty(x => x.Status).ToReactiveProperty();

            // 画面表示名のM -> VMの接続
            ImporterDisplayName = Model.Services[((int)Ailias.Importer)].DisplayName;
            ResponderDisplayName = Model.Services[((int)Ailias.Responder)].DisplayName;
            AscDiplayName = Model.Services[((int)Ailias.Asc)].DisplayName;
            ScpCoreDisplayName = Model.Services[((int)Ailias.ScpCore)].DisplayName;
            MppsDisplayName = Model.Services[((int)Ailias.Mpps)].DisplayName;

            // 開始・停止ボタンは各サービスの
            // CanStopPropertyによって、活性・非活性する
            ImporterStartCommand = CanStopImporter.Select(x => x == false).ToReactiveCommand();
            ImporterStopCommand = CanStopImporter.Select(x => x == true).ToReactiveCommand();
            ResponderStartCommand = CanStopResponder.Select(x => x == false).ToReactiveCommand();
            ResponderStopCommand = CanStopResponder.Select(x => x == true).ToReactiveCommand();
            AscStartCommand = CanStopAsc.Select(x => x == false).ToReactiveCommand();
            AscStopCommand = CanStopAsc.Select(x => x == true).ToReactiveCommand();
            ScpCoreStartCommand = CanStopScpCore.Select(x => x == false).ToReactiveCommand();
            ScpCoreStopCommand = CanStopScpCore.Select(x => x == true).ToReactiveCommand();
            MppsStartCommand = CanStopMpps.Select(x => x == false).ToReactiveCommand();
            MppsStopCommand = CanStopMpps.Select(x => x == true).ToReactiveCommand();

            // StartCommandの購読
            ImporterStartCommand.Subscribe(_ => Model.Services[((int)Ailias.Importer)].Start());
            AscStartCommand.Subscribe(_ => Model.Services[((int)Ailias.Asc)].Start());
            ResponderStartCommand.Subscribe(_ => Model.Services[((int)Ailias.Responder)].Start());
            ScpCoreStartCommand.Subscribe(_ => Model.Services[((int)Ailias.ScpCore)].Start());
            MppsStartCommand.Subscribe(_ => Model.Services[((int)Ailias.Mpps)].Start());

            // StopCommandの購読
            ImporterStopCommand.Subscribe(_ => Model.Services[((int)Ailias.Importer)].Stop());
            AscStopCommand.Subscribe(_ => Model.Services[((int)Ailias.Asc)].Stop());
            ResponderStopCommand.Subscribe(_ => Model.Services[((int)Ailias.Responder)].Stop());
            ScpCoreStopCommand.Subscribe(_ => Model.Services[((int)Ailias.ScpCore)].Stop());
            MppsStopCommand.Subscribe(_ => Model.Services[((int)Ailias.Mpps)].Stop());

            //// 全てのサービスを再起動するコマンド
            RestartServiceCommand.Subscribe(() => RestartAllServiceAsync());

            // ログ
            ShowImporterLogCommand.Subscribe(_ => LogManager.ShowImporterLogFolder());
            ShowResponderLogCommand.Subscribe(_ => LogManager.ShowResponderLogFolder());
            ShowAscLogCommand.Subscribe(_ => LogManager.ShowAscLogFolder());
            ShowScpCoreLogCommand.Subscribe(_ => LogManager.ShowScpCoreLogFolder());
            ShowMppsLogCommand.Subscribe(_ => LogManager.ShowMppsLogFolder());

            // 1秒ごとに購読する
            ScreenSynchronousTimer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            // タイマーを購読し、
            // サービスのステータスを最新のものに同期する
            ScreenSynchronousTimer.Subscribe(_ => Model.ServiceStatusUpdate());

            ScreenSynchronousTimer.Start();
        }
    }
}