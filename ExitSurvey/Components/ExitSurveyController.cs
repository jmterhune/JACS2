/*
' Copyright (c) 2026  Joe Terhune
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
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.ExitSurvey.Components
{
    public class ExitSurveyController
    {
        private readonly IHostSettings _hostSettings;

        public ExitSurveyController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        #region Response
        // Inserts the response and returns the identity assigned to it.
        public int CreateResponse(ExitSurveyResponse response)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<ExitSurveyResponse>();
                rep.Insert(response);
            }
            return response.ResponseID;
        }

        public ExitSurveyResponse GetResponse(int responseId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.GetRepository<ExitSurveyResponse>().GetById(responseId);
            }
        }

        public IEnumerable<ExitSurveyResponse> GetResponses(int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.GetRepository<ExitSurveyResponse>()
                    .Find("WHERE ModuleID = @0 ORDER BY CreatedOnDate DESC", moduleId);
            }
        }

        public void DeleteResponse(int responseId)
        {
            var response = GetResponse(responseId);
            if (response == null) return;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.GetRepository<ExitSurveyResponse>().Delete(response);
                ctx.GetRepository<ExitSurveyRating>().Delete("WHERE ResponseID = @0", responseId);
                ctx.GetRepository<ExitSurveyReason>().Delete("WHERE ResponseID = @0", responseId);
            }
        }
        #endregion

        #region Rating
        public void CreateRating(ExitSurveyRating rating)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.GetRepository<ExitSurveyRating>().Insert(rating);
            }
        }

        public IEnumerable<ExitSurveyRating> GetRatings(int responseId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.GetRepository<ExitSurveyRating>()
                    .Find("WHERE ResponseID = @0 ORDER BY RatingID", responseId);
            }
        }

        public IEnumerable<ExitSurveyRating> GetRatings(int responseId, string section)
        {
            return GetRatings(responseId).Where(r => r.Section == section);
        }
        #endregion

        #region Reason
        public void CreateReason(ExitSurveyReason reason)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                ctx.GetRepository<ExitSurveyReason>().Insert(reason);
            }
        }

        public IEnumerable<ExitSurveyReason> GetReasons(int responseId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.GetRepository<ExitSurveyReason>()
                    .Find("WHERE ResponseID = @0 ORDER BY ReasonID", responseId);
            }
        }

        public IEnumerable<ExitSurveyReason> GetReasons(int responseId, string section)
        {
            return GetReasons(responseId).Where(r => r.Section == section);
        }
        #endregion
    }
}
