/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
*/

using System;
using System.Collections.Generic;

namespace tjc.Modules.ExpertWitness.Components.Api
{
    // Public DTOs for the Web API JSON contract. The internal data entities
    // (Type, Location, RequestListItem, ...) can't be used directly: they're
    // internal, inherit the internal ExpertBase, and carry computed [IgnoreColumn]
    // getters that would hit the database during serialization.

    public class TypeDto
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
    }

    public class LocationDto
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
    }

    public class RequestListDto
    {
        public int RequestID { get; set; }
        public string CaseNumber { get; set; }
        public string LocationName { get; set; }
        public string TemplateName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class RequestRequirementDto
    {
        public int Sequence { get; set; }
        public int NumberRequired { get; set; }
        public string Types { get; set; }
    }

    public class RequestExpertDto
    {
        public int ExpertID { get; set; }
        public int Sequence { get; set; }
        public string Description { get; set; }
    }

    public class RequestDetailDto
    {
        public int RequestID { get; set; }
        public string CaseNumber { get; set; }
        public string LocationName { get; set; }
        public string TemplateName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<RequestRequirementDto> Requirements { get; set; }
        public List<RequestExpertDto> Experts { get; set; }
    }
}
