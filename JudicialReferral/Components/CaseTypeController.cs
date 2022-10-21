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

namespace tjc.Modules.JudicialReferral.Components
{
    internal class CaseTypeController
    {
        public void CreateCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Insert(t);
            }
        }

        public void DeleteCaseType(int caseTypeId)
        {
            var t = GetCaseType(caseTypeId);
            DeleteCaseType(t);
        }

        public void DeleteCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CaseType> GetCaseTypes()
        {
            IEnumerable<CaseType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                t = rep.Get();
            }
            return t;
        }
       
        public CaseType GetCaseType(int caseTypeId)
        {
            CaseType t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                t = rep.GetById(caseTypeId);
            }
            return t;
        }
        public void UpdateCaseType(CaseType t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseType>();
                rep.Update(t);
            }
        }
    }
}
