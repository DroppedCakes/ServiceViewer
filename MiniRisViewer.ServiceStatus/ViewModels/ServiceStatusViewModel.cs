using MiniRisViewer.Domain.Model;
using MiniRisViewer.Domain.Service;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;


namespace MiniRisViewer.ServiceStatus.ViewModels
{

    /// <summary>
    /// MV→Vのためのサービス
    /// </summary>
    public class ServiceViewModel
    {
        public ReactiveProperty<ServiceControllerStatus> Status { set; get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<bool> CanStop { set; get; } = new ReactiveProperty<bool>();
        public ReactiveCommand StartStopCommand { set; get; } = new ReactiveCommand();
        public ReactiveCommand ShowLogCommand { get; private set; } = new ReactiveCommand();
        public string DisplayName { get; set; }
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
        public List<ServiceViewModel> ServiceCards { get; set; }

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
        /// 
        /// </summary>
        public void CreateModel()
        {

            try
            {
                string ConfigPath = @"C:\ProgramData\UsTEC\MiniRisViewer\Config.xml";
                var config = ConfigLoader.LoadConfigFromFile(ConfigPath);
                Model = new ServiceAdministrator(config);
            }

            catch (Exception)
            {
                // メッセージボックスだして、アプリを終了させる
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatusViewModel()
        {
            CreateModel();


            int num;

            this.ServiceCards = new List<ServiceViewModel>();
            for (num = 0; num < Enum.GetValues(typeof(Ailias)).Length; ++num)
            {
                ServiceViewModel TmpServiceCard = new ServiceViewModel();
                ServiceCards.Add(TmpServiceCard);

            }


            // Stop判定のM -> VMの接続
            ServiceCards[0].CanStop = Model.ServiceManagers[0].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            // ステータスのM -> VMの接続
            ServiceCards[0].Status = Model.ServiceManagers[0].ObserveProperty(x => x.Status).ToReactiveProperty();
            //画面表示名のM->VMの接続
            ServiceCards[0].DisplayName = Model.ServiceManagers[0].DisplayName;
            //// 開始・停止
            ServiceCards[0].StartStopCommand.Subscribe(() =>
            {
                if (ServiceCards[0].CanStop.Value != false) Model.ServiceManagers[0].Stop();
                else Model.ServiceManagers[0].Start();
            });
            // ログ
            ServiceCards[0].ShowLogCommand.Subscribe(() => Model.ServiceManagers[0].ShowLogFolder());


            // Stop判定のM -> VMの接続
            ServiceCards[1].CanStop = Model.ServiceManagers[1].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            // ステータスのM -> VMの接続
            ServiceCards[1].Status = Model.ServiceManagers[1].ObserveProperty(x => x.Status).ToReactiveProperty();
            //画面表示名のM->VMの接続
            ServiceCards[1].DisplayName = Model.ServiceManagers[1].DisplayName;
            //// 開始・停止
            ServiceCards[1].StartStopCommand.Subscribe(() =>
            {
                if (ServiceCards[1].CanStop.Value != false) Model.ServiceManagers[1].Stop();
                else Model.ServiceManagers[1].Start();
            });
            // ログ
            ServiceCards[1].ShowLogCommand.Subscribe(() => Model.ServiceManagers[1].ShowLogFolder());


            // Stop判定のM -> VMの接続
            ServiceCards[2].CanStop = Model.ServiceManagers[2].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            // ステータスのM -> VMの接続
            ServiceCards[2].Status = Model.ServiceManagers[2].ObserveProperty(x => x.Status).ToReactiveProperty();
            //画面表示名のM->VMの接続
            ServiceCards[2].DisplayName = Model.ServiceManagers[2].DisplayName;
            //// 開始・停止
            ServiceCards[2].StartStopCommand.Subscribe(() =>
            {
                if (ServiceCards[2].CanStop.Value != false) Model.ServiceManagers[2].Stop();
                else Model.ServiceManagers[2].Start();
            });
            // ログ
            ServiceCards[2].ShowLogCommand.Subscribe(() => Model.ServiceManagers[2].ShowLogFolder());

            // Stop判定のM -> VMの接続
            ServiceCards[3].CanStop = Model.ServiceManagers[3].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            // ステータスのM -> VMの接続
            ServiceCards[3].Status = Model.ServiceManagers[3].ObserveProperty(x => x.Status).ToReactiveProperty();
            //画面表示名のM->VMの接続
            ServiceCards[3].DisplayName = Model.ServiceManagers[3].DisplayName;
            //// 開始・停止
            ServiceCards[3].StartStopCommand.Subscribe(() =>
            {
                if (ServiceCards[3].CanStop.Value != false) Model.ServiceManagers[3].Stop();
                else Model.ServiceManagers[3].Start();
            });
            // ログ
            ServiceCards[3].ShowLogCommand.Subscribe(() => Model.ServiceManagers[3].ShowLogFolder());


            // Stop判定のM -> VMの接続
            ServiceCards[4].CanStop = Model.ServiceManagers[4].ObserveProperty(x => x.CanStop).ToReactiveProperty();
            // ステータスのM -> VMの接続
            ServiceCards[4].Status = Model.ServiceManagers[4].ObserveProperty(x => x.Status).ToReactiveProperty();
            //画面表示名のM->VMの接続
            ServiceCards[4].DisplayName = Model.ServiceManagers[4].DisplayName;
            //// 開始・停止
            ServiceCards[4].StartStopCommand.Subscribe(() =>
            {
                if (ServiceCards[4].CanStop.Value != false) Model.ServiceManagers[4].Stop();
                else Model.ServiceManagers[4].Start();
            });
            // ログ
            ServiceCards[4].ShowLogCommand.Subscribe(() => Model.ServiceManagers[4].ShowLogFolder());


            //int num;
            //int serviceCount = Enum.GetValues(typeof(Ailias)).Length;

            //this.ServiceCards = new List<ServiceViewModel>();

            //for (num = 0; num < serviceCount; ++num)
            //{
            //    ServiceViewModel TmpServiceCard = new ServiceViewModel();
            //    ServiceCards.Add(TmpServiceCard);

            //}

            //int status =(int)Model.ServiceManagers[num].Status;
            //for (num = 0; num < serviceCount; num++)
            //{
            //    //ServiceViewModel TmpServiceCard = new ServiceViewModel();

            //    if (( 0 <= status ) && ( status <= 7))
            //    {

            //        // Stop判定のM -> VMの接続
            //        ServiceCards[num].CanStop = Model.ServiceManagers[num].ObserveProperty(x => x.CanStop).ToReactiveProperty();

            //        // ステータスのM -> VMの接続
            //        ServiceCards[num].Status = Model.ServiceManagers[num].ObserveProperty(x => x.Status).ToReactiveProperty();

            //        //画面表示名のM->VMの接続
            //        ServiceCards[num].DisplayName = Model.ServiceManagers[num].DisplayName;

            //        //// 開始・停止
            //        ServiceCards[num].StartStopCommand.Subscribe(() =>
            //        {
            //            if (ServiceCards[num].CanStop.Value != false) Model.ServiceManagers[num].Stop();
            //            else Model.ServiceManagers[num].Start();
            //        });


            //        // ログ
            //        ServiceCards[num].ShowLogCommand.Subscribe(() => Model.ServiceManagers[num].ShowLogFolder());

            //        //ServiceCards.Add(TmpServiceCard);
            //    }
            //}
            ////num = 0; debugで使用

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