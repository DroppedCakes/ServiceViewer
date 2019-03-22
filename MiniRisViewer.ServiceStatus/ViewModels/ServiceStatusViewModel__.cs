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
    public class ServiceViewModel
    {
        public ReactiveProperty<ServiceControllerStatus> Status { set; get; } = new ReactiveProperty<ServiceControllerStatus>();
        public ReactiveProperty<bool> CanStop { set; get; } = new ReactiveProperty<bool>();
        public ReactiveCommand StartStopCommand { set; get; } = new ReactiveCommand();
        public ReactiveCommand ShowLogCommand { get; private set; } = new ReactiveCommand();
        public string DisplayName { get; set; }
    }

    public class ServiceStatusViewModel__ : BindableBase, IDisposable
    {
        public Reactive.Bindings.Notifiers.BooleanNotifier InProgress { get; } = new Reactive.Bindings.Notifiers.BooleanNotifier(false);
        public ReactivePropertySlim<string> ProgressMessage { get; } = new ReactivePropertySlim<string>("");

        /// <summary>
        /// Model
        /// </summary>
        public Domain.Model.ServiceAdministrator Model;
        public ServiceViewModel[] ServiceViewModels { get; set; }


        /// <summary>
        ///  サービスの状態を保持するプロパティ
        /// </summary>

        //1つのボタンにしてみる
        public ReactiveProperty<bool> StatusCommand { set; get; } = new ReactiveProperty<bool>();
        //1つのボタンにしてみる
        public ReactiveCommand ImporterCommand { get; } = new ReactiveCommand();


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


        public string[] DisplayNameList { get; set; }
        public int[] StatusList { get; set; }
        public ReactiveCommand[] LogCommandList { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatusViewModel__()
        {
            CreateModel();

            ServiceViewModels = new ServiceViewModel[Enum.GetValues(typeof(Ailias)).Length];

            for (int num = 0; num < Enum.GetValues(typeof(Ailias)).Length; num++) {

                ServiceViewModels[num] = new ServiceViewModel();
                
                // Stop判定のM -> VMの接続
                ServiceViewModels[num].CanStop = Model.ServiceManagers[num].ObserveProperty(x => x.CanStop).ToReactiveProperty();

                // ステータスのM -> VMの接続
                ServiceViewModels[num].Status = Model.ServiceManagers[num].ObserveProperty(x => x.Status).ToReactiveProperty();

                // 画面表示名のM -> VMの接続
                ServiceViewModels[num].DisplayName = Model.ServiceManagers[num].DisplayName;

                // 開始・停止
                ServiceViewModels[num].StartStopCommand.Subscribe(() => {
                        if (ServiceViewModels[num].CanStop.Value != false) Model.ServiceManagers[num].Stop();
                        else Model.ServiceManagers[num].Start();
                    });

                // ログ
                ServiceViewModels[num].ShowLogCommand.Subscribe(() => Model.ServiceManagers[num].ShowLogFolder());
            }

            StatusList = new int[Enum.GetValues(typeof(Ailias)).Length];
            int i = 0;
            foreach (ServiceViewModel x in ServiceViewModels)
            {
                StatusList[i] = (int)x.Status.Value;
                i = i + 1;
            }

            LogCommandList = new ReactiveCommand[Enum.GetValues(typeof(Ailias)).Length];
            i = 0;
            foreach (ServiceViewModel x in ServiceViewModels)
            {
                LogCommandList[i] = x.ShowLogCommand;
                i = i + 1;
            }

            DisplayNameList = new String [Enum.GetValues(typeof(Ailias)).Length];
            i = 0;
            foreach (ServiceViewModel x in ServiceViewModels)
            {
                DisplayNameList[i] = (String)x.DisplayName;
                i = i + 1;
            }

            // 1秒ごとに購読する
            ScreenSynchronousTimer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            // タイマーを購読し、
            // サービスのステータスを最新のものに同期する
            ScreenSynchronousTimer.Subscribe(_ => Model.UpdateServiceStatusAsync());
            ScreenSynchronousTimer.Start();
        }

    }


}