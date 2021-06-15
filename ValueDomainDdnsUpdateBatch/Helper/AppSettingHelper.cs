using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Extensions.Configuration;

namespace ValueDomainDdnsUpdateBatch.Helper
{
    /// <summary>
    /// アプリケーション設定関連操作を支援します
    /// </summary>
    public class AppSettingHelper
    {
        private const string SETTING_FILE_NAME = "appsettings.json";

        /// <summary>
        /// json にて定義されたアプリケーション設定を取得します。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <returns>設定値</returns>
        public static string GetAppSetting(string section, string key)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(SETTING_FILE_NAME, true, true)
                    .Build();

            var sectionObj = configuration.GetSection(section);
            return sectionObj[key];
        }
    }
}
