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
using System.Collections.Generic;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class RaceController
    {
        public void CreateRace(Race t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Race>();
                rep.Insert(t);
            }
        }

        public void DeleteRace(int raceId)
        {
            var t = GetRace(raceId);
            DeleteRace(t);
        }

        public void DeleteRace(Race t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Race>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Race> GetRaces()
        {
            IEnumerable<Race> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Race>();
                t = rep.Get();
            }
            return t;
        }
        public Race GetRace(int raceId)
        {
            Race t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Race>();
                t = rep.GetById(raceId);
            }
            return t;
        }

        public void UpdateRace(Race t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Race>();
                rep.Update(t);
            }
        }

    }
}
