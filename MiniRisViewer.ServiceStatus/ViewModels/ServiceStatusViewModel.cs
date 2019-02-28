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

namespace MiniRisViewer.ServiceStatus.ViewModels
{
    public class ServiceStatusViewModel : BindableBase,IDisposable
    {
        public Domain.Model.ServiceStatus model;

        public ReactiveProperty<string> ImporterStatus { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ResponderStatus { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> AscStatus{ get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ScpCoreStatus { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> MppsStatus { get; } = new ReactiveProperty<string>();

        public ReactiveCommand RebootOSCommand { get; }
        public ReactiveCommand RestartServiceCommand { get; } = new ReactiveCommand();
        public ReactiveTimer ScreenSynchronousTimer;

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
        public ServiceStatusViewModel()
        {
            model = new Domain.Model.ServiceStatus();

            // M ->Vの接続
            ImporterStatus = model.ObserveProperty(x => x.ImporterStatus)
                .ToReactiveProperty();
            ResponderStatus = model.ObserveProperty(x => x.ResponderStatus)
                .ToReactiveProperty();
            AscStatus = model.ObserveProperty(x => x.AscStatus)
                .ToReactiveProperty();
            ScpCoreStatus = model.ObserveProperty(x => x.ScpCoreStatus)
                .ToReactiveProperty();
            MppsStatus = model.ObserveProperty(x => x.MppsStatus)
                .ToReactiveProperty();

            // VM => Vの接続
            ImporterStatus
                .Subscribe(x => model.ImporterStatus = x);

            // 全てのサービスを再起動するコマンド
            RestartServiceCommand.Subscribe(_ => model.RestartServiceAll());

            ScreenSynchronousTimer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            // タイマーを購読し、
            // サービスのステータスを最新のものに同期する
            ScreenSynchronousTimer.Subscribe(_=>model.RefreshServiceStatus());

            ScreenSynchronousTimer.Start();
        }

    }
}
