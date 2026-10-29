/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// A logged-in public-site user's subscription to a calendar's change
    /// notifications. UserId is an account on the public (external) portal - the
    /// external site writes these rows. When a judge/JA sends an update
    /// notification from the intranet app, it resolves each subscriber's email
    /// from the external portal's Users table (via the "Jud12" connection).
    /// </summary>
    [TableName("tjc_cc_subscriptions")]
    [PrimaryKey("SubscriptionId", AutoIncrement = true)]
    public class Subscription
    {
        public int SubscriptionId { get; set; }

        public int CalendarId { get; set; }

        /// <summary>External (public portal) UserId of the subscriber.</summary>
        public int UserId { get; set; }

        public DateTime CreatedOnDate { get; set; }
    }
}
