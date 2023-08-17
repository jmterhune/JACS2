using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using tjc.Modules.EmployeeDB.Components;

namespace tjc.Modules.EmployeeDB
{
    public partial class SwnList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    int maxphone = 0;
                    int maxgroup = 0;
                    int maxsms = 0;
                    var ctl = new EmployeeController();

                    var EmployeeList = ctl.GetSwnContacts();

                    maxphone = GetMaxPhone();
                    maxgroup = GetMaxGroup();
                    maxsms = GetMaxSMS();
                    StringBuilder builder = new StringBuilder();
                    List<string> rows = new List<string>();

                    foreach (SwnContact s in EmployeeList)
                    {
                        s.PhoneList = GetPhoneList(s.UniqueID).ToList();
                        int phonecount = s.PhoneList.Where(p => p.SwnCall == true).Count();
                        int textcount = s.PhoneList.Where(p => p.SwnText == true).Count();
                        s.GroupList = GetGroupList(s.UniqueID);
                        int grpCount = s.GroupList.Count;

                        List<string> currentRow = new List<string>
                    {
                        s.UniqueID.ToString(),
                        s.LastName,
                        s.FirstName,
                        s.MiddleInitial
                    };
                        foreach (Group g in s.GroupList)
                        {
                            currentRow.Add(g.GroupId.ToString());
                            currentRow.Add(g.GroupName);
                        }
                        if (maxgroup == 0)
                        {
                            currentRow.Add("");
                            currentRow.Add("");
                        }
                        else if (grpCount < maxgroup)
                        {
                            for (var i = grpCount + 1; i <= maxgroup; i++)
                            {
                                currentRow.Add("");
                                currentRow.Add("");
                            }
                        }
                        currentRow.Add(s.Address1);
                        currentRow.Add(s.Address2);
                        currentRow.Add(s.City);
                        currentRow.Add(s.StateProvince);
                        currentRow.Add(s.ZipPostalCode);
                        currentRow.Add(s.Country);
                        currentRow.Add(s.TimeZone);
                        currentRow.Add(s.PreferredLanguage);
                        currentRow.Add(s.CustomLabel1);
                        currentRow.Add(s.CustomValue1);
                        currentRow.Add(s.CustomLabel2);
                        currentRow.Add(s.CustomValue2);
                        currentRow.Add(s.CustomLabel3);
                        currentRow.Add(s.CustomValue3);
                        currentRow.Add(s.CustomLabel4);
                        currentRow.Add(s.CustomValue4);
                        currentRow.Add(s.CustomLabel5);
                        currentRow.Add(s.CustomValue5);
                        currentRow.Add(s.CustomLabel6);
                        currentRow.Add(s.CustomValue6);

                        foreach (SwnPhone p in s.PhoneList)
                        {
                            if (p.SwnCall)
                            {
                                currentRow.Add(p.PhoneType);
                                currentRow.Add(p.CountryCode);
                                currentRow.Add(p.PhoneNumber);
                                if (p.SwnExcludeExtension)
                                    currentRow.Add("");
                                else
                                    currentRow.Add(p.Extension);
                                currentRow.Add(p.PhoneCascade.ToString());
                            }
                        }
                        if (maxphone == 0)
                        {
                            currentRow.Add("");
                            currentRow.Add("");
                            currentRow.Add("");
                            currentRow.Add("");
                            currentRow.Add("");
                        }
                        else if (phonecount < maxphone)
                        {
                            for (var i = phonecount + 1; i <= maxphone; i++)
                            {
                                currentRow.Add("");
                                currentRow.Add("");
                                currentRow.Add("");
                                currentRow.Add("");
                                currentRow.Add("");
                            }
                        }
                        currentRow.Add(s.EmailLabel1);
                        currentRow.Add(s.Email1);
                        currentRow.Add(s.EmailLabel2);
                        currentRow.Add(s.Email2);
                        foreach (SwnPhone p in s.PhoneList)
                        {
                            if (p.SwnText)
                            {
                                currentRow.Add(p.SmsLabel);
                                currentRow.Add(p.Sms);
                            }
                        }
                        if (maxsms == 0)
                        {
                            currentRow.Add("");
                            currentRow.Add("");
                        }
                        else if (textcount < maxsms)
                        {
                            for (var i = textcount + 1; i <= maxsms; i++)
                            {
                                currentRow.Add("");
                                currentRow.Add("");
                            }
                        }
                        currentRow.Add(s.BBPinLabel);
                        currentRow.Add(s.BBPin);
                        rows.Add(string.Join("|", currentRow.ToArray()));
                    }

                    List<string> columnNames = new List<string>
                {
                    "UNIQUE ID",
                    "LAST NAME",
                    "FIRST NAME",
                    "MIDDLE INITIAL"
                };
                    if (maxgroup > 1)
                    {
                        for (var i = 1; i <= maxgroup; i++)
                        {
                            columnNames.Add("GROUP ID " + i.ToString());
                            columnNames.Add("GROUP DESCRIPTION " + i.ToString());
                        }
                    }
                    else if (maxgroup <= 1)
                    {
                        columnNames.Add("GROUP ID");
                        columnNames.Add("GROUP DESCRIPTION");
                    }
                    columnNames.Add("ADDRESS 1");
                    columnNames.Add("ADDRESS 2");
                    columnNames.Add("CITY");
                    columnNames.Add("STATE/PROVINCE");
                    columnNames.Add("ZIP/POSTAL CODE");
                    columnNames.Add("COUNTRY");
                    columnNames.Add("TIME ZONE");
                    columnNames.Add("PREFERRED LANGUAGE");
                    columnNames.Add("CUSTOM LABEL 1");
                    columnNames.Add("CUSTOM VALUE 1");
                    columnNames.Add("CUSTOM LABEL 2");
                    columnNames.Add("CUSTOM VALUE 2");
                    columnNames.Add("CUSTOM LABEL 3");
                    columnNames.Add("CUSTOM VALUE 3");
                    columnNames.Add("CUSTOM LABEL 4");
                    columnNames.Add("CUSTOM VALUE 4");
                    columnNames.Add("CUSTOM LABEL 5");
                    columnNames.Add("CUSTOM VALUE 5");
                    columnNames.Add("CUSTOM LABEL 6");
                    columnNames.Add("CUSTOM VALUE 6");
                    if (maxphone > 1)
                    {
                        for (var i = 1; i <= maxphone; i++)
                        {
                            columnNames.Add("PHONE LABEL " + i.ToString());
                            columnNames.Add("PHONE COUNTRY CODE " + i.ToString());
                            columnNames.Add("PHONE " + i.ToString());
                            columnNames.Add("PHONE EXTENSION " + i.ToString());
                            columnNames.Add("CASCADE " + i.ToString());
                        }
                    }
                    else if (maxphone <= 1)
                    {
                        columnNames.Add("PHONE LABEL");
                        columnNames.Add("PHONE COUNTRY CODE");
                        columnNames.Add("PHONE");
                        columnNames.Add("PHONE EXTENSION");
                        columnNames.Add("CASCADE");
                    }
                    columnNames.Add("EMAIL LABEL 1");
                    columnNames.Add("EMAIL 1");
                    columnNames.Add("EMAIL LABEL 2");
                    columnNames.Add("EMAIL 2");
                    if (maxsms > 1)
                    {
                        for (var i = 1; i <= maxsms; i++)
                        {
                            columnNames.Add("SMS LABEL " + i.ToString());
                            columnNames.Add("SMS " + i.ToString());
                        }
                    }
                    else if (maxsms <= 1)
                    {
                        columnNames.Add("SMS LABEL");
                        columnNames.Add("SMS");
                    }
                    columnNames.Add("BB PIN LABEL");
                    columnNames.Add("BB PIN");

                    builder.Append(string.Join("|", columnNames.ToArray())).Append(Environment.NewLine);
                    builder.Append(string.Join(Environment.NewLine, rows.ToArray()));
                    Response.Clear();
                    Response.ContentType = "text/plain";
                    string fileNameHeader = string.Format("attachment;filename=SWN-Export-{0}.txt", DateTime.Now.ToString("MM-dd-yyyy-h-m"));
                    Response.AddHeader("Content-Disposition", fileNameHeader);
                    Response.Write(builder.ToString());
                    Response.End();

                }
                catch (Exception exc)
                {
                    ltMessage.Text =string.Format("<div class='alert alert-danger'><i class='bi bi-x-circle-fill'></i> The list could not be generated due to the following error:<p class='text-danger'>{0}</p></div> ", exc.Message);
                    Exceptions.LogException(exc);
                }
            }
        }
        private List<SwnPhone> GetPhoneList(long employeeId)
        {
            var ctl = new PhoneController();

            return ctl.GetSwnPhonesByEmployee(employeeId).ToList();
        }

        private List<Group> GetGroupList(long employeeId)
        {
            var ctl = new GroupController();

            return ctl.GetEmployeeSwnGroups(employeeId).ToList();
        }

        private int GetMaxGroup()
        {
            var ctl = new GroupController();
            return ctl.GetMaxGroup();
        }

        private int GetMaxPhone()
        {
            var ctl = new PhoneController();
            return ctl.GetMaxPhone();
        }

        private int GetMaxSMS()
        {
            var ctl = new PhoneController();
            return ctl.GetMaxSMS();
        }

    }
}