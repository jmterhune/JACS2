<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Review.ascx.cs" Inherits="tjc.Modules.ProcessServer.Review" %>
<asp:Panel ID="pnlView" runat="server">
    <h3>Certified Process Server Application</h3>
    <p class="alert alert-info">
        <strong>Payment Type:</strong>
        <asp:Literal ID="ltPaymentType" runat="server" />
    </p>

    <div class="mb-3">
        <div class="row">
            <div class="col-md-3">
                <asp:Label ID="lblFirstName" runat="server" AssociatedControlID="txtFirstName" Text="First Name <em>*</em>" />
                <asp:TextBox ID="txtFirstName" CssClass="form-control" MaxLength="25" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <asp:Label ID="lblMiddleInitial" runat="server" AssociatedControlID="txtMiddleInitial" Text="Middle Initial" />
                <asp:TextBox ID="txtMiddleInitial" CssClass="form-control" MaxLength="25" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName" Text="Last Name <em>*</em>" />
                <asp:TextBox ID="txtLastName" CssClass="form-control" MaxLength="25" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblMaidenName" runat="server" AssociatedControlID="txtMaidenName" Text="Maiden Name" />
                <asp:TextBox ID="txtMaidenName" CssClass="form-control" MaxLength="25" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
        </div>
    </div>
    <div class="mb-3">
        <div class="row">
            <div class="col-md-2">
                <asp:Label ID="lblIdNumber" runat="server" AssociatedControlID="txtIdNumber" Text="ID Number" />
                <asp:TextBox ID="txtIdNumber" ReadOnly="true" CssClass="form-control" MaxLength="5" runat="server"></asp:TextBox>
            </div>
            <div class="col-md-6">
                <asp:Label ID="lblAliases" runat="server" AssociatedControlID="txtAliases" Text="Alias(es), other changes in name" />
                <asp:TextBox ID="txtAliases" CssClass="form-control" MaxLength="250" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEMail" Text="Email Address<em>*</em>" />
                <asp:TextBox ID="txtEMail" runat="server" MaxLength="150" CssClass="form-control" AutoCompleteType="Email" ReadOnly="true" />
            </div>
        </div>
    </div>
    <div class="mb-3">
        <div class="row">
            <div class="col-md-2">
                <asp:Label ID="lblCell" runat="server" AssociatedControlID="txtCell" Text="Primary Phone<em>*</em>" />
                <asp:TextBox ID="txtCell" runat="server" MaxLength="15" ReadOnly="true" CssClass="form-control phone-us" placeholder="(555) 555-5555" AutoCompleteType="Cellular" />

            </div>

            <div class="col-md-2">
                <asp:Label ID="lblPhone" runat="server" AssociatedControlID="txtPhone" Text="Secondary Phone" />
                <asp:TextBox ID="txtPhone" runat="server" MaxLength="15" ReadOnly="true" CssClass="form-control phone-us" placeholder="(555) 555-5555" AutoCompleteType="HomePhone" />

            </div>
            <div class="col-md-2">
                <asp:Label ID="lblSNN" runat="server" AssociatedControlID="txtSSN" Text="<abbr title='Social Security Number'>SSN</abbr><em>*</em>" />
                <asp:TextBox ID="txtSSN" runat="server" MaxLength="250" ReadOnly="true" CssClass="form-control ssn" placeholder="000-00-0000" />
            </div>
            <div class="col-md-2">
                <asp:Label ID="lblDOB" runat="server" AssociatedControlID="txtDOB" Text="Date of Birth<em>*</em>" />
                <asp:TextBox ID="txtDOB" runat="server" MaxLength="20" ReadOnly="true" CssClass="form-control datepicker" />
            </div>
            <div class="col-md-4">
                <asp:Label ID="lblBirthPlace" runat="server" AssociatedControlID="txtBirthPlace" Text="Place of Birth<em>*</em>" />
                <asp:TextBox ID="txtBirthPlace" CssClass="form-control" ReadOnly="true" MaxLength="250" runat="server"></asp:TextBox>
            </div>
        </div>
    </div>
    <div class="mb-3">
        <div class="row">
            <div class="col-md-5">
                <asp:Label ID="lblAddress" runat="server" AssociatedControlID="txtAddress" Text="Mailing Address<em>*</em>" />
                <asp:TextBox ID="txtAddress" runat="server" MaxLength="100" ReadOnly="true" CssClass="form-control" AutoCompleteType="HomeStreetAddress" />
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblCity" runat="server" AssociatedControlID="txtCity" Text="City<em>*</em>" />
                <asp:TextBox ID="txtCity" runat="server" MaxLength="50" ReadOnly="true" CssClass="form-control" AutoCompleteType="HomeCity" />

            </div>
            <div class="col-md-2">
                <asp:Label ID="lblState" runat="server" AssociatedControlID="drpState" Text="State<em>*</em>" />
                <asp:DropDownList ID="drpState" runat="server" Enabled="false" CssClass="form-control">
                    <asp:ListItem Value="" Text="<Select State>" />
                    <asp:ListItem Value="AL">Alabama</asp:ListItem>
                    <asp:ListItem Value="AK">Alaska</asp:ListItem>
                    <asp:ListItem Value="AZ">Arizona</asp:ListItem>
                    <asp:ListItem Value="AR">Arkansas</asp:ListItem>
                    <asp:ListItem Value="CA">California</asp:ListItem>
                    <asp:ListItem Value="CO">Colorado</asp:ListItem>
                    <asp:ListItem Value="CT">Connecticut</asp:ListItem>
                    <asp:ListItem Value="DC">District of Columbia</asp:ListItem>
                    <asp:ListItem Value="DE">Delaware</asp:ListItem>
                    <asp:ListItem Value="FL">Florida</asp:ListItem>
                    <asp:ListItem Value="GA">Georgia</asp:ListItem>
                    <asp:ListItem Value="HI">Hawaii</asp:ListItem>
                    <asp:ListItem Value="ID">Idaho</asp:ListItem>
                    <asp:ListItem Value="IL">Illinois</asp:ListItem>
                    <asp:ListItem Value="IN">Indiana</asp:ListItem>
                    <asp:ListItem Value="IA">Iowa</asp:ListItem>
                    <asp:ListItem Value="KS">Kansas</asp:ListItem>
                    <asp:ListItem Value="KY">Kentucky</asp:ListItem>
                    <asp:ListItem Value="LA">Louisiana</asp:ListItem>
                    <asp:ListItem Value="ME">Maine</asp:ListItem>
                    <asp:ListItem Value="MD">Maryland</asp:ListItem>
                    <asp:ListItem Value="MA">Massachusetts</asp:ListItem>
                    <asp:ListItem Value="MI">Michigan</asp:ListItem>
                    <asp:ListItem Value="MN">Minnesota</asp:ListItem>
                    <asp:ListItem Value="MS">Mississippi</asp:ListItem>
                    <asp:ListItem Value="MO">Missouri</asp:ListItem>
                    <asp:ListItem Value="MT">Montana</asp:ListItem>
                    <asp:ListItem Value="NE">Nebraska</asp:ListItem>
                    <asp:ListItem Value="NV">Nevada</asp:ListItem>
                    <asp:ListItem Value="NH">New Hampshire</asp:ListItem>
                    <asp:ListItem Value="NJ">New Jersey</asp:ListItem>
                    <asp:ListItem Value="NM">New Mexico</asp:ListItem>
                    <asp:ListItem Value="NY">New York</asp:ListItem>
                    <asp:ListItem Value="NC">North Carolina</asp:ListItem>
                    <asp:ListItem Value="ND">North Dakota</asp:ListItem>
                    <asp:ListItem Value="OH">Ohio</asp:ListItem>
                    <asp:ListItem Value="OK">Oklahoma</asp:ListItem>
                    <asp:ListItem Value="OR">Oregon</asp:ListItem>
                    <asp:ListItem Value="PA">Pennsylvania</asp:ListItem>
                    <asp:ListItem Value="RI">Rhode Island</asp:ListItem>
                    <asp:ListItem Value="SC">South Carolina</asp:ListItem>
                    <asp:ListItem Value="SD">South Dakota</asp:ListItem>
                    <asp:ListItem Value="TN">Tennessee</asp:ListItem>
                    <asp:ListItem Value="TX">Texas</asp:ListItem>
                    <asp:ListItem Value="UT">Utah</asp:ListItem>
                    <asp:ListItem Value="VT">Vermont</asp:ListItem>
                    <asp:ListItem Value="VA">Virginia</asp:ListItem>
                    <asp:ListItem Value="WA">Washington</asp:ListItem>
                    <asp:ListItem Value="WV">West Virginia</asp:ListItem>
                    <asp:ListItem Value="WI">Wisconsin</asp:ListItem>
                    <asp:ListItem Value="WY">Wyoming</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Label ID="lblZip" runat="server" AssociatedControlID="txtZip" Text="Zip<em>*</em>" />
                <asp:TextBox ID="txtZip" runat="server" ReadOnly="true" MaxLength="11" CssClass="form-control" AutoCompleteType="HomeZipCode" />
            </div>
        </div>
    </div>
    <div class="mb-3">
        <div class="row">
            <div class="col-md-3">
                <asp:Label ID="lblCounty" runat="server" AssociatedControlID="txtCounty" Text="County<em>*</em>" />
                <asp:TextBox ID="txtCounty" runat="server" ReadOnly="true" MaxLength="50" CssClass="form-control" />
            </div>

            <div class="col-md-3">
                <asp:Label ID="lblHowLong" runat="server" AssociatedControlID="txtHowLong" Text="How Long at Address?<em>*</em>" />
                <asp:TextBox ID="txtHowLong" runat="server" ReadOnly="true" MaxLength="20" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblRace" runat="server" AssociatedControlID="drpRace" Text="Race<em>*</em>" />
                <asp:DropDownList ID="drpRace" runat="server" Enabled="false" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Text="< Select Race >" Value=""></asp:ListItem>
                    <asp:ListItem Text="Afican American Not Hispanic" />
                    <asp:ListItem Text="Asian / Pacific Islander" />
                    <asp:ListItem Text="Hispanic" />
                    <asp:ListItem Text="Native American / Alaskan" />
                    <asp:ListItem Text="White" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblSex" runat="server" AssociatedControlID="rblSex" Text="Gender<em>*</em>" />
                <asp:RadioButtonList ID="rblSex" runat="server" Enabled="false" CssClass="dnnFormRadioButtons form-control" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Text="Male" />
                    <asp:ListItem Text="Female" />
                </asp:RadioButtonList>
            </div>
        </div>
    </div>
    <div class="mb-3">
        <div class="row">
            <div class="col-md-3">
                <asp:Label ID="lblHeight" runat="server" AssociatedControlID="txtHeight" Text="Height<em>*</em>" />
                <asp:TextBox ID="txtHeight" runat="server" ReadOnly="true" MaxLength="20" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblWeight" runat="server" AssociatedControlID="txtWeight" Text="Weight<em>*</em>" />
                <asp:TextBox ID="txtWeight" runat="server" ReadOnly="true" MaxLength="20" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblEyeColor" runat="server" AssociatedControlID="txtEyeColor" Text="Eye Color<em>*</em>" />
                <asp:TextBox ID="txtEyeColor" runat="server" ReadOnly="true" MaxLength="20" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblHairColor" runat="server" AssociatedControlID="txtHairColor" Text="Hair Color<em>*</em>" />
                <asp:TextBox ID="txtHairColor" runat="server" ReadOnly="true" MaxLength="20" CssClass="form-control" />
            </div>
        </div>
    </div>
    <h3>Certified Process Server Questionnaire</h3>
    <ol class="questions">
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion1" runat="server" AssociatedControlID="rblQuestion1" Text="Are you a permanent resident of the State of Florida?<em>*</em>" />
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion1" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion2" runat="server" AssociatedControlID="rblQuestion2" Text="Do you have a mental or legal disability?<em>*</em>" />
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion2" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion3" runat="server" AssociatedControlID="rblQuestion3" Text="Have you ever been diagnosed or adjudicated incompetent under Florida Statute 744.331 or a similar statute in another state?<em>*</em>" />
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion3" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion4" runat="server" AssociatedControlID="rblQuestion4" Text="Do you chronically and habitually consume alcoholic beverages to the extent that your normal faculties are impaired?<em>*</em>" />
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion4" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion5" runat="server" AssociatedControlID="rblQuestion5" Text="Do you chronically and habitually use a controlled substance?<em>*</em>" />
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion5" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion6" runat="server" AssociatedControlID="rblQuestion6" Text="Have you been committed for treatment for the abuse of alcohol or controlled substances? If yes, explain:?<em>*</em>" />
                        <asp:TextBox ID="txtQuestion6" runat="server" Enabled="false" CssClass="form-control" MaxLength="2000" TextMode="MultiLine" Rows="2"></asp:TextBox>

                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion6" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion7" runat="server" AssociatedControlID="rblQuestion7" Text="Have you been convicted of a felony? If yes, please indicate the charges for which you were convicted, the date of the conviction, and the state in which you were convicted:<em>*</em>" />
                        <asp:TextBox ID="txtQuestion7" runat="server" ReadOnly="true" CssClass="form-control" MaxLength="2000" TextMode="MultiLine" Rows="2"></asp:TextBox>
                        <span class="counter text-muted">&nbsp;</span>

                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion7" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion8" runat="server" AssociatedControlID="rblQuestion8" Text="Have you been convicted of a misdemeanor within the last five years? If yes, please indicate the charges for which you were convicted, the date of the conviction, and the state in which you were convicted:<em>*</em>" />
                        <asp:TextBox ID="txtQuestion8" runat="server" ReadOnly="true" CssClass="form-control" MaxLength="2000" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion8" runat="server" Enabled="false" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion9" runat="server" AssociatedControlID="rblQuestion9" Text="Do you have any pending criminal charges?<em>*</em>" />
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion9" runat="server" Enabled="false" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion10" runat="server" AssociatedControlID="rblQuestion10" Text="Are you an active or former law enforcement officer? <span class='text-danger'>If yes, you must submit a written request to exempt certain information from public inspection</span>.<em>*</em>" />
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion10" runat="server" Enabled="false" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblQuestion11" runat="server" AssociatedControlID="rblQuestion11" Text="Have you ever had a driver’s license issued in another state? If yes, which state(s)?<em>*</em>" />
                        <asp:TextBox ID="txtQuestion11" runat="server" ReadOnly="true" CssClass="form-control" MaxLength="2000" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <asp:RadioButtonList ID="rblQuestion11" Enabled="false" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="dnnFormRadioButtons horizontal">
                            <asp:ListItem Text="Yes" />
                            <asp:ListItem Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </li>
        <li>
            <div class="mb-3">
                <div class="row">
                    <div class="col-md-9">
                        <asp:Label ID="lblCountyServed" runat="server" AssociatedControlID="chkDeSoto" Text="In which counties in the Twelfth Judicial Circuit are you willing to serve?<em>*</em>" />
                    </div>
                    <div class="col-md-3 county-served">
                        <asp:CheckBox ID="chkDeSoto" runat="server"  Enabled="false" Text="DeSoto" />
                        <asp:CheckBox ID="chkManatee" runat="server"  Enabled="false" Text="Manatee" />
                        <asp:CheckBox ID="chkSarasota" runat="server"  Enabled="false" Text="Sarasota" />
                    </div>
                </div>
            </div>
        </li>

    </ol>

    <h3>
        <asp:CheckBox ID="chkPolicy" CssClass="policy-agreement" Enabled="false" runat="server" Text="Statement of Policy and Certificate of Good Conduct of Certified Process Servers" /></h3>


    <h3>
        <asp:CheckBox ID="chkAcknowledge" Enabled="false" CssClass="acknowledge-agreement" runat="server" Text="Acknowledgement and Verification" />
    </h3>
    <h3>
        <asp:CheckBox ID="chkAuthorize" Enabled="false" CssClass="authorize-agreement" runat="server" Text="Authorization and Consent for Release of Personal Information and Criminal Background Check" />
    </h3>
    <asp:Repeater ID="rptUploades" runat="server" OnItemDataBound="rptUploades_ItemDataBound">
        <HeaderTemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>File Name</th>
                        <th>Uploaded</th>
                        <th>Hidden?</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:HyperLink ID="lnkFile" runat="server"><%#DataBinder.Eval(Container.DataItem,"OriginalFileName") %></asp:HyperLink></td>
                <td><%#DataBinder.Eval(Container.DataItem,"DateUploaded","{0:MM/dd/yyyy}") %></td>
                <td><%#Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"hidden"))?"Y":"N" %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
            </table>
        </FooterTemplate>

    </asp:Repeater>

    <hr />
    <div class="mb-3">
        <asp:Label ID="lblCourtCounselApproval" runat="server" CssClass="fw-bold" AssociatedControlID="rblCourtCounselApproval" Text="General Counsel Approval:" />
        <asp:RadioButtonList ID="rblCourtCounselApproval" runat="server" CssClass="dnnFormRadioButtons  w-25" RepeatDirection="Horizontal" RepeatLayout="Flow">
            <asp:ListItem Text="Approve" Value="1" />
            <asp:ListItem Text="Reject" Value="0" />
        </asp:RadioButtonList>
    </div>
    <div class="mb-3">
        <asp:Label ID="lblChiefJudgeApproval" runat="server" CssClass="fw-bold" AssociatedControlID="rblCourtCounselApproval" Text="Chief Judge Approval:" />
        <asp:RadioButtonList ID="rblChiefJudgeApproval" runat="server" CssClass="dnnFormRadioButtons  w-25" RepeatDirection="Horizontal" RepeatLayout="Flow">
            <asp:ListItem Text="Approve" Value="1" />
            <asp:ListItem Text="Reject" Value="0" />
        </asp:RadioButtonList>
    </div>
    <div class="mb-3">
        <asp:Label ID="lblReason" runat="server" CssClass="fw-bold" AssociatedControlID="txtReason" Text="Reason for Rejection" />
        <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
    </div>
    <p>
        <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" CssClass="btn btn-primary" />
        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" OnClick="btnCancel_Click" Text="Cancel" CssClass="btn btn-default" />
    </p>
</asp:Panel>
