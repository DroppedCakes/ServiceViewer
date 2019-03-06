using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using MiniRisViewer.Domain.Model;
using Reactive.Bindings.Extensions;
using System.ServiceProcess;



namespace MiniRisViewer.ServiceStatus.ViewModels
{
    public class ServiceStatusViewModel : BindableBase, IDisposable
    {
        /// <summary>
        /// Model
        /// </summary>
        public Domain.Model.ServiceStatus model;

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

        public ReactiveCommand<string> ShowLogCommand { get; private set; } = new ReactiveCommand<string>();

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
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private string stringSserviceStatus;

        public ServiceStatusViewModel()
        {
            // 後でDIにする
            model = new Domain.Model.ServiceStatus();

            // M -> VMの接続
            ImporterStatus =model.Services[((int)EpithetOfUs.Importer)].ObserveProperty(x=> x.Status ).ToReactiveProperty();
            AscStatus = model.Services[((int)EpithetOfUs.Asc)].ObserveProperty(x => x.Status).ToReactiveProperty();
            ResponderStatus = model.Services[((int)EpithetOfUs.Responder )].ObserveProperty(x => x.Status).ToReactiveProperty();
            ScpCoreStatus = model.Services[((int)EpithetOfUs.ScpCore )].ObserveProperty(x => x.Status).ToReactiveProperty();
            MppsStatus = model.Services[((int)EpithetOfUs.Mpps)].ObserveProperty(x => x.Status).ToReactiveProperty();


            //Commandの設定
            ImporterStartCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Importer)].Start() );
            AscStartCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Asc)].Start());
            ResponderStartCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Responder )].Start());
            ScpCoreStartCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.ScpCore )].Start());
            MppsStartCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Mpps )].Start());

            ImporterStopCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Importer)].Stop());
            AscStopCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Asc)].Stop());
            ResponderStopCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Responder)].Stop());
            ScpCoreStopCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.ScpCore)].Stop());
            MppsStopCommand.Subscribe(_ => model.Services[((int)EpithetOfUs.Mpps)].Stop());

            //// 全てのサービスを再起動するコマンド
            RestartServiceCommand.Subscribe(_ => model.RestartService());


            // 1秒ごとに購読する
            ScreenSynchronousTimer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            // タイマーを購読し、
            // サービスのステータスを最新のものに同期する
            ScreenSynchronousTimer.Subscribe(_=>model.ServiceStatusUpdate());

            ScreenSynchronousTimer.Start();

        }

    }
}
