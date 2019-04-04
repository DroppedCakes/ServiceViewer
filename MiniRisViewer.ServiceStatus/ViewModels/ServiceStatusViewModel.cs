using MiniRisViewer.Domain.Model;
using MiniRisViewer.Domain.Service;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniRisViewer.ServiceStatus.ViewModels
{

    /// <summary>
    /// MV→Vのためのサービス
    /// </summary>
    public class Service
    {
        public ReactiveProperty<ServiceControllerStatus> Status { set; get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<bool> CanStop { set; get; } = new ReactiveProperty<bool>();
        public string DisplayName { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand StartCommand { get; }
        public DelegateCommand StartStopCommand { set; get; }
        public DelegateCommand ShowLogCommand { set; get; }


        /// <summary>
        /// 
        /// </summary>
        public Service(ServiceManager model)
        {
            this.Status = model.ObserveProperty(x => x.Status).ToReactiveProperty();
            this.CanStop = model.ObserveProperty(x => x.CanStop).ToReactiveProperty();

            this.DisplayName = model.DisplayName;

            this.StopCommand = new DelegateCommand(
                () => model.Stop(),
                () => model.CanStop
            );

            this.StartCommand = new DelegateCommand(
                () => model.Start()
            );

            this.StartStopCommand = new DelegateCommand(
                () => { if (model.CanStop) model.Stop(); else model.Start(); }
            );

            this.ShowLogCommand = new DelegateCommand(
                model.ShowLogFolder
            );
        }

    }


    public class ServiceStatusViewModel : BindableBase, IDisposable
    {
        public Reactive.Bindings.Notifiers.BooleanNotifier InProgress { get; } = new Reactive.Bindings.Notifiers.BooleanNotifier(false);
        public ReactivePropertySlim<string> ProgressMessage { get; } = new ReactivePropertySlim<string>("");

        /// <summary>
        /// Model
        /// </summary>
        public Domain.Model.ServiceAdministrator Model;
        ///<summary>
        ///全サービスの状態
        ///</summary>
        public Service[] Services { get; }

        /// <summary>
        /// 全てのサービスを停止するコマンド
        /// </summary>
        public ReactiveCommand AllstopServiceCommand { get; } = new ReactiveCommand();
        /// <summary>
        /// 全てのサービスを起動するコマンド
        /// </summary>
        public ReactiveCommand AllstartServiceCommand { get; } = new ReactiveCommand();
        /// <summary>
        /// 全てのサービスを再起動するコマンド
        /// </summary>
        public ReactiveCommand RestartServiceCommand { get; } = new ReactiveCommand();


        /// <summary>
        /// サービスの状態を更新するタイマー
        /// </summary>
        public ReactiveTimer ScreenSynchronousTimer;

        public ReactiveCommand ShowDialogMaterialCommand { get; private set; } = new ReactiveCommand();


        public InteractionRequest<Notification> NotificationRequestDialog { get; } = new InteractionRequest<Notification>();


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
        /// 非同期でサービスの停止を行い、
        /// UIはBusyIndicatorでブロックする
        /// </summary>
        public async void StopAllServiceAsync()
        {
            this.NotificationRequestDialog.Raise(new Notification { Title = "Alert", Content = "Notification message." });

            InProgress.TurnOn();
            ProgressMessage.Value = "サービス停止中";
            try
            {
                await Task.Run(Model.AllstopServiceAsync);
            }
            finally
            {
                InProgress.TurnOff();
            }
        }
        /// <summary>
        /// 非同期でサービスの再起動を行い、
        /// UIはBusyIndicatorでブロックする
        /// </summary>
        public async void StartAllServiceAsync()
        {
            InProgress.TurnOn();
            ProgressMessage.Value = "サービス起動中";
            try
            {
                await Task.Run(Model.AllstartServiceAsync);
            }
            finally
            {
                InProgress.TurnOff();
            }
        }        /// <summary>
                 /// 非同期でサービスの再起動を行い、
                 /// UIはBusyIndicatorでブロックする
                 /// </summary>
        public async void RestartAllServiceAsync()
        {
            InProgress.TurnOn();
            ProgressMessage.Value = "サービス再起動中";
            try
            {
                await Task.Run(Model.RestartServiceAsync);
            }
            finally
            {
                InProgress.TurnOff();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 


        public async Task CreateModelAsync()
        {

            try
            {

                string ConfigPath = @"C:\ProgramData\UsTEC\UsMiniRisViewer\Config.xml";
                var config = ConfigLoader.LoadConfigFromFile(ConfigPath);

                Model = new ServiceAdministrator(config);

            }

            catch (Exception ex)
            {
                // メッセージボックスだして、アプリを終了させる
            }


        }

        public InteractionRequest<Notification> NotificationRequest { get; } = new InteractionRequest<Notification>();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatusViewModel()
        {
            CreateModelAsync();

            this.Services = Model.ServiceManagers
                .Where(service => service.Visible)
                .Select(service => new Service(service))
                .ToArray();


            // 1秒ごとに購読する
            ScreenSynchronousTimer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            // タイマーを購読し、
            // サービスのステータスを最新のものに同期する
            ScreenSynchronousTimer.Subscribe(_ => Model.UpdateServiceStatusAsync());
            ScreenSynchronousTimer.Start();

            //// 全てのサービスを停止するコマンド
            AllstopServiceCommand.Subscribe(() => StopAllServiceAsync());
            //// 全てのサービスを起動するコマンド
            AllstartServiceCommand.Subscribe(() => StartAllServiceAsync());
            //// 全てのサービスを再起動するコマンド
            RestartServiceCommand.Subscribe(() => RestartAllServiceAsync());

        }


    }



}