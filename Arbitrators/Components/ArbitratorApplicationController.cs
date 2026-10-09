/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.Arbitrators.Components
{
    /// <summary>
    /// Data access for arbitrator applications. The records live in the
    /// jud12.flcourts.org database (where the public Arbitrators module writes
    /// them), so this controller uses the "Jud12" connection rather than the
    /// intranet site's default SiteSqlServer connection - same pattern the
    /// CDSP and CourtRegistry modules use.
    /// </summary>
    public class ArbitratorApplicationController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public ArbitratorApplicationController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<ArbitratorApplication> GetApplications()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ArbitratorApplication>();
                return rep.Get().OrderByDescending(a => a.CreatedOnDate).ToList();
            }
        }

        public ArbitratorApplication GetApplication(int arbitratorApplicationId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ArbitratorApplication>();
                return rep.GetById(arbitratorApplicationId);
            }
        }

        public void UpdateApplication(ArbitratorApplication application)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<ArbitratorApplication>();
                rep.Update(application);
            }
        }

        /// <summary>
        /// Looks up the applicant's verified login email from the jud12.flcourts.org
        /// portal's own Users table (the application's UserId is a user on that
        /// portal, not on this intranet site, so DNN's UserController can't resolve
        /// it here - this app is a separate DNN installation). Falls back to the
        /// free-text business Email captured on the application if the lookup
        /// fails or the schema/qualifier assumption below doesn't match.
        /// </summary>
        public string GetApplicantNotificationEmail(ArbitratorApplication application)
        {
            if (application == null) return null;
            try
            {
                using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
                {
                    string email = ctx.ExecuteScalar<string>(CommandType.Text,
                        "SELECT Email FROM Users WHERE UserID = @0", application.UserId);
                    if (!string.IsNullOrWhiteSpace(email))
                        return email;
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
            return application.Email;
        }
    }
}
