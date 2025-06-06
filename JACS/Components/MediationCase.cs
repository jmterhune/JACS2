using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_cases")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationCases", CacheItemPriority.Default, 20)]
    internal class MediationCase
    {
        public long id { get; set; }
        public string c_caseno { get; set; }
        public long? c_div { get; set; }
        public long? c_Pltf_a_id { get; set; }
        public long? c_def_a_id { get; set; }
        public string c_type { get; set; }
        public string c_otherm_text { get; set; }
        public string c_cmmts { get; set; }
        public string c_sch_notes { get; set; }
        public long location_type_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool injunction { get; set; }
        public bool petitioner { get; set; }
        public bool respondent { get; set; }
        public bool previous { get; set; }
        public string previous_case_num { get; set; }
        public string origin { get; set; }
        public string previous_case_tel { get; set; }
        public string previous_case_email { get; set; }
        public string p_signature { get; set; }
        public string d_signature { get; set; }
        public bool approved { get; set; }
        public DateTime? deleted_at { get; set; }
        public string form_type { get; set; }
        public string gal { get; set; }
        public string gal_tel { get; set; }
        public string gal_add { get; set; }
        public string gal_email { get; set; }
        public string f_issues { get; set; }
        public decimal? e_pltf_chg { get; set; }
        public decimal? e_pltf_annl_chg { get; set; }
        public decimal? e_def_chg { get; set; }
        public decimal? e_def_annl_chg { get; set; }
        public string f_issues_other_notes { get; set; }
        public string approval_reason { get; set; }
        public string cancel_reason { get; set; }
        public string availability { get; set; }
    }
}