using MiniRisViewer.Domain.Model;
using MiniRisViewer.Domain.Service;
using Prism.Commands;
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
    public class ServiceViewModel
    {
        public ReactiveProperty<ServiceControllerStatus> Status { set; get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<bool> CanStop { set; get; } = new ReactiveProperty<bool>();
        //public ReactiveCommand StartStopCommand { set; get; } = new ReactiveCommand();
        //public ReactiveCommand ShowLogCommand { get; set; } = new ReactiveCommand();
        public string DisplayName { get; }

        public DelegateCommand StopCommand { get; }
        public DelegateCommand StartCommand { get; }
        public DelegateCommand StartStopCommand { set; get; }
        public DelegateCommand ShowLogCommand { set; get; }


        /// <summary>
        /// 
        /// </summary>
        public ServiceViewModel(ServiceManager model)
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

        ////試作1
        //public int ServiceIndex { get; set; }

        //試作2
        //public void SetCommand(ServiceManager x)
        //{
        //    StartStopCommand.Subscribe(
        //        () => { if (CanStop.Value) x.Stop(); else x.Start(); }
        //    );
        //    ShowLogCommand.Subscribe(() => x.ShowLogFolder());
        //}
    }


    public class ServiceStatusViewModel : BindableBase, IDisposable
    {
        #region other
        public Reactive.Bindings.Notifiers.BooleanNotifier InProgress { get; } = new Reactive.Bindings.Notifiers.BooleanNotifier(false);
        public ReactivePropertySlim<string> ProgressMessage { get; } = new ReactivePropertySlim<string>("");

        /// <summary>
        /// Model
        /// </summary>
        public Domain.Model.ServiceAdministrator Model;

        ///<summary>
        ///全サービスの状態
        ///</summary>
        public ServiceViewModel[] ServiceCards { get; }

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

        #endregion 


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatusViewModel()
        {
            CreateModel();

            this.ServiceCards = Model.ServiceManagers
                .Where(service => service.Visible)
                .Select(service => new ServiceViewModel(service))
                .ToArray();


            //試作2:indexをつかわず
            //.Subscribeの処理があるので、結局foreach以外繰り返し方法が思いつかないためこうなりましたー

            //this.ServiceCards = new List<ServiceViewModel>();
            //foreach (ServiceManager x in Model.ServiceManagers)
            //{
            //    if ((0 < (int)x.Status) || ((int)x.Status) <= 7)
            //    {

            //        ServiceViewModel tmp = new ServiceViewModel
            //        {
            //            CanStop = x.ObserveProperty(y => y.CanStop).ToReactiveProperty(),
            //            Status = x.ObserveProperty(y => y.Status).ToReactiveProperty(),
            //            DisplayName = x.DisplayName,
            //            StartStopCommand = new ReactiveCommand(),
            //            ShowLogCommand = new ReactiveCommand()
            //        };

            //        tmp.SetCommand(x);

            //        ServiceCards.Add(tmp);
            //    }
            //}

            //試作1:indexを追加して対応（未インストール時のstatuには未対応)
            //int i = 0;
            //this.ServiceCards = Model.ServiceManagers
            //    .Select(a => new ServiceViewModel
            //    {
            //        CanStop = a.ObserveProperty(x => x.CanStop).ToReactiveProperty(),
            //        Status = a.ObserveProperty(x => x.Status).ToReactiveProperty(),
            //        DisplayName = a.DisplayName,
            //        StartStopCommand = new ReactiveCommand(),
            //        ShowLogCommand = new ReactiveCommand(),
            //        ServiceIndex = i++
            //    }).ToList();
            // System.Collections.IEnumerable' を実装していないため、型 'ReactiveCommand' はコレクション初期化子で初期化することはできません。
            //と言われ、宣言時に設定する方法がなさそうなため、結局。。。foreach。indexを追加したのでテンポラリ問題は解決
            //foreach (ServiceViewModel x in this.ServiceCards)
            //{
            //    x.StartStopCommand.Subscribe(() =>
            //                           {
            //                               if (x.CanStop.Value != false) Model.ServiceManagers[x.ServiceIndex].Stop();
            //                               else Model.ServiceManagers[x.ServiceIndex].Start();
            //                           });
            //    x.ShowLogCommand.Subscribe(() => Model.ServiceManagers[x.ServiceIndex].ShowLogFolder());
            //}



            //アドバイス前
            //int loopindex;

            //int serviceCount = Enum.GetValues(typeof(Ailias)).Length;

            //this.ServiceCards = new List<ServiceViewModel>();

            //int status;
            //for (loopindex = 0; loopindex < serviceCount; loopindex++)
            //{
            //    //ServiceViewModel TmpServiceCard = new ServiceViewModel();
            //    status = (int)Model.ServiceManagers[loopindex].Status;

            //    //indexで値を受けないと（loopindex =  serviceCount)固定となる
            //    int index = loopindex;

            //    if ((0 <= status) && (status <= 7))
            //    {

            //        ServiceViewModel TmpServiceCard = new ServiceViewModel();
            //        ServiceCards.Add(TmpServiceCard);


            //        // Stop判定のM -> VMの接続
            //        ServiceCards[index].CanStop = Model.ServiceManagers[index].ObserveProperty(x => x.CanStop).ToReactiveProperty();

            //        // ステータスのM -> VMの接続
            //        ServiceCards[index].Status = Model.ServiceManagers[index].ObserveProperty(x => x.Status).ToReactiveProperty();

            //        //画面表示名のM->VMの接続
            //        ServiceCards[index].DisplayName = Model.ServiceManagers[index].DisplayName;

            //        //// 開始・停止
            //        ServiceCards[index].StartStopCommand.Subscribe(() =>
            //        {
            //            if (ServiceCards[index].CanStop.Value != false) Model.ServiceManagers[index].Stop();
            //            else Model.ServiceManagers[index].Start();

            //        });

            //        // ログ
            //        ServiceCards[index].ShowLogCommand.Subscribe(() => Model.ServiceManagers[index].ShowLogFolder());

            //    }
            //}

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