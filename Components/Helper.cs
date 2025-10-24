using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System;
using System.Collections;
using System.Globalization;

namespace tjc.Modules.jacs.Components
{
    public static class UserHelper
    {
        /// <summary>
        /// Retrieves a UserInfo object based on a profile property value
        /// </summary>
        /// <param name="portalId">The ID of the portal</param>
        /// <param name="propertyName">The name of the profile property (case-sensitive)</param>
        /// <param name="propertyValue">The value to match</param>
        /// <returns>UserInfo object if found, null if no match or error</returns>
        public static UserInfo GetUserByProfileProperty(int portalId, string propertyName, string propertyValue)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue))
                {
                    return null;
                }

                // Ensure portalId is valid
                portalId = PortalController.GetEffectivePortalId(portalId);

                // Get users by profile property
                int totalRecords = 0;
                ArrayList users =DotNetNuke.Entities.Users.UserController.GetUsersByProfileProperty(
                    portalId,
                    propertyName,
                    propertyValue,
                    0,  // pageIndex
                    1,  // pageSize (we only need one match)
                    ref totalRecords,
                    false,  // includeDeleted
                    false   // superUsersOnly
                );

                // Return first matching user, if any
                if (users != null && users.Count > 0)
                {
                    return users[0] as UserInfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log exception (in a production environment, use DNN logging)
                 DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return null;
            }
        }

    }
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the start date of the week containing the specified date, assuming the week starts on Monday.
        /// </summary>
        /// <param name="date">The date within the week.</param>
        /// <returns>The start date of the week (Monday) at 00:00:00.</returns>
        public static DateTime GetStartOfWeek(this DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }
        public static int GetWeekOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime firstWeekDay = firstDayOfMonth.AddDays(
                -(int)firstDayOfMonth.DayOfWeek + (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
            );

            // Adjust if the calculated first weekday falls outside the reasonable range for the month start
            if (firstWeekDay > firstDayOfMonth || firstWeekDay <= firstDayOfMonth.AddDays(-7))
            {
                firstWeekDay = firstWeekDay.AddDays(7);
            }

            return ((date - firstWeekDay).Days / 7) + 1;
        }
        /// <summary>
        /// Gets the start and end dates of the week containing the specified date.
        /// The week starts on Monday and ends on Sunday.
        /// </summary>
        /// <param name="date">The date within the week.</param>
        /// <returns>A tuple containing the start date (Monday at 00:00:00) and end date (Sunday at 23:59:59.9999999).</returns>
        public static (DateTime Start, DateTime End) GetWeekStartEnd(this DateTime date)
        {
            DateTime start = date.StartOfWeek();
            DateTime end = start.AddDays(7).AddTicks(-1);
            return (start, end);
        }
    }
}