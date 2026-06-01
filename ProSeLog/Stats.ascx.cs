/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.ProSeLog.Components;

namespace tjc.Modules.ProSeLog
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ProSeLogModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Stats : ProSeLogModuleBase
    {
        #region Events


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (IsAdmin)
                    {
                        lnkManage.Visible = true;
                        lnkManage.NavigateUrl = CaseTypeListUrl;
                    }
                    var ctl = new CountyController();
                    drpCounty.DataSource = ctl.GetCounties();
                    drpCounty.DataBind();
                    for (var i = 1; i <= 12; i++)
                    {
                        string month = DateTime.Parse(i.ToString() + "/1/2007").ToString("MMM");
                        drpMonths.Items.Add(new ListItem(month, i.ToString()));
                    }
                    int year = 2004;
                    while (year <= DateTime.Now.Year + 1)
                    {
                        drpYear.Items.Add(new ListItem(year.ToString()));
                        year += 1;
                    }
                    drpMonths.SelectedValue = DateTime.Now.Month.ToString();
                    drpYear.SelectedValue = DateTime.Now.Year.ToString();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            var ctl = new HistoryController();
            IEnumerable<HistoryListItem> histories = ctl.GetStats(Int32.Parse(drpMonths.SelectedValue), Int32.Parse(drpYear.SelectedValue), Int32.Parse(drpCounty.SelectedValue));
            List<Stat> colstats = new List<Stat>();
            var cCtl = new ContactController();
            IEnumerable<Contact> contacts = cCtl.GetContacts();
            foreach (Contact contact in contacts)
            {
                colstats.Add(GetStat(histories, 1, contact.ContactName, contact.ContactID));
                //for (var i = 1; i <= 8; i++)
                //{
                //    switch (i)
                //    {
                //        case 1 // email
                //       :
                //            {
                //                colstats.Add(GetStat(histories, 1, "E-mail", 1));
                //                break;
                //            }

                //        case 2 // from a to b
                // :
                //            {
                //                colstats.Add(GetStat(histories, 1, "Form A to B", 2));
                //                break;
                //            }

                //        case 3 // letter
                // :
                //            {
                //                colstats.Add(GetStat(histories, 1, "Letter", 3));
                //                break;
                //            }

                //        case 4 // telephone
                // :
                //            {
                //                colstats.Add(GetStat(histories, 1, "Telephone", 4));
                //                break;
                //            }

                //        case 5 // walkin
                // :
                //            {
                //                colstats.Add(GetStat(histories, 1, "Walk-In", 5));
                //                break;
                //            }

                //        case 6 // case manager
                // :
                //            {
                //                colstats.Add(GetStat(histories, 1, "Case Man.",6));
                //                break;
                //            }

                //        case 7 // other
                // :
                //            {
                //                colstats.Add(GetStat(histories, 1, "Other", 7));
                //                break;
                //            }

                //        case 8 // judge
                // :
                //            {
                //                colstats.Add(GetStat(histories, 1, "Judge/Clerk", 8));
                //                break;
                //            }
                //    }
                //}
            }
            Stat objstats = new Stat();
            objstats.FieldName = "Total";
            objstats.GroupId = 1;
            objstats.CONT = colstats.Select(st => st.CONT).Sum();
            objstats.CS = colstats.Select(st => st.CS).Sum();
            objstats.CUST = colstats.Select(st => st.CUST).Sum();
            objstats.DOM = colstats.Select(st => st.DOM).Sum();
            objstats.DOMCH = colstats.Select(st => st.DOMCH).Sum();
            objstats.MODIF = colstats.Select(st => st.MODIF).Sum();
            objstats.NC = colstats.Select(st => st.NC).Sum();
            objstats.Other = colstats.Select(st => st.Other).Sum();
            objstats.PAT = colstats.Select(st => st.PAT).Sum();
            objstats.SimpDom = colstats.Select(st => st.SimpDom).Sum();
            objstats.SPA = colstats.Select(st => st.SPA).Sum();
            objstats.Total = colstats.Select(st => st.Total).Sum();
            colstats.Add(objstats);
            rptContact.DataSource = colstats;
            rptContact.DataBind();
            List<Stat> colstatsRes = new List<Stat>();
            Stat objStatLetter = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatLetter.FieldName = "Needs Letter";
                objStatLetter.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatLetter.SimpDom = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatLetter.DOM = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatLetter.DOMCH = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatLetter.NC = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatLetter.SPA = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatLetter.CUST = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatLetter.MODIF = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatLetter.CONT = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatLetter.PAT = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatLetter.Other = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatLetter.CS = histories.Where(h => h.NeedsLetter & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatLetter.CS;
                            break;
                        }
                }
                objStatLetter.Total += totalCount;
            }
            colstatsRes.Add(objStatLetter);
            Stat objStatProvForm = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatProvForm.FieldName = "Provided Forms";
                objStatProvForm.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatProvForm.SimpDom = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatProvForm.DOM = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatProvForm.DOMCH = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatProvForm.NC = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatProvForm.SPA = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatProvForm.CUST = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatProvForm.MODIF = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatProvForm.CONT = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatProvForm.PAT = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatProvForm.Other = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatProvForm.CS = histories.Where(h => h.ProvidedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatProvForm.CS;
                            break;
                        }
                }
                objStatProvForm.Total += totalCount;
            }
            colstatsRes.Add(objStatProvForm);
            Stat objStatAssisForm = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatAssisForm.FieldName = "Assisted w/ Forms";
                objStatAssisForm.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatAssisForm.SimpDom = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatAssisForm.DOM = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatAssisForm.DOMCH = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatAssisForm.NC = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatAssisForm.SPA = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatAssisForm.CUST = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.CUST;
                            break;
                        }

                    case 7:
                        {
                            objStatAssisForm.MODIF = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatAssisForm.CONT = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatAssisForm.PAT = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatAssisForm.Other = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatAssisForm.CS = histories.Where(h => h.AssistedForms & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisForm.CS;
                            break;
                        }
                }
                objStatAssisForm.Total += totalCount;
            }
            colstatsRes.Add(objStatAssisForm);
            Stat objStatAssisProc = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatAssisProc.FieldName = "Assisted w/ Procedures";
                objStatAssisProc.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatAssisProc.SimpDom = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatAssisProc.DOM = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatAssisProc.DOMCH = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatAssisProc.NC = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatAssisProc.SPA = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatAssisProc.CUST = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatAssisProc.MODIF = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatAssisProc.CONT = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatAssisProc.PAT = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatAssisProc.Other = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatAssisProc.CS = histories.Where(h => h.AssistedProcedures & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatAssisProc.CS;
                            break;
                        }
                }
                objStatAssisProc.Total += totalCount;
            }
            colstatsRes.Add(objStatAssisProc);
            Stat objStatFinHear = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatFinHear.FieldName = "Set Final Hearing";
                objStatFinHear.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatFinHear.SimpDom = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatFinHear.DOM = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatFinHear.DOMCH = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatFinHear.NC = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatFinHear.SPA = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatFinHear.CUST = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatFinHear.MODIF = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatFinHear.CONT = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatFinHear.PAT = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatFinHear.Other = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatFinHear.CS = histories.Where(h => h.SetFinalHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatFinHear.CS;
                            break;
                        }
                }
                objStatFinHear.Total += totalCount;
            }
            colstatsRes.Add(objStatFinHear);
            Stat objStatOHear = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatOHear.FieldName = "Other Hearing";
                objStatOHear.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatOHear.SimpDom = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatOHear.DOM = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatOHear.DOMCH = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatOHear.NC = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatOHear.SPA = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatOHear.CUST = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatOHear.MODIF = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatOHear.CONT = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatOHear.PAT = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatOHear.Other = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatOHear.CS = histories.Where(h => h.SetOtherHearing & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOHear.CS;
                            break;
                        }
                }
                objStatOHear.Total += totalCount;
            }
            colstatsRes.Add(objStatOHear);
            Stat objStatRefMed = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatRefMed.FieldName = "Referral Mediation";
                objStatRefMed.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatRefMed.SimpDom = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatRefMed.DOM = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatRefMed.DOMCH = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatRefMed.NC = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatRefMed.SPA = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatRefMed.CUST = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatRefMed.MODIF = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatRefMed.CONT = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatRefMed.PAT = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatRefMed.Other = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatRefMed.CS = histories.Where(h => h.ReferralOther & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMed.CS;
                            break;
                        }
                }
                objStatRefMed.Total += totalCount;
            }
            colstatsRes.Add(objStatRefMed);
            Stat objStatRefMag = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatRefMag.FieldName = "Referral Magistrate";
                objStatRefMag.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatRefMag.SimpDom = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatRefMag.DOM = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatRefMag.DOMCH = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatRefMag.NC = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatRefMag.SPA = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatRefMag.CUST = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatRefMag.MODIF = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatRefMag.CONT = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatRefMag.PAT = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatRefMag.Other = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatRefMag.CS = histories.Where(h => h.ReferralGmMag & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatRefMag.CS;
                            break;
                        }
                }
                objStatRefMag.Total += totalCount;
            }
            colstatsRes.Add(objStatRefMag);
            Stat objStatPreOrder = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatPreOrder.FieldName = "Prepared Order";
                objStatPreOrder.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatPreOrder.SimpDom = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatPreOrder.DOM = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatPreOrder.DOMCH = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatPreOrder.NC = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatPreOrder.SPA = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatPreOrder.CUST = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatPreOrder.MODIF = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatPreOrder.CONT = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatPreOrder.PAT = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatPreOrder.Other = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatPreOrder.CS = histories.Where(h => h.PreparedOrder & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatPreOrder.CS;
                            break;
                        }
                }
                objStatPreOrder.Total += totalCount;
            }
            colstatsRes.Add(objStatPreOrder);
            Stat objStatOther = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatOther.FieldName = "Other";
                objStatOther.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatOther.SimpDom = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatOther.DOM = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatOther.DOMCH = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatOther.NC = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatOther.SPA = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatOther.CUST = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatOther.MODIF = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatOther.CONT = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatOther.PAT = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatOther.Other = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatOther.CS = histories.Where(h => h.Other & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatOther.CS;
                            break;
                        }
                }
                objStatOther.Total += totalCount;
            }
            colstatsRes.Add(objStatOther);
            Stat objStatApptPro = new Stat();
            for (var i = 1; i <= 11; i++)
            {
                objStatApptPro.FieldName = "Appointed Professional";
                objStatApptPro.GroupId = 2;
                int totalCount = 0;
                int caseTypeId = i;
                switch (i)
                {
                    case 1:
                        {
                            objStatApptPro.SimpDom = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStatApptPro.DOM = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStatApptPro.DOMCH = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStatApptPro.NC = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.NC;
                            break;
                        }
                    case 5:
                        {
                            objStatApptPro.SPA = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStatApptPro.CUST = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStatApptPro.MODIF = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStatApptPro.CONT = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStatApptPro.PAT = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStatApptPro.Other = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.Other;
                            break;
                        }
                    case 11:
                        {
                            objStatApptPro.CS = histories.Where(h => h.AppointedPro & h.CaseTypeID == caseTypeId).Count();
                            totalCount = objStatApptPro.CS;
                            break;
                        }
                }
                objStatApptPro.Total += totalCount;
            }
            colstatsRes.Add(objStatApptPro);
            rptResolution.DataSource = colstatsRes;
            rptResolution.DataBind();
        }
        #endregion

        #region Methods
        private Stat GetStat(IEnumerable<HistoryListItem> historyList, int groupId, string fieldName, int contactId)
        {
            Stat objStat = new Stat();
            objStat.FieldName = fieldName;
            objStat.GroupId = groupId;
            int totalCount = 0;
            for (var i = 1; i <= 11; i++)
            {
                totalCount = 0;
                switch (i)
                {
                    case 1:
                        {
                            objStat.SimpDom = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 1).Count();
                            totalCount = objStat.SimpDom;
                            break;
                        }
                    case 2:
                        {
                            objStat.DOM = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 2).Count();
                            totalCount = objStat.DOM;
                            break;
                        }
                    case 3:
                        {
                            objStat.DOMCH = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 3).Count();
                            totalCount = objStat.DOMCH;
                            break;
                        }
                    case 4:
                        {
                            objStat.NC = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 4).Count();
                            totalCount = objStat.NC;
                            break;
                        }
                    case 5:
                        {
                            objStat.SPA = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 5).Count();
                            totalCount = objStat.SPA;
                            break;
                        }
                    case 6:
                        {
                            objStat.CUST = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 6).Count();
                            totalCount = objStat.CUST;
                            break;
                        }
                    case 7:
                        {
                            objStat.MODIF = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 7).Count();
                            totalCount = objStat.MODIF;
                            break;
                        }
                    case 8:
                        {
                            objStat.CONT = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 8).Count();
                            totalCount = objStat.CONT;
                            break;
                        }
                    case 9:
                        {
                            objStat.PAT = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 9).Count();
                            totalCount = objStat.PAT;
                            break;
                        }
                    case 10:
                        {
                            objStat.Other = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 10).Count();
                            totalCount = objStat.Other;
                            break;
                        }
                    case 11:
                        {
                            objStat.CS = historyList.Where(h => h.ContactID == contactId & h.CaseTypeID == 11).Count();
                            totalCount = objStat.CS;
                            break;
                        }
                }
                objStat.Total += totalCount;
            }
            return objStat;
        }
        #endregion
    }
}