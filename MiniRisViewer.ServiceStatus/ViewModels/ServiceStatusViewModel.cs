﻿using MiniRisViewer.Domain.Model;
using MiniRisViewer.Domain.Service;
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
        public ReactiveCommand ShowLogCommand { get; set; } = new ReactiveCommand();
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
        public List<ServiceViewModel> ServiceCards;

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

            //this.ServiceCards = Model.ServiceManagers
            //    .Select(a => new ServiceViewModel
            //    {
            //        CanStop = a.ObserveProperty(x => x.CanStop).ToReactiveProperty(),
            //        Status = a.ObserveProperty(x => x.Status).ToReactiveProperty(),
            //        DisplayName = a.DisplayName,
            //        StartStopCommand = new ReactiveCommand(),
            //        ShowLogCommand = new ReactiveCommand()
            //    }).ToList();

            //int i = 0;
            //foreach ( ServiceViewModel x in this.ServiceCards)
            //{              
            //    x.StartStopCommand.Subscribe(() =>
            //                           {
            //                               if (x.CanStop.Value != false) Model.ServiceManagers[i].Stop();
            //                               else Model.ServiceManagers[i].Start();
            //                           });
            //    x.ShowLogCommand.Subscribe(() => Model.ServiceManagers[i].ShowLogFolder());
            //    i = i + 1;
            //}




            int loopindex;

            int serviceCount = Enum.GetValues(typeof(Ailias)).Length;

            this.ServiceCards = new List<ServiceViewModel>();

            int status;
            for (loopindex = 0; loopindex < serviceCount; loopindex++)
            {
                //ServiceViewModel TmpServiceCard = new ServiceViewModel();
                status = (int)Model.ServiceManagers[loopindex].Status;

                //indexで値を受けないと（loopindex =  serviceCount)固定となる
                int index = loopindex;

                if ((0 <= status) && (status <= 7))
                {

                    ServiceViewModel TmpServiceCard = new ServiceViewModel();
                    ServiceCards.Add(TmpServiceCard);


                    // Stop判定のM -> VMの接続
                    ServiceCards[index].CanStop = Model.ServiceManagers[index].ObserveProperty(x => x.CanStop).ToReactiveProperty();

                    // ステータスのM -> VMの接続
                    ServiceCards[index].Status = Model.ServiceManagers[index].ObserveProperty(x => x.Status).ToReactiveProperty();

                    //画面表示名のM->VMの接続
                    ServiceCards[index].DisplayName = Model.ServiceManagers[index].DisplayName;

                    //// 開始・停止
                    ServiceCards[index].StartStopCommand.Subscribe(() =>
                    {
                        if (ServiceCards[index].CanStop.Value != false) Model.ServiceManagers[index].Stop();
                        else Model.ServiceManagers[index].Start();

                    });

                    // ログ
                    ServiceCards[index].ShowLogCommand.Subscribe(() => Model.ServiceManagers[index].ShowLogFolder());

                }
            }

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