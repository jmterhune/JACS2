using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using System;
using System.Collections.Generic;

namespace tjc.Modules.JudicialReferral.Components
{
    public class FeatureController : ModuleSearchBase, IPortable, IUpgradeable
    {
        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            return new List<SearchDocument>();
        }

        public string ExportModule(int moduleId)
        {
            return string.Empty;
        }

        public void ImportModule(int moduleId, string content, string version, int userId)
        {
        }

        public string UpgradeModule(string version)
        {
            return "JudicialReferral module upgraded to version " + version;
        }
    }
}
