<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.ExitSurvey.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />

<div class="container-fluid exit-survey">

    <asp:HyperLink ID="lnkResults" runat="server" Visible="false" CssClass="btn btn-secondary mb-2" Text="View Results" />

    <asp:PlaceHolder ID="plhMessage" runat="server" EnableViewState="false"></asp:PlaceHolder>

    <asp:Panel ID="pnlForm" runat="server">

        <div class="exit-survey-intro">
            <p>Dear <%= UserDisplayName %>,</p>
            <p>As you prepare to leave the Twelfth Judicial Circuit, you have gained experience, insight and opinions as a valued employee. Therefore, we kindly request that you share your opinion and impression of working with the Twelfth Circuit by completing an employee exit survey.</p>
            <p>This survey is designed to obtain your feedback which will assist us in making the Twelfth Circuit an employer of choice for existing and future employees. So your comments will be very valuable in the development of training, benefits, our recruitment and retention efforts, and our efforts to improve our work environment.</p>
            <p>Again, your honesty is greatly appreciated and your opinions valued, and we ask that you complete all items.</p>
            <p>Should you have any questions about the Exit Survey process, please feel free to call the Human Resources Office, at (941) 861-7811.</p>
            <p>Unless specifically requested otherwise, pursuant to the State Courts System Personnel Policy, your responses to the Employee Exit Survey will be part of your personnel file.</p>
        </div>

        <div class="mb-3 exit-survey-single-check">
            <asp:CheckBox ID="chkDoNotShare" runat="server" Text="Check here if you request that the Exit Survey not be shared with your supervisor." />
        </div>

        <%-- Q1 -------------------------------------------------------------- --%>
        <div class="exit-survey-question">1. This question relates to general conditions of the job from which employee resigned. Please indicate how you feel about the listed work areas at the office/work unit.</div>
        <div class="es-matrix" data-vlabel="Question 1 &ndash; rate every work area">
        <asp:Repeater ID="rptGeneral" runat="server" OnItemDataBound="rptRating_ItemDataBound">
            <ItemTemplate>
                <div class="row mb-3 exit-survey-rating">
                    <div class="col-md-4 exit-survey-rating-label">
                        <asp:Literal ID="ltLabel" runat="server"></asp:Literal>
                        <asp:HiddenField ID="hfKey" runat="server" />
                    </div>
                    <div class="col-md-8">
                        <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="rating-options"></asp:RadioButtonList>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <span class="es-error label label-danger d-inline-block"></span>
        </div>
        <asp:Panel ID="divQ1Other" runat="server" CssClass="row mb-3 q1-other d-none">
            <div class="col-auto"><label for="<%= txtQ1Other.ClientID %>">Other (specify)</label></div>
            <div class="col-md-8"><asp:TextBox ID="txtQ1Other" runat="server" CssClass="form-control" MaxLength="255" /></div>
        </asp:Panel>

        <%-- Q2 -------------------------------------------------------------- --%>
        <div class="mb-3 es-field" data-vtype="checks" data-vlabel="Question 2 &ndash; advantages of the new employer" data-vmsg="Select at least one option (or &quot;Not Accepting employment elsewhere&quot;).">
            <span class="exit-survey-question">2. If you are accepting employment elsewhere, what advantages do you feel the new employer offers that you have not found here at the Twelfth Judicial Circuit? (Select all that apply.)</span>
            <asp:CheckBoxList ID="cblAdvantages" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="es-check-list q2-list" />
            <asp:Panel ID="divQ2Other" runat="server" CssClass="row mt-2 q2-other d-none">
                <div class="col-auto"><label for="<%= txtQ2Other.ClientID %>">Other (specify)</label></div>
                <div class="col-md-8"><asp:TextBox ID="txtQ2Other" runat="server" CssClass="form-control" MaxLength="255" /></div>
            </asp:Panel>
            <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Q3 -------------------------------------------------------------- --%>
        <div class="mb-3 es-field" data-vtype="radio" data-vlabel="Question 3 &ndash; accepted another position?" data-vmsg="Please select Yes or No.">
            <span class="exit-survey-question">3. Have you accepted another position?</span>
            <asp:RadioButtonList ID="rblAccepted" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="rating-options q3-list">
                <asp:ListItem Text="Yes" Value="1" />
                <asp:ListItem Text="No" Value="0" />
            </asp:RadioButtonList>
            <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Q4 -------------------------------------------------------------- --%>
        <div class="mb-3 es-field" data-vtype="radio" data-vcond="accepted" data-vlabel="Question 4 &ndash; type of employer" data-vmsg="Please select the type of employer.">
            <span class="exit-survey-question">4. If yes, what type of employer?</span>
            <asp:RadioButtonList ID="rblEmployerType" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="rating-options q4-list" />
            <asp:Panel ID="divQ4Other" runat="server" CssClass="row mt-2 q4-other d-none">
                <div class="col-auto"><label for="<%= txtQ4Other.ClientID %>">Other (specify)</label></div>
                <div class="col-md-8"><asp:TextBox ID="txtQ4Other" runat="server" CssClass="form-control" MaxLength="255" /></div>
            </asp:Panel>
            <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Q5 -------------------------------------------------------------- --%>
        <div class="mb-3 es-field" data-vtype="checks" data-vlabel="Question 5 &ndash; reasons you left" data-vmsg="Select at least one reason you left.">
            <span class="exit-survey-question">5. What influenced you to leave the Twelfth Judicial Circuit? (Select all that apply.)</span>
            <asp:CheckBoxList ID="cblReasons" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="es-check-list q5-list" />
            <asp:Panel ID="divQ5Other" runat="server" CssClass="row mt-2 q5-other d-none">
                <div class="col-auto"><label for="<%= txtQ5Other.ClientID %>">Other (specify)</label></div>
                <div class="col-md-8"><asp:TextBox ID="txtQ5Other" runat="server" CssClass="form-control" MaxLength="255" /></div>
            </asp:Panel>
            <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Q6 -------------------------------------------------------------- --%>
        <div class="exit-survey-question">6. Please indicate your feelings about the supervision you received while employed with the Twelfth Judicial Circuit.</div>
        <div class="es-matrix" data-vlabel="Question 6 &ndash; rate every supervision item">
        <asp:Repeater ID="rptSupervision" runat="server" OnItemDataBound="rptRating_ItemDataBound">
            <ItemTemplate>
                <div class="row mb-3 exit-survey-rating">
                    <div class="col-md-4 exit-survey-rating-label">
                        <asp:Literal ID="ltLabel" runat="server"></asp:Literal>
                        <asp:HiddenField ID="hfKey" runat="server" />
                    </div>
                    <div class="col-md-8">
                        <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="rating-options"></asp:RadioButtonList>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Q7 -------------------------------------------------------------- --%>
        <div class="mb-3 es-field" data-vtype="text" data-vlabel="Question 7 &ndash; training / resources" data-vmsg="This field is required.">
            <label class="exit-survey-question" for="<%= txtQ7.ClientID %>">7. Did you receive adequate training and/or other resources necessary to accomplish your job? If not, please provide details.</label>
            <asp:TextBox ID="txtQ7" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
            <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Q8 -------------------------------------------------------------- --%>
        <div class="mb-3 es-field" data-vtype="text" data-vlabel="Question 8 &ndash; comments" data-vmsg="This field is required.">
            <label class="exit-survey-question" for="<%= txtQ8.ClientID %>">8. Please comment on what you feel could be done to help make the Twelfth Judicial Circuit a better place to work.</label>
            <asp:TextBox ID="txtQ8" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
            <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Q9 -------------------------------------------------------------- --%>
        <div class="mb-3 es-field" data-vtype="radio" data-vlabel="Question 9 &ndash; would you return?" data-vmsg="Please select Yes or No.">
            <span class="exit-survey-question">9. Would you consider working for the Twelfth Judicial Circuit again in the future?</span>
            <asp:RadioButtonList ID="rblWouldReturn" runat="server" RepeatDirection="Vertical" RepeatLayout="UnorderedList" CssClass="rating-options">
                <asp:ListItem Text="Yes" Value="1" />
                <asp:ListItem Text="No" Value="0" />
            </asp:RadioButtonList>
            <span class="es-error label label-danger d-inline-block"></span>
        </div>

        <%-- Personal Info section ------------------------------------------- --%>
        <div class="exit-survey-section">Personal Info</div>
        <div class="row mb-3">
            <div class="col-md-6 es-field" data-vtype="text" data-vlabel="Personal Info &ndash; Name" data-vmsg="This field is required.">
                <label class="exit-survey-req" for="<%= txtName.ClientID %>">Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="150" />
                <span class="es-error label label-danger d-inline-block"></span>
            </div>
            <div class="col-md-6 es-field" data-vtype="text" data-vlabel="Personal Info &ndash; Most recent position title" data-vmsg="This field is required.">
                <label class="exit-survey-req" for="<%= txtPositionTitle.ClientID %>">Most recent position title in Court employment</label>
                <asp:TextBox ID="txtPositionTitle" runat="server" CssClass="form-control" MaxLength="150" />
                <span class="es-error label label-danger d-inline-block"></span>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-6 es-field" data-vtype="text" data-vlabel="Personal Info &ndash; Name of Supervisor" data-vmsg="This field is required.">
                <label class="exit-survey-req" for="<%= txtSupervisorName.ClientID %>">Name of Supervisor</label>
                <asp:TextBox ID="txtSupervisorName" runat="server" CssClass="form-control" MaxLength="150" />
                <span class="es-error label label-danger d-inline-block"></span>
            </div>
            <div class="col-md-6 es-field" data-vtype="text" data-vlabel="Personal Info &ndash; Supervisor's Title" data-vmsg="This field is required.">
                <label class="exit-survey-req" for="<%= txtSupervisorTitle.ClientID %>">Supervisor's Title</label>
                <asp:TextBox ID="txtSupervisorTitle" runat="server" CssClass="form-control" MaxLength="150" />
                <span class="es-error label label-danger d-inline-block"></span>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-4 es-field" data-vtype="text" data-vlabel="Personal Info &ndash; Month/Year you were hired" data-vmsg="This field is required.">
                <label class="exit-survey-req" for="<%= txtHired.ClientID %>">Month/Year you were hired at the Court</label>
                <asp:TextBox ID="txtHired" runat="server" CssClass="form-control" MaxLength="50" />
                <span class="es-error label label-danger d-inline-block"></span>
            </div>
            <div class="col-md-4 es-field" data-vtype="text" data-vlabel="Personal Info &ndash; Month/Year you separated" data-vmsg="This field is required.">
                <label class="exit-survey-req" for="<%= txtSeparated.ClientID %>">Month/Year you separated</label>
                <asp:TextBox ID="txtSeparated" runat="server" CssClass="form-control" MaxLength="50" />
                <span class="es-error label label-danger d-inline-block"></span>
            </div>
            <div class="col-md-4 es-field" data-vtype="text" data-vlabel="Personal Info &ndash; Date Exit Survey Completed" data-vmsg="This field is required.">
                <label class="exit-survey-req" for="<%= txtDateCompleted.ClientID %>">Date Exit Survey Completed</label>
                <asp:TextBox ID="txtDateCompleted" runat="server" CssClass="form-control" MaxLength="50" />
                <span class="es-error label label-danger d-inline-block"></span>
            </div>
        </div>

        <hr />
        <p>
            <asp:Button ID="cmdSubmit" runat="server" CssClass="btn btn-primary" OnClick="cmdSubmit_Click" OnClientClick="return validateExitSurvey();" Text="Submit Survey" />
        </p>

        <p>Thank you for participating in the Employee Exit Survey and telling us about your experience working for the State Courts System. Please return the completed survey to: Human Resources Office, Court Administration, Judge Lynn N. Silvertooth Judicial Center, 8th Floor, 2002 Ringling Blvd., Sarasota, FL 34237</p>

        <p>Best of luck in your future endeavors!</p>

    </asp:Panel>

</div>

<script type="text/javascript">
    (function () {
        function isOtherChecked(list) {
            var labels = list.querySelectorAll("label");
            for (var i = 0; i < labels.length; i++) {
                if (/^\s*Other/i.test(labels[i].textContent)) {
                    var input = document.getElementById(labels[i].getAttribute("for"));
                    if (input) return input.checked;
                }
            }
            return false;
        }
        // Show an "Other (specify)" panel only while its Other option is selected.
        function wireOther(listSelector, otherSelector) {
            var list = document.querySelector(listSelector);
            var other = document.querySelector(otherSelector);
            if (!list || !other) return;
            var inputs = list.querySelectorAll("input");
            function toggle() { other.classList.toggle("d-none", !isOtherChecked(list)); }
            for (var i = 0; i < inputs.length; i++) inputs[i].addEventListener("change", toggle);
            toggle();
        }
        // Q1's "Other (specify)" free text shows once its matrix row is rated.
        function wireQ1() {
            var trigger = document.querySelector(".q1-other-trigger");
            var other = document.querySelector(".q1-other");
            if (!trigger || !other) return;
            var inputs = trigger.querySelectorAll("input");
            function toggle() {
                var any = false;
                for (var i = 0; i < inputs.length; i++) if (inputs[i].checked) any = true;
                other.classList.toggle("d-none", !any);
            }
            for (var i = 0; i < inputs.length; i++) inputs[i].addEventListener("change", toggle);
            toggle();
        }
        function init() {
            wireOther(".q2-list", ".q2-other");
            wireOther(".q4-list", ".q4-other");
            wireOther(".q5-list", ".q5-other");
            wireQ1();
            wireClearOnChange();
        }
        // Hide a field's error as soon as the user starts fixing it.
        function wireClearOnChange() {
            var root = document.querySelector(".exit-survey");
            if (!root) return;
            var handler = function (e) {
                var c = e.target.closest(".es-field, .es-matrix");
                if (!c) return;
                var err = c.querySelector(".es-error");
                if (err) err.textContent = "";
                c.classList.remove("es-invalid");
            };
            root.addEventListener("change", handler);
            root.addEventListener("input", handler);
        }
        if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", init);
        else init();
    })();

    // Client-side required-field check. Shows an inline message on each missing
    // field and a SweetAlert2 summary listing everything that still needs attention.
    // Returns false to cancel the postback when anything is missing.
    function validateExitSurvey() {
        var root = document.querySelector(".exit-survey");
        if (!root) return true;

        root.querySelectorAll(".es-error").forEach(function (el) { el.textContent = ""; });
        root.querySelectorAll(".es-invalid").forEach(function (el) { el.classList.remove("es-invalid"); });

        var missing = [];
        function fail(container, msg, label) {
            var err = container.querySelector(".es-error");
            if (err) err.textContent = msg;
            container.classList.add("es-invalid");
            if (label) missing.push(label);
        }

        // Q3 drives whether Q2 (advantages) and Q4 (employer type) apply.
        var accepted = false;
        var q3 = root.querySelector(".q3-list");
        if (q3) {
            var q3sel = q3.querySelector("input[type=radio]:checked");
            accepted = !!(q3sel && q3sel.value === "1");
        }

        root.querySelectorAll(".es-matrix, .es-field").forEach(function (c) {
            if (c.classList.contains("es-matrix")) {
                var ok = true;
                c.querySelectorAll(".exit-survey-rating").forEach(function (row) {
                    if (row.querySelector(".q1-other-trigger")) return; // Q1 "Other" row is optional
                    if (!row.querySelector("input[type=radio]:checked")) ok = false;
                });
                if (!ok) fail(c, "Please rate every item.", c.getAttribute("data-vlabel"));
                return;
            }
            if (c.getAttribute("data-vcond") === "accepted" && !accepted) return; // not applicable
            var type = c.getAttribute("data-vtype");
            var good = true;
            if (type === "radio") good = !!c.querySelector("input[type=radio]:checked");
            else if (type === "checks") good = !!c.querySelector("input[type=checkbox]:checked");
            else if (type === "text") {
                var t = c.querySelector("textarea, input[type=text]");
                good = !!(t && t.value.trim() !== "");
            }
            if (!good) fail(c, c.getAttribute("data-vmsg") || "This field is required.", c.getAttribute("data-vlabel"));
        });

        if (missing.length) {
            var first = root.querySelector(".es-invalid");
            function scrollToFirst() {
                if (first && first.scrollIntoView) first.scrollIntoView({ behavior: "smooth", block: "start" });
            }
            var list = missing.map(function (m) { return "<li>" + m + "</li>"; }).join("");
            if (window.Swal) {
                // returnFocus:false stops SweetAlert from jumping back to the submit
                // button on close; then scroll the first flagged field to the top.
                Swal.fire({
                    title: "Some items still need your attention",
                    icon: "error",
                    html: '<p>Please complete the following before submitting:</p><ul style="text-align:left;">' + list + "</ul>",
                    confirmButtonText: "OK",
                    returnFocus: false
                }).then(scrollToFirst);
            } else {
                alert("Please complete all required items before submitting.");
                scrollToFirst();
            }
            return false;
        }
        return true;
    }
</script>
