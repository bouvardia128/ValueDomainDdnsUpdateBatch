using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValueDomainDdnsUpdateBatch.Helper
{
    /// <summary>
    /// ログ出力を支援します
    /// </summary>
    public class LogHelper
    {
        private static string LogfileNamePrefix = "ValueDomainDdnsUpdate";
        private static string LogfileExtension = ".txt";
        private static string logfileName;

        /// <summary>
        /// 出力対象のログファイル名
        /// </summary>
        public static string LogFileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(logfileName)) { return logfileName; }
                logfileName = $"{LogfileNamePrefix}_{DateTime.Now.ToString("yyyyMMdd")}{LogfileExtension}";
                return logfileName;
            }
        }
        /// <summary>
        /// ログを書き込みます
        /// </summary>
        /// <param name="logLevel">ログレベル</param>
        /// <param name="message"></param>
        public static void WriteLog(LogLevel logLevel, string message)
        {
            var level = Enum.GetName(typeof(LogLevel), logLevel);
            File.AppendAllText(GetLogFullPath(), $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")}\t{level}\t{message}\n");
        }
        private static string GetLogFullPath()
        {
            var logFolder = AppSettingHelper.GetAppSetting("Logger", "LogFolder");
            return Path.Combine(logFolder, LogFileName);
        }
    }
    /// <summary>
    /// ログレベル
    /// </summary>
    public enum LogLevel
    {
        Verbose,
        Information,
        Warning,
        Error,
        Critical
    }
}
