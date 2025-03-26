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

namespace tjc.Modules.PretrialServices.Sarasota.Components
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
        public IEnumerable<DefendantInProgram> GetDefendantsInProgramByCaseNumber( string caseNumber)
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Find("Where CaseNumber like @0", string.Format("%{0}%",caseNumber));
            }
            return t;
        }
        public IEnumerable<DefendantInProgram> GetDefendantsInProgramByDefendantName(string defendantName)
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Find("Where DefendantName like @0", string.Format("%{0}%", defendantName));
            }
            return t;
        }
        public IEnumerable<DefendantInProgram> GetDefendantsInProgramByDate(DateTime intakeDate)
        {
            IEnumerable<DefendantInProgram> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DefendantInProgram>();
                t = rep.Find("Where IntakeDate=@0" ,intakeDate);
            }
            return t;
        }
        
        public IEnumerable<int> GetYears()
        {
            IEnumerable<int> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<int>(System.Data.CommandType.Text, "Select Distinct Year(IntakeDate) as Year From tjc_pts_sarasota_defendants_in_program Order by 1");
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
