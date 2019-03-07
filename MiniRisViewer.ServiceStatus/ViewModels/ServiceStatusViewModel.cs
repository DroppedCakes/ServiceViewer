using MiniRisViewer.Domain.Model;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.ServiceProcess;

namespace MiniRisViewer.ServiceStatus.ViewModels
{
    public class ServiceStatusViewModel : BindableBase, IDisposable
    {
        /// <summary>
        /// Model
        /// </summary>
        public Domain.Model.ServiceStatus Model;

        public LogManager LogManager;

        /// <summary>
        ///  サービスの状態を保持するプロパティ
        /// </summary>

        #region ReactiveProperty

        public ReactiveProperty<ServiceControllerStatus> ImporterStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> ResponderStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> AscStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> ScpCoreStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<ServiceControllerStatus> MppsStatus { get; } = new ReactiveProperty<ServiceControllerStatus>();

        #endregion ReactiveProperty

        #region ReactiveCommand

        public ReactiveCommand ImporterStartCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ImporterStopCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ResponderStartCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ResponderStopCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ScpCoreStartCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ScpCoreStopCommand { get; } = new ReactiveCommand();
        public ReactiveCommand AscStartCommand { get; } = new ReactiveCommand();
        public ReactiveCommand AscStopCommand { get; } = new ReactiveCommand();
        public ReactiveCommand MppsStartCommand { get; } = new ReactiveCommand();
        public ReactiveCommand MppsStopCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// 全てのサービスを再起動するコマンド
        /// </summary>
        public ReactiveCommand RestartServiceCommand { get; } = new ReactiveCommand();

        public ReactiveCommand ShowImporterLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowResponderLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowAscLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowScpCoreLogCommand { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowMppsLogCommand { get; private set; } = new ReactiveCommand();

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
        /// コンストラクタ
        /// </summary>
        public ServiceStatusViewModel()
        {
            // 後でDIにする
            Model = new Domain.Model.ServiceStatus();

            LogManager = new LogManager();

            // M -> VMの接続
            ImporterStatus = Model.Services[((int)EpithetOfUs.Importer)].ObserveProperty(x => x.Status).ToReactiveProperty();
            AscStatus = Model.Services[((int)EpithetOfUs.Asc)].ObserveProperty(x => x.Status).ToReactiveProperty();
            ResponderStatus = Model.Services[((int)EpithetOfUs.Responder)].ObserveProperty(x => x.Status).ToReactiveProperty();
            ScpCoreStatus = Model.Services[((int)EpithetOfUs.ScpCore)].ObserveProperty(x => x.Status).ToReactiveProperty();
            MppsStatus = Model.Services[((int)EpithetOfUs.Mpps)].ObserveProperty(x => x.Status).ToReactiveProperty();

            //Commandの設定
            ImporterStartCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Importer)].Start());
            AscStartCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Asc)].Start());
            ResponderStartCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Responder)].Start());
            ScpCoreStartCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.ScpCore)].Start());
            MppsStartCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Mpps)].Start());

            ImporterStopCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Importer)].Stop());
            AscStopCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Asc)].Stop());
            ResponderStopCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Responder)].Stop());
            ScpCoreStopCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.ScpCore)].Stop());
            MppsStopCommand.Subscribe(_ => Model.Services[((int)EpithetOfUs.Mpps)].Stop());

            //// 全てのサービスを再起動するコマンド
            RestartServiceCommand.Subscribe(_ => Model.RestartService());

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