// Note: This is a conversion of the PHP extend_calendar function into a DNN 9.11 module using DAL2 (PetaPoco-based).
// Assumptions:
// - The DNN module is named "CourtCalendarModule" (adjust as needed).
// - Database tables are created without prefixes for simplicity; in a real DNN module, you might prefix with the module name or use DNN's table naming conventions.
// - Models are POCOs with DAL2 attributes.
// - Relations are handled via manual fetches (no EF-style navigation properties).
// - Dates use DateTime; time zones are not handled (assume UTC or server time, adjust if needed).
// - The 'order' column in court_template_order is assumed to exist based on PHP code usage (even if not in fillable).
// - API endpoint is POST /api/Calendar/ExtendCalendar, secured with DnnAuthorize.
// - No full module manifest or installation scripts included; focus on requested components.
// - SQL statements are provided at the end for creating relevant tables (inferred from PHP models).
// - Add necessary NuGet packages: DotNetNuke.Core, DotNetNuke.Web, etc.
// - Place files in DesktopModules/CourtCalendarModule/Controllers, Models, etc.

// Models/CourtTemplateOrder.cs
using DocumentFormat.OpenXml.Office2016.Excel;
using DotNetNuke.Data;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

[TableName("court_template_order")]
[PrimaryKey("id", AutoIncrement = true)]
public class CourtTemplateOrder
{
    public int id { get; set; }
    public int court_id { get; set; }
    public DateTime date { get; set; }
    public bool auto { get; set; }
    public int? template_id { get; set; }
    public int order { get; set; } // Assumed based on PHP orderby('order')
}

[TableName("court_templates")]
[PrimaryKey("id", AutoIncrement = true)]
public class Template
{
    public int id { get; set; }
    public string name { get; set; }
    public int court_id { get; set; }
    public int? old_id { get; set; }
}

[TableName("template_timeslots")]
[PrimaryKey("id", AutoIncrement = true)]
public class TemplateTimeslot
{
    public int id { get; set; }
    public int court_template_id { get; set; }
    public int day { get; set; } // 1-5 for weekdays
    public DateTime start { get; set; } // Stored as datetime, but use TimeOfDay
    public DateTime end { get; set; }
    public string description { get; set; }
    public bool allDay { get; set; }
    public int duration { get; set; }
    public int quantity { get; set; }
    public bool blocked { get; set; }
    public bool public_block { get; set; }
    public string block_reason { get; set; }
    public int? category_id { get; set; }
}


[TableName("timeslots")]
[PrimaryKey("id", AutoIncrement = true)]
public class Timeslot
{
    public int id { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }
    public string description { get; set; }
    public bool allDay { get; set; }
    public int duration { get; set; }
    public int quantity { get; set; }
    public bool blocked { get; set; }
    public bool public_block { get; set; }
    public string block_reason { get; set; }
    public int? category_id { get; set; }
    public int? template_id { get; set; }
}

[TableName("court_timeslots")]
[PrimaryKey("id", AutoIncrement = true)]
public class CourtTimeslot
{
    public int id { get; set; }
    public int court_id { get; set; }
    public int timeslot_id { get; set; }
}
[TableName("holidays")]
[PrimaryKey("id", AutoIncrement = true)]
public class Holiday
{
    public int id { get; set; }
    public DateTime date { get; set; }
    // Add other fields if needed, e.g., name
}

[TableName("courts")]
[PrimaryKey("id", AutoIncrement = true)]
public class Court
{
    public int id { get; set; }
    // Other fields as per PHP model
}

// DTO for API request
public class ExtendRequest
{
    public int CourtId { get; set; }
    public int Weeks { get; set; }
    public int StartTemplate { get; set; }
    public DateTime StartDate { get; set; }
}


