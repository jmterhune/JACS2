/*
' Copyright (c) 2026 12th Judicial Circuit Court
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
    internal class EmailTemplateController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public EmailTemplateController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<EmailTemplate> GetTemplates()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteQuery<EmailTemplate>(System.Data.CommandType.Text,
                    "SELECT * FROM tjc_car_email_templates ORDER BY TemplateName");
            }
        }

        public EmailTemplate GetTemplate(int templateId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailTemplate>();
                return rep.GetById(templateId);
            }
        }

        /// <summary>
        /// The template the sending code asks for, or null when there is no row, the row
        /// is switched off, or the table is not there yet.
        ///
        /// Missing is not an error on purpose: the module files can be copied to a site
        /// before the table script is run, and every caller falls back to its built-in
        /// wording, so an attorney still gets a correct email either way.
        /// </summary>
        public EmailTemplate GetTemplateByKey(string templateKey)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
                {
                    EmailTemplate t = ctx.ExecuteQuery<EmailTemplate>(System.Data.CommandType.Text,
                        "SELECT TOP 1 * FROM tjc_car_email_templates WHERE TemplateKey = @0", templateKey)
                        .FirstOrDefault();

                    if (t == null || !t.IsEnabled || string.IsNullOrEmpty(t.Body))
                    {
                        return null;
                    }
                    return t;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateTemplate(EmailTemplate t, int modifiedByUserId)
        {
            t.DateModified = DateTime.Now;
            t.ModifiedByUserId = modifiedByUserId;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<EmailTemplate>();
                rep.Update(t);
            }
        }
    }
}
