namespace CourtCounsel.Desktop.Models;

public record StatusNameOption(string Text, string Group);

// The exact, hard-coded StatusName list from EditHistory.ascx's drpStatus
// (not database-driven). Group is "General" for the ungrouped block, then
// "Appeals" and "Commissioners Reports (Belated Appeal)" for the two
// optgroup-tagged blocks, matching the original markup verbatim.
public static class StatusNameOptions
{
    public static readonly IReadOnlyList<StatusNameOption> All = new List<StatusNameOption>
    {
        new("Admin Task Completed", "General"),
        new("Admin Review Needed", "General"),
        new("Amended Filed", "General"),
        new("Amended Motion Due", "General"),
        new("Assigned", "General"),
        new("Completed", "General"),
        new("EOT Filed", "General"),
        new("EOT Granted", "General"),
        new("Evidentiary Hearing Granted", "General"),
        new("Evidentiary Hearing Scheduled", "General"),
        new("Final Order Due", "General"),
        new("Follow up needed", "General"),
        new("Mandamus Petition Filed w/ 2nd", "General"),
        new("Motion to Hear and Rule filed", "General"),
        new("Motion Stricken With Leave to Amend", "General"),
        new("Motion Under Review", "General"),
        new("NOI I filed", "General"),
        new("NOI II filed", "General"),
        new("NOI III filed", "General"),
        new("Non-Final Order Entered", "General"),
        new("Order to Show Cause", "General"),
        new("Ordered to Respond", "General"),
        new("Post Conviction Counsel Appointed", "General"),
        new("Proposed Order Submitted", "General"),
        new("Response Due", "General"),
        new("Response Filed", "General"),
        new("Other", "General"),

        new("Fee Due", "Appeals"),
        new("Fee Order Issued", "Appeals"),
        new("Fee Paid", "Appeals"),
        new("Show Cause Order", "Appeals"),
        new("Initial Brief Filed", "Appeals"),
        new("Initial Brief Due", "Appeals"),
        new("Answer Brief Due", "Appeals"),
        new("Answer Brief Filed", "Appeals"),
        new("Reply Brief (Optional)", "Appeals"),
        new("Ready for Disposition", "Appeals"),

        new("Evidentiary Hearing Scheduled", "Commissioners Reports (Belated Appeal)"),
        new("Transcripts Ordered", "Commissioners Reports (Belated Appeal)"),
        new("Transcripts Received & Filed", "Commissioners Reports (Belated Appeal)"),
        new("Final Report Filed", "Commissioners Reports (Belated Appeal)"),
    };
}
