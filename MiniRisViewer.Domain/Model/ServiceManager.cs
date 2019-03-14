using Prism.Mvvm;
using System;
using System.ServiceProcess;

namespace MiniRisViewer.Domain.Model
{
    public class ServiceManager : BindableBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceManager(string serviceName, string displayName, string logFolderPath, bool visble)
        {
            ServiceName = serviceName;
            DisplayName = displayName;
            LogFolderPath = logFolderPath;
            Visible = visble;
            GetServiceState();
        }

        /// <summary>
        /// サービスの正式名称
        /// </summary>
        public readonly string ServiceName;

        /// <summary>
        /// 画面表示名称
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// ログ出力先フォルダのパス
        /// </summary>
        public string LogFolderPath { get; }

        private bool visible;

        public bool Visible
        {
            get { return visible; }
            set { SetProperty(ref visible, value); }
        }

        /// <summary>
        /// サービスの状態
        /// </summary>
        private ServiceControllerStatus status;

        public ServiceControllerStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private bool canStop;

        public bool CanStop
        {
            get { return canStop; }
            set { SetProperty(ref canStop, value); }
        }

        /// <summary>
        /// サービスの状態を更新する
        /// </summary>
        public void GetServiceState()
        {
            using (ServiceController sc = new ServiceController(ServiceName))
            {
                Status = sc.Status;
                CanStop = sc.CanStop;
            }
        }

        /// <summary>
        /// サービスの開始を行う
        /// </summary>
        public bool Start()
        {
            TimeSpan timeout = new TimeSpan(00, 00, 10);

            bool CanStart = true;
            try
            {
                using (ServiceController sc = new ServiceController(ServiceName))
                {
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Stopped:
                            //停止中の場合、開始する
                            sc.Start();
                            // サービスが開始するまで待機する
                            sc.WaitForStatus(ServiceControllerStatus.Running, timeout);

                            if (sc.Status == ServiceControllerStatus.Running)
                            {
                                CanStart = true;
                            }
                            break;

                        case ServiceControllerStatus.StartPending:
                            //再開処理中の場合、何もしない
                            break;

                        case ServiceControllerStatus.StopPending:
                            //停止処理中の場合、何もしない
                            break;

                        case ServiceControllerStatus.Running:
                            //動作しているので何もしない
                            break;

                        case ServiceControllerStatus.ContinuePending:
                            //再開中の場合、何もしない
                            break;

                        case ServiceControllerStatus.PausePending:
                            //一時停止処理中の場合、何もしない
                            break;

                        case ServiceControllerStatus.Paused:
                            //一次停止の場合、何もしない
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                CanStart = false;
            }
            return CanStart;
        }

        /// <summary>
        /// サービスの停止を行う
        /// </summary>
        /// Stop可能な状態のみこのメソッドが呼ばれること
        public bool Stop()
        {
            TimeSpan timeout = new TimeSpan(00, 00, 10);

            bool CanStop = false;
            try
            {
                using (ServiceController sc = new ServiceController(ServiceName))
                {
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Stopped:
                            //停止中の場合、何もしない
                            break;

                        case ServiceControllerStatus.StartPending:
                            //再開処理中の場合、何もしない
                            break;

                        case ServiceControllerStatus.StopPending:
                            //停止処理中の場合、サービスが停止するまで待機する
                            break;

                        case ServiceControllerStatus.Running:
                            //動作中の場合、停止する
                            sc.Stop();
                            // サービスが停止するまで待機する
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                            if (sc.Status == ServiceControllerStatus.Stopped)
                            {
                                CanStop = true;
                            }
                            break;

                        case ServiceControllerStatus.ContinuePending:
                            //再開中の場合、何もしない
                            break;

                        case ServiceControllerStatus.PausePending:
                            //一時停止処理中の場合、何もしない
                            break;

                        case ServiceControllerStatus.Paused:
                            //一次停止中の場合、何もしない
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                CanStop = false;
            }
            return CanStop;
        }

        /// <summary>
        /// サービスの再起動を行う
        /// </summary>
        public void Restart()
        {
            TimeSpan timeout = new TimeSpan(00, 00, 10);
            using (ServiceController sc = new ServiceController(ServiceName))
            {
                //停止中以外の場合
                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    //停止処理中以外の場合
                    if (sc.Status != ServiceControllerStatus.StopPending)
                    {
                        //停止する
                        sc.Stop();
                    }
                    // サービスが停止するまで待機する
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                sc.Start();
                // サービスが開始するまで待機する
                sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
        }

        /// <summary>
        /// ログ出力先フォルダを
        /// エクスプローラで開く
        /// </summary>
        public void ShowLogFolder()
        {
            System.Diagnostics.Process.Start(
                "EXPLORER.EXE", LogFolderPath);
        }
    }
}