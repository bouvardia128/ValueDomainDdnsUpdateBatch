using System;

using ValueDomainDdnsUpdateBatch.Helper;
using ValueDomainDdnsUpdateBatch.Service;

namespace ValueDomainDdnsUpdateBatch
{
    class Program
    {
        /// <summary>
        /// アプリケーションのメインルートです。
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            LogHelper.WriteLog(LogLevel.Information, "ValueDomain DDNS 更新処理を開始します。");
            var inetIpInfoService = new InetIpInfoService();
            var valueDomainDdnsService = new ValueDomainDdnsService();
            var ipAddress = inetIpInfoService.GetWanIpAddress();
            var result = valueDomainDdnsService.Update(ipAddress);
            if (result == ValueDomainUpdateResult.UPDATE_SUCCESS)
            {
                // 正常終了
                LogHelper.WriteLog(LogLevel.Information, "DDNS 更新が正常に完了しました。");
            }
            else
            {
                var errorStatus = Enum.GetName(typeof(ValueDomainUpdateResult), result);
                // 異常終了
                LogHelper.WriteLog(LogLevel.Information, $"DDNS 更新時にエラーが発生しました。ErrorStatus : {errorStatus}");

            }
            LogHelper.WriteLog(LogLevel.Information, "ValueDomain DDNS 更新処理を終了します。");

        }
    }
}
