/*
' Copyright (c) 2025 Joe Terhune
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

namespace tjc.Modules.CourtRegistry.Components
{
    internal class LocationController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateLocation(Location t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Location>();
                rep.Insert(t);
            }
        }

        public void DeleteLocation(int locationId)
        {
            var t = GetLocation(locationId);
            DeleteLocation(t);
        }

        public void DeleteLocation(Location t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Location>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Location> GetLocations()
        {
            IEnumerable<Location> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Location>();
                t = rep.Get();
            }
            return t;
        }

        public Location GetLocation(int locationId)
        {
            Location t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Location>();
                t = rep.GetById(locationId);
            }
            return t;
        }

        public void UpdateLocation(Location t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Location>();
                rep.Update(t);
            }
        }

    }
}
