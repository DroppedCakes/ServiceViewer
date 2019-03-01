using Prism.Mvvm;
using System.Collections.Generic;
using System.ServiceProcess;

namespace MiniRisViewer.Domain.Model
{
    public class ServiceStatus : BindableBase
    {
        /// <summary>
        /// サービスの通称とサービス名を保持するディクショナリー
        /// </summary>
        private static readonly ServiceManager[] Services =        {
            new ServiceManager(ServiceManager.UsFileImporterServiceName),
            new  ServiceManager(ServiceManager.UsMwmResponderServiceName),
            new  ServiceManager(ServiceManager.UsScpCoreServiceName ),
            new  ServiceManager(ServiceManager.UsAscServiceName),
            new  ServiceManager(ServiceManager.UsMppsReceiverServiceName ),
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceStatus()
        {
        }

        /// <summary>
        ///  FileImporterのステータス
        /// </summary>
        private string importerStatus;

        public string ImporterStatus
        {
            get { return importerStatus; }
            set { SetProperty(ref importerStatus, value); }
        }

        /// <summary>
        /// Responderのステータス
        /// </summary>
        private string responderStatus;

        public string ResponderStatus
        {
            get { return responderStatus; }
            set { SetProperty(ref responderStatus, value); }
        }

        /// <summary>
        /// AssociationServiceのステータス
        /// </summary>
        private string ascStatus;

        public string AscStatus
        {
            get { return ascStatus; }
            set { SetProperty(ref ascStatus, value); }
        }

        /// <summary>
        /// ScpCoreのステータス
        /// </summary>
        private string scpCoreStatus;

        public string ScpCoreStatus
        {
            get { return scpCoreStatus; }
            set { SetProperty(ref scpCoreStatus, value); }
        }

        /// <summary>
        /// MppsReceiverのステータス
        /// </summary>
        private string mppsStatus;

        public string MppsStatus
        {
            get { return mppsStatus; }
            set { SetProperty(ref mppsStatus, value); }
        }

        /// <summary>
        ///  対象のサービスの状態を取得し
        ///  文字列を反映させる
        /// </summary>
        public void RefreshServiceStatus()
        {
            // Linqでやりたい感
            // 型引数を明示化する必要があるらしい
            //Services.Select(x => SynchronizeServiceState(x.Key, x.Value));

            foreach (KeyValuePair<EpithetOfUs, string> x in Services)
            {
                SynchronizeServiceState(x.Key, x.Value);
            }
        }

        /// <summary>
        /// サービスの状態を
        /// 文字列に変換し、Propertyにセットする
        /// </summary>
        /// <param name="ailias"></param>
        /// <param name="serviceName"></param>
        private void SynchronizeServiceState(EpithetOfUs ailias, string serviceName)
        {
            // 引数のサービス名からサービスの状態を取得し、
            // 文字列に変換する
            var serviceStateString = ConvertToStatusName(GetServiceState(serviceName));

            // サービス名によって、
            // 文字列の代入先を変化させる
            switch (ailias)
            {
                case EpithetOfUs.Importer:
                    ImporterStatus = serviceStateString;
                    break;

                case EpithetOfUs.Responder:
                    ResponderStatus = serviceStateString;
                    break;

                case EpithetOfUs.ScpCore:
                    ScpCoreStatus = serviceStateString;
                    break;

                case EpithetOfUs.Asc:
                    AscStatus = serviceStateString;
                    break;

                case EpithetOfUs.Mpps:
                    MppsStatus = serviceStateString;
                    break;
            }
        }

        /// <summary>
        /// Enumに対応した文字列を返す
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private string ConvertToStatusName(ServiceControllerStatus status)
        {
            switch (status)
            {
                case ServiceControllerStatus.StartPending:
                    return "サービス開始させています";

                case ServiceControllerStatus.Running:
                    return "動作中";

                case ServiceControllerStatus.Stopped:
                    return "停止中";

                case ServiceControllerStatus.StopPending:
                    return "停止させています";

                case ServiceControllerStatus.Paused:
                    return "一時停止";

                case ServiceControllerStatus.PausePending:
                    return "一時停止させています";

                case ServiceControllerStatus.ContinuePending:
                    return "再開させています";

                default:
                    return "不明";
            }
        }

        /// <summary>
        /// 引数として与えられたサービス名から
        /// サービスの状態を取得する
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private ServiceControllerStatus GetServiceState(string serviceName)
        {
            ServiceControllerStatus retv;
            using (ServiceController sc = new ServiceController(serviceName))
            {
                retv = sc.Status;
            }
            return retv;
        }

        /// <summary>
        /// 引数として与えられた
        /// サービスの状態を反転させる
        /// </summary>
        /// <param name="serviceName"></param>
        public void ReverseState(string serviceName)
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                switch (sc.Status)
                {
                    case ServiceControllerStatus.ContinuePending:

                        break;

                    case ServiceControllerStatus.Paused:
                        sc.Continue();
                        break;

                    case ServiceControllerStatus.PausePending:
                        break;

                    case ServiceControllerStatus.Running:
                        break;

                    case ServiceControllerStatus.StartPending:
                        break;

                    case ServiceControllerStatus.Stopped:
                        sc.Start();
                        break;

                    case ServiceControllerStatus.StopPending:
                        break;

                    default:
                        break;
                }

                //if (sc.Status == ServiceControllerStatus.Paused )
                //{
                //    sc.Continue();
                //}
                //else
                //{
                //    sc.Pause();
                //}
            }
        }

        /// <summary>
        ///  非同期でタスクの再起動を行う予定
        /// </summary>
        public async void RestartServiceAll()
        {
            // ここもLINQでやりたいよね
            //Services.Select(x => RestartService(x.Value));
            foreach (KeyValuePair<EpithetOfUs, string> x in Services)
            {
                RestartService(x.Value);
            }
        }

        /// <summary>
        /// 引数として与えられた
        /// サービスの再起動を行う
        /// </summary>
        /// <param name="serviceName"></param>
        private void RestartService(string serviceName)
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    // サービスが停止されるまで待機する
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }

                sc.Start();
                // サービスが開始されるまで待機する
                sc.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
    }
}