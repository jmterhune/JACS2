/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using tjc.Modules.ExpertWitness.Components;
namespace tjc.Modules.ExpertWitness
{
    public partial class View : ExpertWitnessModuleBase
    {
        #region Properties
        private readonly INavigationManager _navigationManager;
        private string _requestedID = string.Empty;
        private string _currentSequence = string.Empty;
        private string[] _addedExperts = new string[0];
        // Width of the shuffle window, in rotation "positions". Two experts whose
        // last-used positions are within this distance can swap order randomly, so the
        // surfaced set varies between requests while least-recently-used experts stay to
        // the front (fair rotation + shuffle).
        private const double ShuffleWindowPositions = 4d;
        private int _notifyKey;

        public Guid GlobalID
        {
            get
            {
                if (ViewState["guid"] != null)
                    return Guid.Parse(ViewState["guid"].ToString());
                else
                    return Guid.Empty;
            }
            set
            {
                ViewState["guid"] = value;
            }
        }
        internal List<AddedExpert> AddedExpertsList
        {
            get
            {
                if (ViewState["AddedExperts"] != null)
                    return (List<AddedExpert>)ViewState["AddedExperts"];
                else
                    return new List<AddedExpert>();
            }
            set
            {
                ViewState["AddedExperts"] = value;
            }
        }

        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (IsAdmin)
                    {
                        lnkAdmin.Visible = true;
                        lnkAdmin.NavigateUrl = EditUrl("request");
                    }
                    GlobalID = Guid.NewGuid();
                    BindLists();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            string errorMessage = "";
            var tCtl = new TemplateController();
            var eCtl = new ExpertController();
            var rtCtl = new RequestedTemplateController();
            var rCtl = new RequestController();
            Int32.TryParse(drpEvaluation.SelectedValue, out int TemplateID);
            Template objTemplate = tCtl.GetTemplate(TemplateID);
            IEnumerable<RequestCart> SelectedExperts = rtCtl.GetRequestedTemplatesByGuidByStatus(GlobalID, Convert.ToInt32(RequestStatus.selected));
            IEnumerable<TemplateSequence> templateSequences = tCtl.GetTemplateSequences(TemplateID);
            foreach (TemplateSequence s in templateSequences)
            {
                int count = SelectedExperts.Where(t => t.Sequence == s.Sequence).Count();
                if (count < s.NumberRequired)
                {
                    int difference = s.NumberRequired - count;
                    errorMessage += "<p>Requirement #" + s.Sequence + " was not met. Please choose " + difference + " more expert(s).";
                }
            }
            if (errorMessage == "")
            {
                Request objRequest = new Request
                {
                    CaseNumber = txtCaseNumber.Text,
                    CreatedBy = UserInfo.Email,
                    CreatedDate = DateTime.Now,
                    ModifiedBy=UserInfo.Email,
                    ModifiedDate=DateTime.Now,
                    LocationID = Int32.Parse(drpLocation.SelectedValue),
                    TemplateID = Int32.Parse(drpEvaluation.SelectedValue)
                };
                rCtl.CreateRequest(objRequest);
                var persistedExperts = new HashSet<int>();
                foreach (var exp in SelectedExperts)
                {
                    // Never write the same expert to a request more than once
                    if (!persistedExperts.Add(exp.ExpertID))
                        continue;
                    ExpertTemplate objExpertTemplate = eCtl.GetExpertTemplate(exp.ExpertID, exp.TemplateID);
                    if (exp.CurrentOrder != objExpertTemplate.Position)
                    {
                        errorMessage = "<p>Expert " + exp.ExpertName + " has already been selected by another user.</p><p>Please Reset the form and try again</p>";
                        break;
                    }
                    eCtl.CreateExpertRequest(exp.ExpertID, objRequest.RequestID, exp.Sequence);
                    // Persist the rotation so this expert moves to the back of the line
                    // and a different expert surfaces next time.
                    objExpertTemplate.Position = GetNewPosition(exp.TemplateID);
                    eCtl.UpdateExpertTemplate(objExpertTemplate);
                }
                if (errorMessage != "")
                {
                    rtCtl.DeleteRequestTemplatesByGuid(GlobalID);
                    Notify("error", "Request Failed!", errorMessage);
                }
                else
                {
                    rtCtl.DeleteRequestTemplatesByGuid(GlobalID);
                    cmdUpdate.Enabled = false;
                    rptExpertSelection.Visible = false;

                    Notify("success", "Success!", "Your request has been added successfully.");
                }
            }
            else
                Notify("warning", "Requirements Not Fulfilled", errorMessage);
        }
        protected void cmdReset_Click(object sender, EventArgs e)
        {
            var rtCtl = new RequestedTemplateController();
            rtCtl.DeleteRequestTemplatesByGuid(GlobalID);
            Response.Redirect(_navigationManager.NavigateURL(), true);
        }
        protected void rptExpertSelection_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RequestedTemplate objRequestedTemplate = (RequestedTemplate)e.Item.DataItem;
                Literal ltViewComment = (Literal)e.Item.FindControl("ltViewComment");
                if (string.IsNullOrEmpty(objRequestedTemplate.Comments))
                    ltViewComment.Visible = false;
                else
                    ltViewComment.Text = string.Format("<a class=\"btn btn-default me-2 view-comment\" data-comment=\"{0}\"><i class=\"fas fa-search\"></i> View Comment</a>", objRequestedTemplate.Comments);
                HtmlGenericControl divContainer = (HtmlGenericControl)e.Item.FindControl("divContainer");
                HtmlGenericControl divHeader = (HtmlGenericControl)e.Item.FindControl("divHeader");
                LinkButton cmdSelect = (LinkButton)e.Item.FindControl("cmdSelect");
                LinkButton cmdPass = (LinkButton)e.Item.FindControl("cmdPass");
                LinkButton cmdAddExpert = (LinkButton)e.Item.FindControl("cmdAddExpert");
                string sequence = objRequestedTemplate.Sequence.ToString();
                Literal ltTypeHeader = (Literal)e.Item.FindControl("ltTypeHeader");
                if (_currentSequence != sequence)
                {
                    ltTypeHeader.Text = "<h5 class='d-inline-block text-white m-0'>" + objRequestedTemplate.HeaderTypes + "</h5>";
                    _currentSequence = sequence;
                }
                else
                    divHeader.Visible = false;
                if (objRequestedTemplate.Status == Convert.ToInt32(RequestStatus.passed))
                {
                    divContainer.Attributes["class"] = "expertName bg-danger rounded m-3 p-1 ps-3 text-white";
                    cmdPass.Visible = false;
                }
                if (objRequestedTemplate.Status == Convert.ToInt32(RequestStatus.selected))
                {
                    divContainer.Attributes["class"] = "expertName bg-success rounded m-3 p-1 ps-3 text-white";
                    cmdSelect.Visible = false;
                    cmdPass.Visible = false;
                }
            }
        }
        protected void rptExpertSelection_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            HiddenField hdSequence = e.Item.FindControl("hdSequence") as HiddenField;
            int seq = Convert.ToInt32(hdSequence.Value);
            if (e.CommandName == "add")
            {
                try
                {
                    if (AddExpert(seq))
                    {
                        AddAdditionalExpert(seq);
                        GetExperts();
                    }
                    else
                        Notify("warning", "Add Expert Failed", "No more experts remain related to the required specification.");
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
            if (e.CommandName == "select")
            {
                var ctl = new RequestedTemplateController();
                int expertId= Convert.ToInt32(e.CommandArgument); 
                RequestCart requestCart=ctl.GetRequestedTemplatesByExpertByGuidBySequence(expertId,GlobalID,seq);
                requestCart.Status = Convert.ToInt32(RequestStatus.selected);
                ctl.UpdateRequestedTemplate(requestCart);
                GetExperts();
            }
            if (e.CommandName == "pass")
            {
                int expertId = Convert.ToInt32(e.CommandArgument);
                var ctl = new RequestedTemplateController();
                RequestCart requestCart = ctl.GetRequestedTemplatesByExpertByGuidBySequence(expertId, GlobalID, seq);
                requestCart.Status = Convert.ToInt32(RequestStatus.passed);
                ctl.UpdateRequestedTemplate(requestCart);
                GetExperts();
            }
        }
        protected void rptExpertSelection_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdAddExpert = (LinkButton)e.Item.FindControl("cmdAddExpert");
                LinkButton cmdSelect = (LinkButton)e.Item.FindControl("cmdSelect");
                LinkButton cmdPass = (LinkButton)e.Item.FindControl("cmdPass");
                scriptMan.RegisterAsyncPostBackControl(cmdAddExpert);
                scriptMan.RegisterAsyncPostBackControl(cmdSelect);
                scriptMan.RegisterAsyncPostBackControl(cmdPass);
            }
        }
        protected void drpEvaluation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpLocation.SelectedValue != "")
            {
                // FillRequestCart()
                AddedExpertsList = null;
                GetExperts();
            }
        }
        protected void drpLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpEvaluation.SelectedValue != "")
            {
                // FillRequestCart()
                AddedExpertsList = null;
                GetExperts();
            }
        }
        #endregion
        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindLists()
        {
            var tCtl = new TemplateController();
            var lCtl = new LocationController();
            drpEvaluation.DataSource = tCtl.GetTemplates().OrderBy(t => t.TemplateName);
            drpEvaluation.DataTextField = "TemplateName";
            drpEvaluation.DataValueField = "TemplateID";
            drpEvaluation.DataBind();
            drpEvaluation.Items.Insert(0, new ListItem("<- Select Evaluation Type ->", ""));
            drpLocation.DataSource = lCtl.GetLocations().OrderBy(l => l.LocationName);
            drpLocation.DataTextField = "LocationName";
            drpLocation.DataValueField = "LocationID";
            drpLocation.DataBind();
            drpLocation.Items.Insert(0, new ListItem("<- Select Location ->", ""));
        }
        private void GetExperts()
        {
            List<RequestedTemplate> tempRequestedTemplate = new List<RequestedTemplate>();
            List<RequestedTemplate> listRequestedTemplate = new List<RequestedTemplate>();
            var tCtl = new TemplateController();
            var lCtl = new LocationController();
            var rCtl = new RequestedTemplateController();
            var eCtl = new ExpertController();
            Int32.TryParse(drpEvaluation.SelectedValue, out int templateId);
            Int32.TryParse(drpLocation.SelectedValue, out int locationId);
            Template objTemplate = tCtl.GetTemplate(templateId);
            Location objLocation = lCtl.GetLocation(locationId);
            List<RequestedTemplate> SelectedExperts = GetSelectedExperts().Select(s => new RequestedTemplate() { ExpertID = s.ExpertID, ExpertName = s.ExpertName, Guid = s.Guid, Comments = s.Comments, Header = "", Sequence = s.Sequence, Status = s.Status, TemplateID = s.TemplateID }).ToList();
            List<RequestedTemplate> PassedExperts = GetPassedExperts().Select(et => new RequestedTemplate() { ExpertID = et.ExpertID, ExpertName = et.ExpertName, Comments = et.Comments, TemplateID = et.TemplateID, Header = "", Sequence = et.Sequence, Guid = et.Guid, Status = et.Status }).ToList();
            // Every expert already selected anywhere in this request – excluded from every
            // requirement list so the same expert can never be selected twice.
            var usedExpertIds = new HashSet<int>(SelectedExperts.Select(e => e.ExpertID));
            string errorMessage = "";
            FillRequestCart();
            foreach (TemplateSequence tt in tCtl.GetTemplateSequences(templateId).OrderBy(o => o.Sequence))
            {
                int countSelected = SelectedExperts.Where(e => e.Sequence == tt.Sequence).Count();
                int numberRemaining = tt.NumberRequired - countSelected;
                var listSelectedExperts = SelectedExperts.Where(ep => ep.Sequence == tt.Sequence).Select(i => i.ExpertID);
                if (AddedExpertsList.Count > 0)
                {
                    AddedExpert addedexpert = AddedExpertsList.Where(exp => exp.Sequence == tt.Sequence).FirstOrDefault();
                    if (addedexpert != null)
                        numberRemaining += addedexpert.Count;
                }
                tempRequestedTemplate = rCtl.GetTemporaryRequestedTemplates(tt.TemplateID, tt.Sequence, locationId)
                   .Where(x => !listSelectedExperts.Contains(x.ExpertID) && !listRequestedTemplate.Select(lt => lt.ExpertID).Contains(x.ExpertID) && !usedExpertIds.Contains(x.ExpertID))
                   .Select(r => new RequestedTemplate() { ExpertID = r.ExpertID, ExpertName = r.ExpertName, Comments = r.Comments, TemplateID = templateId, NumberRequired = r.NumberRequired, Header = r.Header, Sequence = tt.Sequence, Guid = GlobalID, Status = Convert.ToInt32(RequestStatus.unselected), Position = r.Position }).Distinct().ToList().Except(PassedExperts, new RequestedTemplateComparer()).OrderBy(x => GetShuffleKey(x.ExpertID, x.Position)).Take(numberRemaining).ToList();

                if (tempRequestedTemplate.Count < numberRemaining)
                    errorMessage += "<li>There are not enough experts available to fulfill requirement # " + tt.Sequence.ToString() + "</li>";
                listRequestedTemplate.AddRange(tempRequestedTemplate);
                listRequestedTemplate.AddRange(SelectedExperts.Where(s => s.Sequence == tt.Sequence));
                listRequestedTemplate.AddRange(PassedExperts.Where(s => s.Sequence == tt.Sequence & !listSelectedExperts.Contains(s.ExpertID)));

                foreach (RequestedTemplate t in listRequestedTemplate.Where(b => b.Sequence == tt.Sequence))
                {
                    RequestCart objRequestCart = new RequestCart();
                    ExpertTemplate et = eCtl.GetExpertTemplate(t.ExpertID, t.TemplateID);
                    if (et != null)
                    {
                        objRequestCart.CurrentOrder = et.Position;
                        objRequestCart.ExpertID = t.ExpertID;
                        objRequestCart.ExpertName = t.ExpertName;
                        objRequestCart.Guid = GlobalID;
                        objRequestCart.Comments = t.Comments;
                        objRequestCart.Sequence = t.Sequence;
                        objRequestCart.Status = t.Status;
                        objRequestCart.TemplateID = t.TemplateID;
                        t.Header = tt.HeaderTypes;
                        rCtl.CreateRequestedTemplate(objRequestCart);
                    }
                }
            }
            if (errorMessage != "")
            {
                errorMessage = "<ul>" + errorMessage + "</ul><p>Reset the form and try again.</p>";
                Notify("error", "Template Requirements Were Not Met", errorMessage);
            }
            rptExpertSelection.DataSource = listRequestedTemplate.OrderBy(o => o.Sequence).ThenBy(o => o.Status).ThenBy(o => o.ExpertName);
            rptExpertSelection.DataSourceID = null;
            rptExpertSelection.DataBind();
        }
        private bool AddExpert(int sequence)
        {
            int templateId = Int32.Parse(drpEvaluation.SelectedValue);
            int locationId = Int32.Parse(drpLocation.SelectedValue);
            var ctl = new RequestedTemplateController();
            // Every expert already in this request (any sequence, any status) so we never
            // offer the same expert twice.
            var usedExpertIds = new HashSet<int>(ctl.GetRequestedTemplates(GlobalID).Select(i => i.ExpertID));
            return ctl.GetRequestedTemplatesByTemplateByLocationBySequence(templateId, locationId, sequence)
                      .Any(x => !usedExpertIds.Contains(x.ExpertID));
        }
        private double GetShuffleKey(int expertId, int position)
        {
            // Recency-weighted shuffle key: experts are ranked by their rotation Position
            // (least-recently-used first) but jittered by a deterministic amount so the
            // surfaced set varies between requests. The jitter is seeded from the page's
            // GlobalID + the expert, so the order stays stable while the user works on a
            // single request (passing/adding doesn't reshuffle everyone) yet differs on
            // each new page load.
            int seed = (GlobalID.GetHashCode() * 397) ^ expertId;
            double jitter = new Random(seed).NextDouble() * ShuffleWindowPositions;
            return position + jitter;
        }
        private void Notify(string type, string title, string message)
        {
            // Client-side toast/alert (SweetAlert2 + Noty) instead of a server-side
            // module message, so it works cleanly after an async (UpdatePanel) postback.
            string t = System.Web.HttpUtility.JavaScriptStringEncode(title ?? string.Empty);
            string m = System.Web.HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            string script = string.Format("ewNotify('{0}','{1}','{2}');", type, t, m);
            ScriptManager.RegisterStartupScript(pnlUpdate, pnlUpdate.GetType(), "ewNotify_" + (_notifyKey++), script, true);
        }
        private int GetNewPosition(int templateId)
        {
            var ctl = new ExpertController();
            int position = 0;
            var expertTemplates = ctl.GetExpertTemplates(templateId);
            if (expertTemplates.Count() > 0)
                position = expertTemplates.Max(p => p.Position) + 1;
            else
                position = 1;
            return position;
        }
        private void AddAdditionalExpert(int sequence)
        {
            List<AddedExpert> list = AddedExpertsList;
            bool updated = false;
            foreach (AddedExpert exp in list)
            {
                if (exp.Sequence == sequence)
                {
                    exp.Count += 1;
                    updated = true;
                }
            }
            if (!updated)
                list.Add(new AddedExpert { Sequence = sequence, Count = 1 });
            AddedExpertsList = list;

        }
        private void FillRequestCart()
        {
            var ctl = new RequestedTemplateController();
            // Delete Existing request items in cart
            IEnumerable<RequestCart> requestCarts = ctl.GetRequestedTemplates(GlobalID);
            foreach (var requestCart in requestCarts) { ctl.DeleteRequestedTemplate(requestCart); }
        }
        private void BindRepeater()
        {
            var ctl = new RequestedTemplateController();
            IEnumerable<RequestedTemplate> requestCarts = ctl.GetRequestedTemplatesByGuid(GlobalID);
            rptExpertSelection.DataSource = requestCarts.Select(x => new RequestedTemplate() { ExpertID = x.ExpertID, ExpertName = x.ExpertName, Comments = x.Comments, Guid = x.Guid, Header = x.HeaderTypes, NumberRequired = x.NumberRequired, RequestID = x.RequestID, Sequence = x.Sequence, Status = x.Status, TemplateID = x.TemplateID });
            rptExpertSelection.DataBind();
        }
        private IEnumerable<RequestCart> GetPassedExperts()
        {
            var ctl = new RequestedTemplateController();
            return ctl.GetRequestedTemplatesByGuidByStatus(GlobalID, Convert.ToInt32(RequestStatus.passed));
        }
        private IEnumerable<RequestCart> GetSelectedExperts()
        {
            var ctl = new RequestedTemplateController();
            return ctl.GetRequestedTemplatesByGuidByStatus(GlobalID, Convert.ToInt32(RequestStatus.selected));
        }
        #endregion
    }
}