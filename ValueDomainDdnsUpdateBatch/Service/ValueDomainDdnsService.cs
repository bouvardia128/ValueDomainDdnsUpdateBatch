using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using ValueDomainDdnsUpdateBatch.Helper;

namespace ValueDomainDdnsUpdateBatch.Service
{
    /// <summary>
    /// VALUE-DOMAIN のDDNS 更新API に対するサービスです
    /// </summary>
    public class ValueDomainDdnsService
    {
        private const string DDNS_UPDATE_URL = "https://dyn.value-domain.com/cgi-bin/dyn.fcg";

        /// <summary>
        /// DDNS のIP アドレスを更新します
        /// </summary>
        /// <param name="ipAddress">IP アドレス</param>
        /// <returns></returns>
        public ValueDomainUpdateResult Update(string ipAddress)
        {
            var result = "";
            var webClient = BuildUpdateWebClient(ipAddress);
            try
            {
                LogHelper.WriteLog(LogLevel.Information, $"DDNS API にアクセスします。");
                result = webClient.DownloadString(DDNS_UPDATE_URL);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(LogLevel.Error, $"DDNS API に正常にアクセスできませんでした。{e.Message}");
                throw e;
            }
            return GetResultCode(result);
        }
        /// <summary>
        /// DDNS Update 用のWebClientを 構築します
        /// </summary>
        /// <param name="ipAddress">IP アドレス</param>
        /// <returns>クエリ文字列を仕込んだWebClient</returns>
        private WebClient BuildUpdateWebClient(string ipAddress)
        {
            var domain = AppSettingHelper.GetAppSetting("ValueDomain", "Domain");
            var password = AppSettingHelper.GetAppSetting("ValueDomain", "Password");
            var hostName = AppSettingHelper.GetAppSetting("ValueDomain", "HostName");

            var webClient = new WebClient();
            webClient.QueryString.Add("d", domain);
            webClient.QueryString.Add("p", password);
            webClient.QueryString.Add("h", hostName);
            webClient.QueryString.Add("i", ipAddress);
            return webClient;
        }
        /// <summary>
        /// API レスポンスから結果コードを抽出します。
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private ValueDomainUpdateResult GetResultCode(string response)
        {
            // そもそもレスポンスなしは論外
            if (string.IsNullOrWhiteSpace(response))
            {
                LogHelper.WriteLog(LogLevel.Error, "DDNS 更新レスポンスがありません。");
                return ValueDomainUpdateResult.OTHER_ERROR;
            }

            // レスポンス文字列を解析してStatus を抽出する
            var resArray = response.Split('\n');
            if (resArray.Length == 0)
            {
                LogHelper.WriteLog(LogLevel.Error, $"DDNS 更新レスポンス フォーマットが不正です。想定外の形式で返却されているようです。response : {response}");
                return ValueDomainUpdateResult.OTHER_ERROR;
            }
            var statusArray = resArray[0].Split("=");
            if (statusArray.Length != 2)
            {
                LogHelper.WriteLog(LogLevel.Error, $"DDNS 更新レスポンス フォーマットが不正です。Key=Value 型ではないようです。response : {response}");
                return ValueDomainUpdateResult.OTHER_ERROR;
            }
            int.TryParse(statusArray[0], out var statusCode);
            return (ValueDomainUpdateResult)Enum.ToObject(typeof(ValueDomainUpdateResult), statusCode);
        }
    }

    /// <summary>
    /// 更新結果の定数群です。
    /// 定義は <a href="https://www.value-domain.com/ddns.php?action=howto">VALUE-DOMAIN リファレンス</a>を参照
    /// </summary>
    public enum ValueDomainUpdateResult
    {
        // 更新に成功
        UPDATE_SUCCESS = 0,
        // 不正なリクエスト
        BAD_REQUEST = 1,
        // 不正なドメインとパスワード
        BAD_DOMAIN_AND_PASSWORD = 2,
        // 不正なIP アドレス
        BAD_IP_ADDRESS = 3,
        // パスワードが不一致
        PASSWORD_UNMATCHED = 4,
        // データベースサーバーが混雑している
        DATABASE_BUSY = 5,
        // 更新対象のレコードがない
        NO_UPDATE_TARGET = 8,
        // その他のエラー
        OTHER_ERROR = 9,
        // 連続アクセス等の過負荷エラー
        TEMPORARY_UNAVAILABLE = 503
    }
}
