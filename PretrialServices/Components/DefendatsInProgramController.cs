/*
' Copyright (c) 2023 Joe Terhune
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
using System.Runtime.InteropServices.ComTypes;

namespace tjc.Modules.PretrialServices.Components
{
    internal class DefendantInProgramController
    {
        public void CreateDefendantInProgram(DefendantInProgram t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                rep.Insert(t);
            }
        }

        public void DeleteDefendantInProgram(int itemId)
        {
            var t = GetDefendantInProgram(itemId);
            DeleteDefendantInProgram(t);
        }

        public void DeleteDefendantInProgram(DefendantInProgram t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                rep.Delete(t);
            }
        }

        public IEnumerable<DefendantInProgram> GetDefendantsInProgram()
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<DefendantInProgram> GetDefendantsInProgram(DateTime startDate,DateTime endDate)
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Find("Where IntakeDate Between @0 AND @1",startDate,endDate);
            }
            return t;
        }
        public IEnumerable<DefendantInProgram> GetDefendantsInProgramByCaseNumber(int countyId, string caseNumber)
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Find("Where CountyId=@0 And CaseNumber like @1",countyId, string.Format("%{0}%",caseNumber));
            }
            return t;
        }
        public IEnumerable<DefendantInProgram> GetDefendantsInProgramByDefendantName(int countyId, string defendantName)
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Find("Where CountyId=@0 And DefendantName like @1",countyId, string.Format("%{0}%", defendantName));
            }
            return t;
        }
        public IEnumerable<DefendantInProgram> GetDefendantsInProgramByCounty(int countyId,DateTime intakeDate)
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Find("Where CountyId=@0 And IntakeDate=@1" ,countyId,intakeDate);
            }
            return t;
        }
        
        public IEnumerable<int> GetYears(int countyId)
        {
            IEnumerable<int> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<int>(System.Data.CommandType.Text, "Select Distinct Year(IntakeDate) as Year From tjc_pts_defendants_in_program Where countyid=@0 Order by 1",countyId);
            }

            return t;
        }

        public DefendantInProgram GetDefendantInProgram(long itemId)
        {
            DefendantInProgram t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.GetById(itemId);
            }
            return t;
        }

        public void UpdateDefendantInProgram(DefendantInProgram t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                rep.Update(t);
            }
        }

    }
}
