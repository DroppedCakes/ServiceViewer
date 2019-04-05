﻿using MiniRisViewer.Domain.Service;
using NLog;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniRisViewer.Domain.Model
{

    /// <summary>
    ///
    /// </summary>
    public class ServiceAdministrator : BindableBase
    {

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// 各サービスの状態・操作を行う
        /// </summary>
        public ServiceManager[] ServiceManagers { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceAdministrator(Config config)
        {
            try { 
            this.ServiceManagers = config.Services
                .Select(service => new ServiceManager(service.Name, service.Caption, service.LogPath, System.Convert.ToBoolean(service.Visible)))
                .ToArray();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Config.xmlファイル内容エラー");
                return;
            }
        }

        /// <summary>
        /// 各サービスの状態を更新
        /// </summary>
        public async void UpdateServiceStatusAsync()
        {
            await Task.WhenAll(
                ServiceManagers.Select(service => Task.Run(() => service.GetServiceState()))
            );
        }

        ///<summary>
        ///全サービスの停止を行う
        /// </summary>
        public async Task AllstopServiceAsync()
        {
            await Task.WhenAll(
                ServiceManagers.Select(service => Task.Run(() => service.Stop()))
            );
        }

        ///<summary>
        ///全サービスの起動を行う
        /// </summary>
        public async Task AllstartServiceAsync()
        {
            await Task.WhenAll(
                ServiceManagers.Select(service => Task.Run(() => service.Start()))
            );
        }

        ///<summary>
        ///全サービスの再起動を行う
        /// </summary>
        public async Task RestartServiceAsync()
        {
            await Task.WhenAll(
                ServiceManagers.Select(service => Task.Run(() => service.Restart()))
            );
        }
    }
}