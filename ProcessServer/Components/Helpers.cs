using System;

namespace tjc.Modules.ProcessServer.Components
{
    public static class Helpers
    {
        public static bool IsInEnrollmentPeriod(string beginDate, string endDate)
        {
            DateTime today = DateTime.Today;
            DateTime.TryParse(beginDate, out DateTime dateOne);
            DateTime.TryParse(endDate, out DateTime dateTwo);
            return today >= OnCurrentYear(dateOne) && today <= OnCurrentYear(dateTwo);
        }

        /// <summary>
        /// Returns the same month and day on the current year. The enrollment period is stored as a
        /// full date but repeats every year, so the stored year is ignored and the window no longer
        /// has to be re-entered each year.
        /// </summary>
        public static DateTime OnCurrentYear(DateTime stored)
        {
            if (stored == DateTime.MinValue)
                return stored;

            int year = DateTime.Today.Year;
            //Feb 29 would throw on a non leap year
            int day = Math.Min(stored.Day, DateTime.DaysInMonth(year, stored.Month));
            return new DateTime(year, stored.Month, day);
        }
    }
}