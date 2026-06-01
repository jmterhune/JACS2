using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;

namespace tjc.Modules.ExpertWitness.Components
{
    [TableName("tjc_expert_requested_template")]
    //setup the primary key for table
    [PrimaryKey("RequestID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("ExperWitnessTemplates", CacheItemPriority.Default, 20)]
    internal class RequestCart : ExpertBase
    {
        public int RequestID { get; set; }
        public int TemplateID { get; set; }
        public int ExpertID { get; set; }
        public Guid Guid { get; set; }
        public string ExpertName { get; set; }
        public int Sequence { get; set; }
        public int Status { get; set; }
        public int CurrentOrder { get; set; }
        public string Comments { get; set; }

    }
    internal class RequestedTemplate : RequestCart
    {
        public int NumberRequired { get; set; }
        public string Header { get; set; }
        public int Position { get; set; }
        public string HeaderTypes
        {
            get
            { var ctl = new TemplateController();
                TemplateSequence templateSequence = ctl.GetTemplateSequence(TemplateID,Sequence);
                string headerTypes = string.Format("Requirement #{0}: ", Sequence);
                headerTypes += string.Format("Select {0} of the following ( ", templateSequence.NumberRequired);
               
                IEnumerable<Type> types = ctl.GetTemplateTypeTypesBySequence(TemplateID,Sequence);
                foreach (Type type in types)
                {
                    headerTypes += type.TypeName + " or ";
                }
                return headerTypes.Trim().TrimEnd('r').TrimEnd('o') + ")";
            }
        }

    }
    internal enum RequestStatus
    {
        unselected,
        passed,
        selected
    }

    internal class RequestedTemplateComparer : IEqualityComparer<RequestedTemplate>
    {
        public bool Equals(RequestedTemplate x, RequestedTemplate y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return (x.ExpertID == y.ExpertID) && (x.Sequence == y.Sequence);
        }

        public int GetHashCode(RequestedTemplate obj)
        {
            if (obj == null)
                return 0;
            var hashRequesteTemplateExpertId = obj.ExpertID.GetHashCode();
            var hashRequestTemplateSequence = obj.Sequence.GetHashCode();
            return hashRequesteTemplateExpertId ^ hashRequestTemplateSequence;
        }
    }

    internal class TemplateExpertSelection
    {
        public IEnumerable<Type> ExpertTypes
        {
            get; set;
        }
        public int NumberRequired
        {
            get; set;
        }

        public int Sequence
        {
            get; set;
        }

        public string TypeNames
        {
            get
            {
                string types = "";
                foreach (Type t in ExpertTypes)
                    types += t.TypeName + ";";
                return types.Trim().TrimEnd(';');
            }
        }
    }

    [Serializable]
    internal class AddedExpert
    {
        public int Sequence
        {
            get; set;
        }

        public int Count
        {
            get; set;
        }
    }
}