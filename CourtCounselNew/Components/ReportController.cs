/*
' Copyright (c) 2022 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtCounsel.Components
{
    internal class ReportController
    {
        

        public IEnumerable<CaseTypeCount> GetCaseTypeCounts(ReportQueryParameters reportQueryParameters)
        {
            List<CaseTypeCount> caseTypeList = new List<CaseTypeCount>();
            using (IDataContext ctx = DataContext.Instance())
            {
                var ctlCaseType = new CaseTypeController();
                IEnumerable<CaseType> caseTypes = ctlCaseType.GetCaseTypes();

                string sqlCaseDetail = "Select ct.CaseTypeId, ct.CaseTypeName, a.MotionFiled," +
                    " a.DateReceived, l.Description As CaseName, l.CaseNumber, " +
                    "p.PhaseName, IsNull(m.FirstName + ' ', '') + IsNull(m.LastName, '') As Responsible " +
                    "From tjc_cc_assignments a " +
                        "Inner Join tjc_cc_case_types ct On a.CaseTypeId = ct.CaseTypeId " +
                        "Inner Join tjc_cc_log_entries l On a.LogId = l.LogId " +
                        "Left Outer Join tjc_cc_members m On m.MemberId = a.CurrentAttorneyId " +
                        "Left Outer Join tjc_cc_phases p On p.PhaseId = a.PhaseId ";
                string sqlWhereClause = "";

                StatusTypes statusType = (StatusTypes)reportQueryParameters.Status;
                switch (statusType)
                {
                    case StatusTypes.active:
                        sqlWhereClause = " Where a.DateReceived >= @0 And a.DateReceived <= @1 And a.StatusTypeId=0";
                        break;
                    case StatusTypes.pending:
                        sqlWhereClause = " Where a.DateReceived >= GetDate() And a.DateReceived <= @1 And a.StatusTypeId=1";

                        break;
                    case StatusTypes.closed:
                        sqlWhereClause = " Where a.DateCompleted >= @0 And a.DateCompleted <= @1 And a.StatusTypeId=2";

                        break;
                    default:
                        sqlWhereClause = " Where ((a.DateReceived >= @0 And a.DateReceived <= @1) Or (a.DateCompleted >= @0 And a.DateCompleted <= @1))";

                        break;
                }
                if (reportQueryParameters.County > 0)
                {
                    sqlWhereClause += " And l.CountyId = @2";
                }
                if (reportQueryParameters.Phase > 0)
                {
                    sqlWhereClause += " And a.PhaseId =@4";
                }
                if (reportQueryParameters.Requestor > 0)
                {
                    sqlCaseDetail += " Left Outer Join tjc_cc_judge_assignments ja On ja.AssignmentId = a.AssignmentId";
                    sqlWhereClause += " And ja.JudgeId = @3";
                }

                if (!string.IsNullOrEmpty(reportQueryParameters.AttorneyList))
                {
                    sqlWhereClause += $" And a.CurrentAttorneyId In ({reportQueryParameters.AttorneyList})";
                }
                
                string sqlQuery = sqlCaseDetail + sqlWhereClause;
                IEnumerable<CaseDetail> caseDetails=  ctx.ExecuteQuery<CaseDetail>(System.Data.CommandType.Text, sqlQuery, reportQueryParameters.StartDate, reportQueryParameters.EndDate, reportQueryParameters.County, reportQueryParameters.Requestor, reportQueryParameters.Phase, reportQueryParameters.Status);

                IEnumerable<CaseTypeCount> caseTypeCounts = caseDetails.GroupBy(g => new { g.CaseTypeId, g.CaseTypeName }).Select(group => new CaseTypeCount { CaseTypeId = group.Key.CaseTypeId, CaseTypeName = group.Key.CaseTypeName, Count = group.Count() });
                if (reportQueryParameters.Details == 1)
                {
                    foreach (CaseTypeCount caseTypeItem in caseTypeCounts)
                    {
                        IEnumerable<CaseDetail> details = caseDetails.Where(c => c.CaseTypeId == caseTypeItem.CaseTypeId).Select(cd => new CaseDetail { MotionFiled = cd.MotionFiled, DateReceived = cd.DateReceived, CaseName = cd.CaseName, CaseNumber = cd.CaseNumber, PhaseName = cd.PhaseName, Responsible = cd.Responsible });
                        caseTypeItem.CaseDetails = details.OrderBy(d => d.DateReceived);
                        caseTypeList.Add(caseTypeItem);
                    }
                }
                else
                {
                    caseTypeList = caseTypeCounts.ToList();
                }
                List<int> existingCaseTypeIds = caseTypes.Select(ct => ct.CaseTypeId).Except(caseTypeCounts.Select(ct => ct.CaseTypeId)).ToList();
                foreach (CaseType caseType in caseTypes.Where(ct => existingCaseTypeIds.Contains(ct.CaseTypeId)))
                {
                    caseTypeList.Add(new CaseTypeCount { CaseTypeId = caseType.CaseTypeId, CaseTypeName = caseType.CaseTypeName, Count = 0 });
                }
            }
            return caseTypeList.OrderBy(ct => ct.CaseTypeName);
        }
    }
}
