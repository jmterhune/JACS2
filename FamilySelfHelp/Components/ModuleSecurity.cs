using DotNetNuke.Security.Permissions;
using DotNetNuke.Entities.Modules;

namespace tjc.Modules.FamilySelfHelp.Components
{
    public class ModuleSecurity
    {
        public const string PERMISSIONCODE = "LOG_MODULE";
        public const string REPORTS = "REPORTS";
        public const string SUPEREDIT = "SUPEREDIT";
        public const string MERGE = "MERGE";
        public const string DELETELOG = "DELETELOG";

        // private variables
        private bool _hasMerge;
        private bool _hasReports;
        private bool _hasSuperEdit;
        private bool _hasDelete;

        public ModuleSecurity(ModuleInfo moduleInfo)
        {
            ModulePermissionCollection permCollection = moduleInfo.ModulePermissions;
            _hasMerge = ModulePermissionController.HasModulePermission(permCollection, MERGE);
            _hasReports = ModulePermissionController.HasModulePermission(permCollection, REPORTS);
            _hasSuperEdit = ModulePermissionController.HasModulePermission(permCollection, SUPEREDIT);
            _hasDelete = ModulePermissionController.HasModulePermission(permCollection, DELETELOG);
        }


        public bool HasReportPermission
        {
            get
            {
                return _hasReports;
            }
        }

        public bool HasSuperEditPermission
        {
            get
            {
                return _hasSuperEdit;
            }
        }

        public bool HasMergePermission
        {
            get
            {
                return _hasMerge;
            }
        }

        public bool HasDeletePermission
        {
            get
            {
                return _hasDelete;
            }
        }
    }
}