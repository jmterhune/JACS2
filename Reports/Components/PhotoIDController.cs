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

namespace tjc.Modules.Reports.Components
{
    internal class PhotoIDController
    {
        private const string CONN_DATACARD = "DataCard"; //Connection
        public void CreatePhotoID(PhotoID t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_DATACARD))
            {
                var rep = ctx.GetRepository<PhotoID>();
                rep.Insert(t);
            }
        }

        public void DeletePhotoID(int id)
        {
            var t = GetPhotoID(id);
            DeletePhotoID(t);
        }

        public void DeletePhotoID(PhotoID t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_DATACARD))
            {
                var rep = ctx.GetRepository<PhotoID>();
                rep.Delete(t);
            }
        }

        public IEnumerable<PhotoID> GetPhotoIDs()
        {
            IEnumerable<PhotoID> t;
            using (IDataContext ctx = DataContext.Instance(CONN_DATACARD))
            {
                var rep = ctx.GetRepository<PhotoID>();
                t = rep.Get();
            }
            return t;
        }    

        public PhotoID GetPhotoID(long id)
        {
            PhotoID t;
            using (IDataContext ctx = DataContext.Instance(CONN_DATACARD))
            {
                var rep = ctx.GetRepository<PhotoID>();
                t = rep.GetById(id);
            }
            return t;
        }

        public void UpdatePhotoID(PhotoID t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_DATACARD))
            {
                var rep = ctx.GetRepository<PhotoID>();
                rep.Update(t);
            }
        }
    }
}
