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
        /// <summary>
        /// Model
        /// </summary>
        public Domain.Model.ServiceStatus model;

        /// <summary>
        ///  サービスの状態を保持するプロパティ
        /// </summary>
        #region ReactiveProperty
        public ReactiveProperty<string> ImporterStatus { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ResponderStatus { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> AscStatus{ get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> ScpCoreStatus { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> MppsStatus { get; } = new ReactiveProperty<string>();
        #endregion ReactiveProperty

        #region ReactiveCommand
        public ReactiveCommand ImporterStartCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ImporterStopCommand { get; }
        public ReactiveCommand ResponderStartCommand { get; }
        public ReactiveCommand ResponderStopCommand { get; }
        public ReactiveCommand ScpCoreStartCommand { get; }
        public ReactiveCommand ScpCoreStopCommand { get; }
        public ReactiveCommand AscStartCommand { get; }
        public ReactiveCommand AscStopCommand { get; }
        public ReactiveCommand MppsStartCommand { get; }
        public ReactiveCommand MppsStopCommand { get; }
        public ReactiveCommand RestartServiceCommand { get; } = new ReactiveCommand();
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
        public ServiceStatusViewModel()
        {
            // 後でDIにする
            model = new Domain.Model.ServiceStatus();

            // M -> VMの接続
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


            // ReverseStateのお試し用
            ImporterStartCommand.Subscribe(_ => model.ReverseState("UsFileImporter"));



            // 全てのサービスを再起動するコマンド
            RestartServiceCommand.Subscribe(_ => model.RestartServiceAll());

            // 1秒ごとに購読する
            ScreenSynchronousTimer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            // タイマーを購読し、
            // サービスのステータスを最新のものに同期する
            ScreenSynchronousTimer.Subscribe(_=>model.RefreshServiceStatus());

            ScreenSynchronousTimer.Start();
        }

    }
}
