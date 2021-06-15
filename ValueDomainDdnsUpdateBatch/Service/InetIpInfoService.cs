using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using ValueDomainDdnsUpdateBatch.Helper;

namespace ValueDomainDdnsUpdateBatch.Service
{
    /// <summary>
    /// inet-ip.info に対するサービスです
    /// </summary>
    public class InetIpInfoService
    {
        private const string IP_CHECK_ADDRESS = "http://inet-ip.info/ip";

        /// <summary>
        /// WAN 側IPアドレスを返却します。
        /// 取得できない場合はnull を返却します。
        /// </summary>
        /// <returns>WAN 側IP アドレス</returns>
        public string GetWanIpAddress()
        {
            var webClient = new WebClient();
            try
            {
                LogHelper.WriteLog(LogLevel.Information, $"WAN IP アドレス取得サービスにアクセスします。");
                return webClient.DownloadString(IP_CHECK_ADDRESS);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(LogLevel.Error, $"WAN IP アドレス取得サービスに正常にアクセスできませんでした。{e.Message}");
                throw e;
            }
        }
    }
}
