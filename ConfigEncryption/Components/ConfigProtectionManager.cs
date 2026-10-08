using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Tjc.Modules.ConfigEncryption.Components
{
    public class SectionStatus
    {
        public string Name { get; set; }
        public bool IsProtected { get; set; }
        public string Provider { get; set; }
        public int ItemCount { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Encrypts and decrypts the connectionStrings and appSettings sections of the site's root web.config
    /// using the .NET protected configuration providers.
    /// </summary>
    public static class ConfigProtectionManager
    {
        public const string RsaProvider = "RsaProtectedConfigurationProvider";
        public const string DataProtectionProvider = "DataProtectionConfigurationProvider";

        public static readonly string[] ManagedSections = { "connectionStrings", "appSettings" };
        public static readonly string[] Providers = { RsaProvider, DataProtectionProvider };

        private const string BackupFolder = "~/App_Data/ConfigBackups";

        public static IList<SectionStatus> GetStatus()
        {
            var config = OpenConfig();
            return ManagedSections.Select(name => ReadStatus(config, name)).ToList();
        }

        public static void Protect(string section, string provider)
        {
            section = NormalizeSection(section);
            provider = string.IsNullOrWhiteSpace(provider) ? RsaProvider : provider;
            if (!Providers.Contains(provider, StringComparer.Ordinal))
            {
                throw new ArgumentException("Unknown protection provider '" + provider + "'.");
            }

            var config = OpenConfig();
            var configSection = GetSection(config, section);
            if (configSection.SectionInformation.IsProtected)
            {
                throw new InvalidOperationException("The " + section + " section is already encrypted.");
            }

            Backup(config);
            configSection.SectionInformation.ProtectSection(provider);
            configSection.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static void Unprotect(string section)
        {
            section = NormalizeSection(section);

            var config = OpenConfig();
            var configSection = GetSection(config, section);
            if (!configSection.SectionInformation.IsProtected)
            {
                throw new InvalidOperationException("The " + section + " section is not encrypted.");
            }

            Backup(config);
            configSection.SectionInformation.UnprotectSection();
            configSection.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static string NormalizeSection(string section)
        {
            var match = ManagedSections.FirstOrDefault(s => string.Equals(s, section, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                throw new ArgumentException("Section '" + section + "' cannot be managed here.");
            }
            return match;
        }

        private static Configuration OpenConfig()
        {
            return WebConfigurationManager.OpenWebConfiguration("~");
        }

        private static ConfigurationSection GetSection(Configuration config, string name)
        {
            var section = config.GetSection(name);
            if (section == null)
            {
                throw new InvalidOperationException("The " + name + " section was not found in web.config.");
            }
            if (section.SectionInformation.IsLocked)
            {
                throw new InvalidOperationException("The " + name + " section is locked and cannot be changed.");
            }
            return section;
        }

        private static SectionStatus ReadStatus(Configuration config, string name)
        {
            var status = new SectionStatus { Name = name };
            try
            {
                var section = config.GetSection(name);
                if (section == null)
                {
                    status.Error = "Section not found in web.config.";
                    return status;
                }

                status.IsProtected = section.SectionInformation.IsProtected;
                if (status.IsProtected && section.SectionInformation.ProtectionProvider != null)
                {
                    status.Provider = section.SectionInformation.ProtectionProvider.Name;
                }

                var connectionStrings = section as ConnectionStringsSection;
                var appSettings = section as AppSettingsSection;
                status.ItemCount = connectionStrings != null ? connectionStrings.ConnectionStrings.Count
                    : appSettings != null ? appSettings.Settings.Count : 0;
            }
            catch (Exception ex)
            {
                // Typically the key container is not readable by the app pool identity.
                status.Error = ex.Message;
            }
            return status;
        }

        // The backup holds the file as it was before the change, so it contains plain text secrets when
        // encrypting. It lives under App_Data (never served) and should be removed once the change is verified.
        private static void Backup(Configuration config)
        {
            var folder = HostingEnvironment.MapPath(BackupFolder);
            Directory.CreateDirectory(folder);
            var target = Path.Combine(folder, "web.config." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".bak");
            File.Copy(config.FilePath, target, false);
        }
    }
}
