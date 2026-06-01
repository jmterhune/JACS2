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
    internal class SettingController
    {
        private const string CONN_JUD12 = "Jud12"; //Connection
        public void CreateSetting(Setting t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Setting>();
                rep.Insert(t);
            }
        }

        public void DeleteSetting(int settingId)
        {
            var t = GetSetting(settingId);
            DeleteSetting(t);
        }

        public void DeleteSetting(Setting t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Setting>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Setting> GetSettings()
        {
            IEnumerable<Setting> t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Setting>();
                t = rep.Get();
            }
            return t;
        }

        public Setting GetSetting(int settingId)
        {
            Setting t;
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Setting>();
                t = rep.GetById(settingId);
            }
            return t;
        }

        public void UpdateSetting(Setting t)
        {
            using (IDataContext ctx =DataContext.Instance(CONN_JUD12))
            {
                var rep = ctx.GetRepository<Setting>();
                rep.Update(t);
            }
        }

    }
}
