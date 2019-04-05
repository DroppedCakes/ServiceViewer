using MiniRisViewer.Domain.Model;
using MiniRisViewer.Domain.Service;
using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;

namespace MiniRisViewer.ServiceStatus.ViewModels
{
    /// <summary>
    /// MV→Vのためのサービス
    /// </summary>
    public class Service
    {
        private CompositeDisposable DisposeCollection = new CompositeDisposable();

        public ReactiveProperty<ServiceControllerStatus> Status { set; get; }
        public ReactiveProperty<bool> CanStop { set; get; }
        public string DisplayName { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand StartCommand { get; }
        public DelegateCommand StartStopCommand { set; get; }
        public DelegateCommand ShowLogCommand { set; get; }

        /// <summary>
        /// 
        /// </summary>
        internal Service()
        {
            this.DisplayName = "Service";
        }

        /// <summary>
        ///
        /// </summary>
        public Service(ServiceManager model)
        {
            this.Status = model.ObserveProperty(x => x.Status).ToReactiveProperty().AddTo(this.DisposeCollection);
            this.CanStop = model.ObserveProperty(x => x.CanStop).ToReactiveProperty().AddTo(this.DisposeCollection);

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
        public Domain.Model.ServiceAdministrator Model { get; }

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


        /// <summary>
        /// IDisposableの実装部
        /// </summary>
        private CompositeDisposable DisposeCollection = new CompositeDisposable();

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
        ///　Viewで確認用コンストラクタ
        /// </summary>
        public ServiceStatusViewModel() {
            this.Services = new[] {
                new Service()
                {
                }
            };
        }

        
        //InteractionRequestクラスのプロパティ
        public InteractionRequest<Notification> NotificationRequest { get; } = new InteractionRequest<Notification>();

        public DelegateCommand NotificationCommand { get; }

        //Raiseイベントの実装
        private void NotificationCommandExecute()
        {
            this.NotificationRequest.Raise(new Notification { Title = "Dialog", Content = "Notification message." });
        }


        public InteractionRequest<Notification> MetroNotification { get; private set; } = new InteractionRequest<Notification>();
        public ReactiveCommand RaiseMetroNotification { get; private set; } = new ReactiveCommand();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatusViewModel(ServiceAdministrator model)
        {
            this.Model = model;
            //CreateModel();

            try {
                this.Services = Model.ServiceManagers
                .Where(service => service.Visible)
                .Select(service => new Service(service))
                .ToArray();

            }
            catch (Exception ex)
            {

                _logger.Log(LogLevel.Error, ex, "アプリ終了");


                this.MetroNotification.Raise(new Notification()
                {
                    Title = "MetroNotification",
                    Content = "MetroNotification",
                }, n => Console.WriteLine("MetroNotification"));


                Environment.Exit(0);
            }


            // 1秒ごとに購読する
            ScreenSynchronousTimer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            // タイマーを購読し、
            // サービスのステータスを最新のものに同期する
            ScreenSynchronousTimer.Subscribe(_ => Model.UpdateServiceStatusAsync()).AddTo(this.DisposeCollection);
            ScreenSynchronousTimer.Start();

            //// 全てのサービスを停止するコマンド
            AllstopServiceCommand.Subscribe(() => StopAllServiceAsync()).AddTo(this.DisposeCollection);
            //// 全てのサービスを起動するコマンド
            AllstartServiceCommand.Subscribe(() => StartAllServiceAsync()).AddTo(this.DisposeCollection);
            //// 全てのサービスを再起動するコマンド
            RestartServiceCommand.Subscribe(() => RestartAllServiceAsync()).AddTo(this.DisposeCollection);

            this.NotificationCommand = new DelegateCommand(this.NotificationCommandExecute);

            this.RaiseMetroNotification.Subscribe(() => this.MetroNotification.Raise(new Notification()
            {
                Title = "MetroNotification",
                Content = "MetroNotification",
            }, n => Console.WriteLine("MetroNotification")));


        }
    }
}