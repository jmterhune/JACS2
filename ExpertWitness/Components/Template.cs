using DotNetNuke.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Caching;

namespace tjc.Modules.ExpertWitness.Components
{
    [TableName("tjc_expert_template")]
    //setup the primary key for table
    [PrimaryKey("TemplateID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("ExperWitnessTemplates", CacheItemPriority.Default, 20)]
    internal class Template : ExpertBase
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        [IgnoreColumn]
        public string TypesRequired
        {
            get
            {
                string typesRequired = "";
                var ctl = new TemplateController();
                IEnumerable<TemplateSequence> templateSequences = ctl.GetTemplateSequences(TemplateID);
                foreach (TemplateSequence templateSequence in templateSequences)
                {
                    IEnumerable<Type> templateTypes = ctl.GetTemplateTypeTypesBySequence(TemplateID,templateSequence.Sequence);
                    foreach (Type templateType in templateTypes)
                    {
                        typesRequired += templateType.TypeName + ", ";
                    }
                    typesRequired = typesRequired.TrimEnd(',');
                    typesRequired += string.Concat("(", templateSequence.NumberRequired, ") - ");
                }
                return typesRequired.Trim().TrimEnd('-');
            }
        }
       
    }
    [TableName("tjc_expert_sequence_by_template")]
    //setup the primary key for table
    internal class TemplateSequence
    {
        public int TemplateID { get; set; }
        public int Sequence { get; set; }
        public int NumberRequired { get; set; }
        [IgnoreColumn]
        public string HeaderTypes
        {
            get
            {
                string headerTypes = string.Format("Requirement #{0}: ", Sequence);
                headerTypes += string.Format("Select {0} of the following ( ", NumberRequired);
                var ctl = new TemplateController();
                IEnumerable<Type> types = ctl.GetTemplateTypeTypesBySequence(TemplateID, Sequence);
                foreach (Type type in types)
                {
                    headerTypes += type.TypeName + " or ";
                }
                return headerTypes.Trim().TrimEnd('r').TrimEnd('o') + ")";
            }
        }
    }
    [TableName("tjc_expert_type_by_template")]
    //setup the primary key for table
    internal class TemplateType
    {
        public int TemplateID { get; set; }
        public int TypeID { get; set; }
        public int Sequence { get; set; }
    }
    internal class TemplateRequirement
    {
        public int NumberRequired { get; set; }
        public List<Type> Types { get; set; }
        public int Sequence { get; set; }
    }
}
