<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.ExitSurvey.View" %>

<div class="container-fluid exit-survey">

    <asp:HyperLink ID="lnkResults" runat="server" Visible="false" CssClass="btn btn-secondary" Text="View Results" />

    <asp:PlaceHolder ID="plhMessage" runat="server" EnableViewState="false"></asp:PlaceHolder>

    <asp:Panel ID="pnlForm" runat="server">

        <div class="exit-survey-intro">
            <p>Dear Employee,</p>
            <p>As you prepare to leave the Twelfth Judicial Circuit, you have gained experience, insight and opinions as a valued employee. Therefore, we kindly request that you share your opinion and impression of working with the Twelfth Circuit by completing an employee exit survey.</p>
            <p>This survey is designed to obtain your feedback which will assist us in making the Twelfth Circuit an employer of choice for existing and future employees. So your comments will be very valuable in the development of training, benefits, our recruitment and retention efforts, and our efforts to improve our work environment.</p>
            <p>Again, your honesty is greatly appreciated and your opinions valued, and we ask that you complete all items.</p>
            <p>Should you have any questions about the Exit Survey process, please feel free to call the Human Resources Office, at (941) 861-7811.</p>
            <p>Unless specifically requested otherwise, pursuant to the State Courts System Personnel Policy, your responses to the Employee Exit Survey will be part of your personnel file.</p>
        </div>

        <div class="form-check mb-3">
            <asp:CheckBox ID="chkDoNotShare" runat="server" CssClass="form-check-input" />
            <label class="form-check-label" for="<%= chkDoNotShare.ClientID %>">Check here if you request that the Exit Survey not be shared with your supervisor.</label>
        </div>

        <%-- Q1 -------------------------------------------------------------- --%>
        <h5 class="exit-survey-question">1. This question relates to general conditions of the job from which employee resigned. Please indicate how you feel about the listed work areas at the office/work unit.</h5>
        <asp:Repeater ID="rptGeneral" runat="server" OnItemDataBound="rptRating_ItemDataBound">
            <ItemTemplate>
                <div class="row mb-3 exit-survey-rating">
                    <div class="col-md-4 exit-survey-rating-label">
                        <asp:Literal ID="ltLabel" runat="server"></asp:Literal>
                        <asp:HiddenField ID="hfKey" runat="server" />
                    </div>
                    <div class="col-md-8">
                        <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-options"></asp:RadioButtonList>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="row mb-3">
            <div class="col-md-4"><label for="<%= txtQ1Other.ClientID %>">Other (specify)</label></div>
            <div class="col-md-8"><asp:TextBox ID="txtQ1Other" runat="server" CssClass="form-control" MaxLength="255" /></div>
        </div>

        <%-- Q2 -------------------------------------------------------------- --%>
        <div class="mb-3">
            <label class="exit-survey-question" for="<%= txtQ2.ClientID %>">2. If you are accepting employment elsewhere, what advantages do you feel the new employer offers that you have not found here at the Twelfth Judicial Circuit?</label>
            <asp:TextBox ID="txtQ2" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
        </div>

        <%-- Q3 -------------------------------------------------------------- --%>
        <div class="mb-3">
            <span class="exit-survey-question">3. Have you accepted another position?</span>
            <asp:RadioButtonList ID="rblAccepted" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-options">
                <asp:ListItem Text="Yes" Value="1" />
                <asp:ListItem Text="No" Value="0" />
            </asp:RadioButtonList>
        </div>

        <%-- Q4 -------------------------------------------------------------- --%>
        <div class="mb-3">
            <span class="exit-survey-question">4. If yes, what type of employer?</span>
            <asp:RadioButtonList ID="rblEmployerType" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="stacked-options" />
            <div class="row mt-2">
                <div class="col-md-4"><label for="<%= txtQ4Other.ClientID %>">Other (specify)</label></div>
                <div class="col-md-8"><asp:TextBox ID="txtQ4Other" runat="server" CssClass="form-control" MaxLength="255" /></div>
            </div>
        </div>

        <%-- Q5 -------------------------------------------------------------- --%>
        <div class="mb-3">
            <span class="exit-survey-question">5. What influenced you to leave the Twelfth Judicial Circuit? (Select all that apply.)</span>
            <asp:CheckBoxList ID="cblReasons" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" CssClass="stacked-options" />
            <div class="row mt-2">
                <div class="col-md-4"><label for="<%= txtQ5Other.ClientID %>">Other (specify)</label></div>
                <div class="col-md-8"><asp:TextBox ID="txtQ5Other" runat="server" CssClass="form-control" MaxLength="255" /></div>
            </div>
        </div>

        <%-- Q6 -------------------------------------------------------------- --%>
        <h5 class="exit-survey-question">6. Please indicate your feelings about the supervision you received while employed with the Twelfth Judicial Circuit.</h5>
        <asp:Repeater ID="rptSupervision" runat="server" OnItemDataBound="rptRating_ItemDataBound">
            <ItemTemplate>
                <div class="row mb-3 exit-survey-rating">
                    <div class="col-md-4 exit-survey-rating-label">
                        <asp:Literal ID="ltLabel" runat="server"></asp:Literal>
                        <asp:HiddenField ID="hfKey" runat="server" />
                    </div>
                    <div class="col-md-8">
                        <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-options"></asp:RadioButtonList>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <%-- Q7 -------------------------------------------------------------- --%>
        <div class="mb-3">
            <label class="exit-survey-question" for="<%= txtQ7.ClientID %>">7. Did you receive adequate training and/or other resources necessary to accomplish your job? If not, please provide details.</label>
            <asp:TextBox ID="txtQ7" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
        </div>

        <%-- Q8 -------------------------------------------------------------- --%>
        <div class="mb-3">
            <label class="exit-survey-question" for="<%= txtQ8.ClientID %>">8. Please comment on what you feel could be done to help make the Twelfth Judicial Circuit a better place to work.</label>
            <asp:TextBox ID="txtQ8" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
        </div>

        <%-- Q9 -------------------------------------------------------------- --%>
        <div class="mb-3">
            <span class="exit-survey-question">9. Would you consider working for the Twelfth Judicial Circuit again in the future?</span>
            <asp:RadioButtonList ID="rblWouldReturn" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-options">
                <asp:ListItem Text="Yes" Value="1" />
                <asp:ListItem Text="No" Value="0" />
            </asp:RadioButtonList>
        </div>

        <%-- Optional identifying section ------------------------------------ --%>
        <h5 class="exit-survey-question">Optional</h5>
        <div class="row mb-3">
            <div class="col-md-6">
                <label for="<%= txtName.ClientID %>">Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-md-6">
                <label for="<%= txtPositionTitle.ClientID %>">Most recent position title in Court employment</label>
                <asp:TextBox ID="txtPositionTitle" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-6">
                <label for="<%= txtSupervisorName.ClientID %>">Name of Supervisor</label>
                <asp:TextBox ID="txtSupervisorName" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-md-6">
                <label for="<%= txtSupervisorTitle.ClientID %>">Supervisor's Title</label>
                <asp:TextBox ID="txtSupervisorTitle" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-4">
                <label for="<%= txtHired.ClientID %>">Month/Year you were hired at the Court</label>
                <asp:TextBox ID="txtHired" runat="server" CssClass="form-control" MaxLength="50" />
            </div>
            <div class="col-md-4">
                <label for="<%= txtSeparated.ClientID %>">Month/Year you separated</label>
                <asp:TextBox ID="txtSeparated" runat="server" CssClass="form-control" MaxLength="50" />
            </div>
            <div class="col-md-4">
                <label for="<%= txtDateCompleted.ClientID %>">Date Exit Survey Completed</label>
                <asp:TextBox ID="txtDateCompleted" runat="server" CssClass="form-control" MaxLength="50" />
            </div>
        </div>

        <hr />
        <p>
            <asp:Button ID="cmdSubmit" runat="server" CssClass="btn btn-primary" OnClick="cmdSubmit_Click" Text="Submit Survey" />
        </p>

    </asp:Panel>

</div>
