using System.Configuration;

namespace AxionDataUpload.Data
{
    public class AppSettings
    {
        public string DlFtpHost { get { return GetAppSettingValue("dl-site:Host"); } }
        public string DlFtpUsername { get { return GetAppSettingValue("dl-site:Username"); } }
        public string DlFtpPassword { get { return GetAppSettingValue("dl-site:Password"); } }
        public string PeopleFirstDirectory { get { return GetAppSettingValue("PeopleFirstDirectory"); } }
        public string UlFtpHost { get { return GetAppSettingValue("ul-site:Host"); } }
        public string UlFtpUsername { get { return GetAppSettingValue("ul-site:Username"); } }
        public string UlFtpPassword { get { return GetAppSettingValue("ul-site:Password"); } }
        public string UlFtpDirectory { get { return GetAppSettingValue("ul-site:Directory"); } }
        public string ExportFileName { get { 
                string fileName = GetAppSettingValue("AxiomExportFilename");
                string dateText = DateTime.Now.ToString("yyyyMMdd");
                fileName = fileName.Replace("[DATE]", dateText);
                return fileName; } }
        public string LocalDirectoryPath { get { return GetAppSettingValue("LocalDirectory"); } }

        private string GetAppSettingValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                return value.ToString();
            return string.Empty;
        }
    }


}
