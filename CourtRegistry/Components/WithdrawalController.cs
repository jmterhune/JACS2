/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtRegistry.Components
{
    internal class WithdrawalController
    {
        private readonly IHostSettings _hostSettings;

        public WithdrawalController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        private const string CONN_JUD12 = "Jud12"; //Connection

        private const string ListSelect =
            @"SELECT w.WithdrawalId, w.AttorneyId, w.ApplicationId, ISNULL(w.[Year], 0) AS [Year], w.Status, w.Reason,
                     w.Signature, w.Acknowledged, w.DateRequested, w.DateReviewed, w.ReviewNote, w.EffectiveDate,
                     atty.BarNumber, atty.FirstName, atty.LastName, atty.Email,
                     a.Status AS ApplicationStatus
              FROM tjc_car_withdrawal_requests w
              JOIN tjc_car_attorneys atty ON w.AttorneyId = atty.AttorneyId
              LEFT JOIN tjc_car_applications a ON w.ApplicationId = a.ApplicationId ";

        /// <summary>Requests for the review list; pass -1 for every status.</summary>
        public IEnumerable<WithdrawalListItem> GetWithdrawalRequests(int status)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteQuery<WithdrawalListItem>(System.Data.CommandType.Text,
                    ListSelect + @"WHERE (@0 < 0 OR w.Status = @0)
                                   ORDER BY CASE WHEN w.Status = 0 THEN 0 ELSE 1 END, w.DateRequested DESC",
                    status);
            }
        }

        public WithdrawalListItem GetWithdrawalRequest(int withdrawalId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteSingleOrDefault<WithdrawalListItem>(System.Data.CommandType.Text,
                    ListSelect + "WHERE w.WithdrawalId = @0", withdrawalId);
            }
        }

        /// <summary>The attorney's open request, used to flag their application screen.</summary>
        public WithdrawalListItem GetPendingForAttorney(int attorneyId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteSingleOrDefault<WithdrawalListItem>(System.Data.CommandType.Text,
                    ListSelect + @"WHERE w.AttorneyId = @0 AND w.Status = @1
                                   ORDER BY w.WithdrawalId DESC",
                    attorneyId, (int)WithdrawalStatus.Pending);
            }
        }

        public int GetPendingCount()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteScalar<int>(System.Data.CommandType.Text,
                    "SELECT COUNT(*) FROM tjc_car_withdrawal_requests WHERE Status = @0",
                    (int)WithdrawalStatus.Pending);
            }
        }

        /// <summary>
        /// Takes the attorney off the registry for future appointments. Their
        /// applications for the requested fiscal year and later are archived, which
        /// is what keeps them out of the JAC export and the registry reports; nothing
        /// is deleted, so the approved case categories stay readable on the
        /// application screen.
        /// </summary>
        public void Approve(int withdrawalId, int reviewedByUserId, string note, DateTime effectiveDate)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.BeginTransaction();
                try
                {
                    ctx.Execute(System.Data.CommandType.Text,
                        @"UPDATE tjc_car_withdrawal_requests
                          SET Status = @1, DateReviewed = @2, ReviewedByUserId = @3, ReviewNote = @4, EffectiveDate = @5
                          WHERE WithdrawalId = @0",
                        withdrawalId, (int)WithdrawalStatus.Approved, DateTime.Now, reviewedByUserId, note, effectiveDate);

                    ctx.Execute(System.Data.CommandType.Text,
                        @"UPDATE a
                          SET a.Status = @1, a.DateReviewed = @2, a.LastModifiedByUserId = @3
                          FROM tjc_car_applications a
                          JOIN tjc_car_withdrawal_requests w ON a.AttorneyId = w.AttorneyId
                          WHERE w.WithdrawalId = @0
                            AND a.[Year] >= ISNULL(w.[Year], 0)
                            AND ISNULL(a.Status, 0) <> @1",
                        withdrawalId, (int)ApplicationStatus.Archived, DateTime.Now, reviewedByUserId);

                    ctx.Commit();
                }
                catch
                {
                    ctx.RollbackTransaction();
                    throw;
                }
            }
        }

        /// <summary>Leaves the attorney on the registry; the note is shown to them.</summary>
        public void Deny(int withdrawalId, int reviewedByUserId, string note)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    @"UPDATE tjc_car_withdrawal_requests
                      SET Status = @1, DateReviewed = @2, ReviewedByUserId = @3, ReviewNote = @4
                      WHERE WithdrawalId = @0",
                    withdrawalId, (int)WithdrawalStatus.Denied, DateTime.Now, reviewedByUserId, note);
            }
        }

        /// <summary>Case categories the attorney held when the request was made.</summary>
        public IEnumerable<ApplicationJacCodeDetail> GetAffectedJacCodes(int applicationId)
        {
            if (applicationId <= 0)
            {
                return Enumerable.Empty<ApplicationJacCodeDetail>();
            }
            return new ApplicationController(_hostSettings).GetApplicationJacCodes(applicationId);
        }
    }
}
