using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Port of AWS.SWN.API.SWNServiceRequests from
    /// D:\websites\Intranet\App_Code\EmployeeDB\SWNServiceRequests.vb.
    ///
    /// Wraps the SWN Users SOAP service for the employee sync flow: pulls
    /// Employee/Phone/Group data from the module's controllers, shapes it
    /// into SWN ContactDetailRequest/GrpCreateReq/etc., and forwards to
    /// <see cref="ClientFactory.CreateSWNOnlineProxy"/>.
    ///
    /// Generated WCF proxy lives at Service References\\swn\\Reference.cs (produced by svcutil from Users.wsdl)./// </summary>
    public class SWNServiceRequests
    {
        private const string PhoneCountryCode = "1";

        private readonly UsersClient _service;

        public SWNServiceRequests() : this(null, null) { }

        public SWNServiceRequests(string username, string password)
        {
            _service = ClientFactory.CreateSWNOnlineProxy(username, password);
        }

        private readonly EmployeeController _employees = new EmployeeController();
        private readonly PhoneController _phones = new PhoneController();
        private readonly GroupController _groups = new GroupController();
        private readonly GroupMembershipController _memberships = new GroupMembershipController();
        private readonly CountyController _counties = new CountyController();
        private readonly OfficeLocationController _locations = new OfficeLocationController();
        private readonly SwnInterfaceLogController _log = new SwnInterfaceLogController();

        private const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        #region Public methods

        /// <summary>
        /// Ports BlockAddContact(List(Of Employee.Employee)) from
        /// SWNServiceRequests.vb (lines 12-35).
        /// Bulk-creates contacts in SWN for the supplied employees.
        /// </summary>
        public SWNResponse BlockAddContact(List<EmployeeInfo> employees)
        {
            try
            {
                ObjActionResult[] result = null;
                var count = 0;
                foreach (var e in employees)
                {
                    var contact = PopulateContact(e);
                    if (count == 0)
                    {
                        result = AddContact(contact, e);
                    }
                    else
                    {
                        // VB: result.Concat(AddContact(...)) — preserves quirk
                        // where the return was discarded. Keeping semantics.
                        result = result.Concat(AddContact(contact, e)).ToArray();
                    }
                    count++;
                }
                return ProcessResponse(result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// Ports BlockUpdateContacts() from SWNServiceRequests.vb (lines 37-60).
        /// Syncs all active employees into SWN, deleting any SWN contact ID
        /// not present in the active employee list.
        /// </summary>
        public SWNResponse BlockUpdateContacts()
        {
            try
            {
                var result = new List<SWNResponse>();
                AddAllGroups();

                var employeeList = GetActiveEmployees();
                var employeeIds = employeeList.Select(e => e.EmployeeId.ToString()).ToList();
                var swnList = GetContactIds().Select(g => g.ToString()).Except(employeeIds).ToList();
                var deleteResult = ProcessResponse(_service.DeleteContacts(swnList.ToArray()));
                result.Add(deleteResult);

                foreach (var e in employeeList)
                {
                    var resp = AddUpdateContact(e);
                    StampEmployeeOnMessages(resp, e);
                    result.Add(resp);
                }
                return ProcessResponse(result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>Add only the active employees who don't already have an
        /// SWN contact. Mirrors the legacy BlockAddContact() but takes the
        /// pre-filtered "missing" list from the caller (so the membership
        /// check happens in one place — the SwnController endpoint).</summary>
        public SWNResponse BlockAddMissing(IList<EmployeeInfo> missingEmployees)
        {
            try
            {
                var result = new List<SWNResponse>();
                AddAllGroups();   // ensure SWN-side groups exist before assigning members

                foreach (var e in missingEmployees ?? new List<EmployeeInfo>())
                {
                    var contact = PopulateContact(e);
                    var addResult = AddContact(contact, e);
                    var resp = ProcessResponse(addResult);
                    StampEmployeeOnMessages(resp, e);
                    result.Add(resp);
                }
                return ProcessResponse(result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>Prefix every message in <paramref name="response"/> with
        /// the employee's name + ID so the aggregated SWN Sync output makes
        /// it clear which contact each line refers to. Without this stamp
        /// the SWN service messages are just "Valid Contact" / "Contact
        /// updated successfully" / "Maximum length for Address1 is 60" with
        /// no per-row attribution.</summary>
        private static void StampEmployeeOnMessages(SWNResponse response, EmployeeInfo emp)
        {
            if (response == null || response.MessageList == null || emp == null) return;
            var label = (emp.DisplayName ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(label)) label = "Employee";
            label = label + " (#" + emp.EmployeeId + "): ";
            foreach (var m in response.MessageList)
            {
                m.MessageText = label + (m.MessageText ?? string.Empty);
            }
        }

        /// <summary>
        /// Ports GetContactIds() from SWNServiceRequests.vb (lines 62-71).
        /// Returns the numeric IDs of all contacts in the SWN "MASTER" group.
        /// </summary>
        public List<int> GetContactIds()
        {
            var contactIds = new List<int>();
            var grpDetails = _service.GetGroupDetail("MASTER");
            if (grpDetails != null && grpDetails.ContactsList != null)
            {
                foreach (var c in grpDetails.ContactsList)
                {
                    int parsed;
                    if (int.TryParse(c.Id, out parsed))
                    {
                        contactIds.Add(parsed);
                    }
                }
            }
            return contactIds;
        }

        /// <summary>
        /// Ports EmployeeExists(employeeid) from SWNServiceRequests.vb
        /// (lines 73-80). Returns true if the employee ID already exists as
        /// a contact in SWN.
        /// </summary>
        public bool EmployeeExists(string employeeid)
        {
            try
            {
                return _service.IsContactInAccount(employeeid);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Ports AddUpdateContact(objEmployee) from SWNServiceRequests.vb
        /// (lines 133-168). Creates/updates/deletes the contact based on
        /// whether it already exists and whether the employee is active.
        /// </summary>
        public SWNResponse AddUpdateContact(EmployeeInfo objEmployee)
        {
            try
            {
                ObjActionResult[] result = null;
                var exists = _service.IsContactInAccount(objEmployee.EmployeeId.ToString());
                var grpResponse = new SWNResponse();
                var active = objEmployee.IsActive ?? false;

                if (exists && active)
                {
                    var contact = PopulateContact(objEmployee);
                    result = _service.UpdateContact(contact);
                    grpResponse = UpdateContactGroups(objEmployee);
                }
                else if (exists && !active)
                {
                    var contactid = new[] { objEmployee.EmployeeId.ToString() };
                    result = _service.DeleteContacts(contactid);
                }
                else if (!exists && active)
                {
                    var contact = PopulateContact(objEmployee);
                    result = AddContact(contact, objEmployee);
                }

                if (grpResponse.HasErrors)
                {
                    var resp = ProcessResponse(result);
                    resp.HasErrors = true;
                    resp.MessageList.AddRange(grpResponse.MessageList);
                    return resp;
                }
                return ProcessResponse(result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(
                    SWNResponseMessageType.Failure,
                    ex.Message + " id:" + objEmployee.EmployeeId);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// Ports DeleteContact(employeeid) from SWNServiceRequests.vb
        /// (lines 212-226).
        /// </summary>
        public SWNResponse DeleteContact(string employeeid)
        {
            try
            {
                var contacts = new[] { employeeid };
                var deleteResult = _service.DeleteContacts(contacts);
                return ProcessResponse(deleteResult);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// Ports AddAllGroups() from SWNServiceRequests.vb (lines 228-272).
        /// Creates any SWN group not already present and populates its
        /// members from the local GroupMemberships table.
        /// </summary>
        public SWNResponse AddAllGroups()
        {
            try
            {
                ObjActionResult[] addResult = null;
                var requestGrp = _service.GetGroups(false);
                var swnGrps = new List<int>();
                if (requestGrp != null)
                {
                    foreach (var g in requestGrp)
                    {
                        int parsed;
                        if (int.TryParse(g.Name, out parsed))
                        {
                            swnGrps.Add(parsed);
                        }
                    }
                }

                var groupList = GetSwnGroupsMissingFrom(swnGrps);

                if (groupList.Count > 0)
                {
                    foreach (var g in groupList)
                    {
                        var group = new GrpCreateReq
                        {
                            Name = g.GroupID.ToString(),
                            Desc = g.GroupName
                        };
                        addResult = _service.CreateGroup(group);

                        var usersInGroup = GetGroupMembershipIds(g.GroupID);
                        var grpAction = new GrpActionContactsReq
                        {
                            ActionType = GrpActionContactsReqActionType.Add,
                            GroupName = g.GroupID.ToString(),
                            ContactIdsList = usersInGroup.Select(u => u.ToString()).ToArray()
                        };
                        // VB: addResult.Concat(...) — return discarded on purpose.
                        addResult = addResult == null
                            ? _service.ActionContactsGroup(grpAction)
                            : addResult.Concat(_service.ActionContactsGroup(grpAction)).ToArray();
                    }
                    return ProcessResponse(addResult);
                }
                return new SWNResponse(false, new List<SWNResponseMessage>());
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// Ports AddUpdateGroup(objGroup, Optional oldGroup) from
        /// SWNServiceRequests.vb (lines 274-320). When oldGroup is blank,
        /// creates a new group and adds its members; otherwise updates the
        /// existing SWN group's name/description.
        /// </summary>
        public SWNResponse AddUpdateGroup(GroupInfo objGroup, string oldGroup = null)
        {
            try
            {
                if (string.IsNullOrEmpty(oldGroup))
                {
                    var group = new GrpCreateReq
                    {
                        Name = objGroup.GroupID.ToString(),
                        Desc = objGroup.GroupName
                    };
                    var addResult = _service.CreateGroup(group);

                    var usersInGroup = GetGroupMembershipIds(objGroup.GroupID);
                    var grpAction = new GrpActionContactsReq
                    {
                        ActionType = GrpActionContactsReqActionType.Add,
                        GroupName = objGroup.GroupID.ToString(),
                        ContactIdsList = usersInGroup.Select(u => u.ToString()).ToArray()
                    };
                    // VB: AddResult.Concat(...) — return discarded.
                    addResult = addResult == null
                        ? _service.ActionContactsGroup(grpAction)
                        : addResult.Concat(_service.ActionContactsGroup(grpAction)).ToArray();
                    return ProcessResponse(addResult);
                }
                else
                {
                    var groupNewDesc = new GrpUpdReqNewDesc
                    {
                        Clear = false,
                        Value = objGroup.GroupName
                    };
                    var group = new GrpUpdReq
                    {
                        Name = oldGroup,
                        NewDesc = groupNewDesc,
                        NewName = objGroup.GroupID.ToString()
                    };
                    var updateResult = _service.UpdateGroup(group);
                    return ProcessResponse(updateResult);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// Ports DeleteGroup(groupId) from SWNServiceRequests.vb (lines
        /// 322-334).
        /// </summary>
        public SWNResponse DeleteGroup(string groupId)
        {
            try
            {
                var result = _service.DeleteGroup(groupId);
                return ProcessResponse(result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// Ports ManageGroupContact(groupId, employeeId, Action) from
        /// SWNServiceRequests.vb (lines 336-356). Adds or removes a single
        /// contact to/from a group.
        /// </summary>
        public SWNResponse ManageGroupContact(string groupId, string employeeId, GrpActionContactsReqActionType action)
        {
            try
            {
                var grpAction = new GrpActionContactsReq
                {
                    ActionType = action,
                    GroupName = groupId,
                    ContactIdsList = new[] { employeeId }
                };
                var result = _service.ActionContactsGroup(grpAction);
                return ProcessResponse(result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// String-based overload that accepts the action as "Add" or "Remove"
        /// so callers don't need to reference the generated enum. Kept for
        /// signature parity with the task spec.
        /// </summary>
        public SWNResponse ManageGroupContact(string groupId, string employeeId, string action)
        {
            var parsed = string.Equals(action, "Remove", StringComparison.OrdinalIgnoreCase)
                ? GrpActionContactsReqActionType.Remove
                : GrpActionContactsReqActionType.Add;
            return ManageGroupContact(groupId, employeeId, parsed);
        }

        /// <summary>
        /// Ports CreateCustomFields() from SWNServiceRequests.vb (lines
        /// 358-388). Creates the Department/Location/Title/County custom
        /// field definitions in SWN.
        /// </summary>
        public SWNResponse CreateCustomFields()
        {
            try
            {
                var defs = new List<ContactCustomFieldDefinition>
                {
                    new ContactCustomFieldDefinition { Name = "Department" },
                    new ContactCustomFieldDefinition { Name = "Location" },
                    new ContactCustomFieldDefinition { Name = "Title" },
                    new ContactCustomFieldDefinition { Name = "County" }
                };
                var result = _service.SetContactCustomFieldsDefinition(defs.ToArray());
                return ProcessResponse(result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(SWNResponseMessageType.Failure, ex.Message);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        #endregion

        #region Private helpers

        /// <summary>
        /// Ports private UpdateContactGroups(objEmployee) from
        /// SWNServiceRequests.vb (lines 83-131). Diffs SWN group membership
        /// against local GroupMembership rows (for SWN-flagged groups only)
        /// and issues Add/Remove calls to reconcile.
        /// </summary>
        private SWNResponse UpdateContactGroups(EmployeeInfo objEmployee)
        {
            try
            {
                var request = _service.GetContactGroups(objEmployee.EmployeeId.ToString());
                var response = new SWNResponse { MessageList = new List<SWNResponseMessage>() };
                var swnGrps = new List<int>();

                var allGrps = GetSwnGroupIds();
                var empGrps = GetEmployeeGroupIds(objEmployee.EmployeeId)
                    .Where(g => allGrps.Contains(g))
                    .ToList();

                if (request != null)
                {
                    foreach (var g in request)
                    {
                        int groupID;
                        if (int.TryParse(g.Name, out groupID))
                        {
                            swnGrps.Add(groupID);
                        }
                    }
                }

                var delGrps = swnGrps.Except(empGrps).ToList();
                var addGrps = empGrps.Except(swnGrps).ToList();

                foreach (var g in addGrps)
                {
                    var r = ManageGroupContact(g.ToString(), objEmployee.EmployeeId.ToString(), GrpActionContactsReqActionType.Add);
                    if (r.HasErrors)
                    {
                        response.HasErrors = true;
                        if (r.MessageList != null)
                        {
                            response.MessageList.AddRange(r.MessageList);
                        }
                    }
                }

                foreach (var g in delGrps)
                {
                    var r = ManageGroupContact(g.ToString(), objEmployee.EmployeeId.ToString(), GrpActionContactsReqActionType.Remove);
                    if (r.HasErrors)
                    {
                        response.HasErrors = true;
                        if (r.MessageList != null)
                        {
                            response.MessageList.AddRange(r.MessageList);
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                LogException(ex);
                var errorMessage = new SWNResponseMessage(
                    SWNResponseMessageType.Failure,
                    ex.Message + " id:" + objEmployee.EmployeeId);
                var errorList = new List<SWNResponseMessage> { errorMessage };
                return new SWNResponse(true, errorList);
            }
        }

        /// <summary>
        /// Ports private AddContact(contact, newEmployee) from
        /// SWNServiceRequests.vb (lines 170-184). Creates the contact in SWN
        /// then folds in any SWN-group memberships.
        /// </summary>
        private ObjActionResult[] AddContact(ContactDetailRequest contact, EmployeeInfo newEmployee)
        {
            var contactList = new List<ContactDetailRequest> { contact };
            var result = _service.CreateContacts(contactList.ToArray());

            foreach (var grpId in GetEmployeeGroupIds(newEmployee.EmployeeId))
            {
                var grp = GetGroup(grpId);
                if (grp != null && grp.IsSwnGroup)
                {
                    // VB: result.Concat(AddContactGroups(...)) — return discarded.
                    result = result == null
                        ? AddContactGroups(grpId.ToString(), newEmployee.EmployeeId.ToString())
                        : result.Concat(AddContactGroups(grpId.ToString(), newEmployee.EmployeeId.ToString())).ToArray();
                }
            }
            return result;
        }

        /// <summary>
        /// Ports private AddContactGroups(groupId, employeeId) from
        /// SWNServiceRequests.vb (lines 186-210). Wraps ActionContactsGroup
        /// with a local try/catch that manufactures an ObjActionResult on
        /// failure so the calling Concat() still sees something usable.
        /// </summary>
        private ObjActionResult[] AddContactGroups(string groupId, string employeeId)
        {
            try
            {
                var grpAction = new GrpActionContactsReq
                {
                    ActionType = GrpActionContactsReqActionType.Add,
                    GroupName = groupId,
                    ContactIdsList = new[] { employeeId }
                };
                return _service.ActionContactsGroup(grpAction);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var detail = new ObjActionResultDetailInfo
                {
                    Type = ObjActionResultDetailInfoType.Error,
                    Desc = ex.Message
                };
                var exResult = new ObjActionResult { Details = new[] { detail } };
                return new[] { exResult };
            }
        }

        /// <summary>
        /// Ports private ProcessResponse(result) from SWNServiceRequests.vb
        /// (lines 390-448). Normalises the various shapes of SWN result
        /// objects into a single <see cref="SWNResponse"/>.
        /// </summary>
        private SWNResponse ProcessResponse(object result)
        {
            var response = new SWNResponse();
            if (result == null)
            {
                response.HasErrors = false;
                return response;
            }
            response.MessageList = new List<SWNResponseMessage>();

            if (result is ObjActionResult)
            {
                var objactionResult = (ObjActionResult)result;
                AppendDetails(response, objactionResult.Details);
            }
            else if (result is ObjActionResult[])
            {
                foreach (var r in (ObjActionResult[])result)
                {
                    AppendDetails(response, r.Details);
                }
            }
            else if (result is ObjActionResult1)
            {
                var objactionResult = (ObjActionResult1)result;
                AppendDetails(response, objactionResult.Details);
            }
            else if (result is ObjActionResult1[])
            {
                foreach (var r in (ObjActionResult1[])result)
                {
                    AppendDetails(response, r.Details);
                }
            }
            else if (result is List<SWNResponse>)
            {
                foreach (var r in (List<SWNResponse>)result)
                {
                    if (r.HasErrors)
                    {
                        response.HasErrors = true;
                    }
                    if (r.MessageList != null)
                    {
                        response.MessageList.AddRange(r.MessageList);
                    }
                }
            }
            else
            {
                response.HasErrors = true;
                response.MessageList.Add(new SWNResponseMessage(SWNResponseMessageType.Failure, "Undefined Result Type"));
            }
            return response;
        }

        private void AppendDetails(SWNResponse response, ObjActionResultDetailInfo[] details)
        {
            if (details == null)
            {
                return;
            }
            foreach (var d in details)
            {
                if (d.Type == ObjActionResultDetailInfoType.Error)
                {
                    response.HasErrors = true;
                }
                response.MessageList.Add(new SWNResponseMessage(MapDetailType(d.Type), d.Desc));
            }
        }

        private static SWNResponseMessageType MapDetailType(ObjActionResultDetailInfoType t)
        {
            switch (t)
            {
                case ObjActionResultDetailInfoType.Error:
                    return SWNResponseMessageType.Failure;
                case ObjActionResultDetailInfoType.Warning:
                    return SWNResponseMessageType.Warning;
                default:
                    return SWNResponseMessageType.Information;
            }
        }

        /// <summary>
        /// Ports private GetPhoneSMS(phone) from SWNServiceRequests.vb
        /// (lines 450-460). Builds the SMS contact-point for cell/mobile
        /// phones; returns a blank ContactPntTextMsgType for other types.
        /// </summary>
        private ContactPntTextMsgType GetPhoneSMS(PhoneInfo phone)
        {
            var contactText = new ContactPntTextMsgType();
            if (phone.PhoneType == null)
            {
                return contactText;
            }
            var type = phone.PhoneType.ToLower().Trim();
            if (type.Contains("mobile") || type.Contains("cell"))
            {
                contactText.Carrier = "SWN Global SMS"; // sms.sendwordnow.com
                contactText.Label = phone.PhoneType;
                contactText.Number = PhoneCountryCode + phone.PhoneNumber;
            }
            return contactText;
        }

        /// <summary>
        /// Ports private GetPhone(phone) from SWNServiceRequests.vb (lines
        /// 462-487). Builds the voice contact-point, falling back to "Other"
        /// when the phone type is missing, skipping the extension when the
        /// SWNExcludeExtension flag is set or the extension is non-numeric.
        /// </summary>
        private ContactPntVoiceType GetPhone(PhoneInfo phone)
        {
            try
            {
                var contactPhone = new ContactPntVoiceType
                {
                    Number = phone.PhoneNumber,
                    Label = !string.IsNullOrEmpty(phone.PhoneType) ? phone.PhoneType : "Other",
                    CountryCode = PhoneCountryCode
                };

                if (phone.PhoneCascade.HasValue && phone.PhoneCascade.Value > 0)
                {
                    contactPhone.CascadeOrder = phone.PhoneCascade.Value;
                }

                if (!string.IsNullOrEmpty(phone.Extension) && !phone.SwnExcludeExtension)
                {
                    int _;
                    if (int.TryParse(phone.Extension, out _))
                    {
                        contactPhone.Extension = phone.Extension;
                    }
                }
                return contactPhone;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Ports private PopulateContact(objEmployee) from
        /// SWNServiceRequests.vb (lines 489-593). Assembles a
        /// ContactDetailRequest from the module's EmployeeInfo + phones +
        /// department/location/title/county custom fields.
        /// </summary>
        private ContactDetailRequest PopulateContact(EmployeeInfo objEmployee)
        {
            var contact = new ContactDetailRequest();
            var customFieldsList = new List<ContactCustomField>();
            var contactPointsList = new ContactDetailContactPointsRequest();
            var phonelist = new List<ContactPntVoiceType>();
            var textlist = new List<ContactPntTextMsgType>();
            var emailList = new List<ContactPntEmailType>();

            try
            {
                var phones = GetEmployeePhones(objEmployee.EmployeeId);
                foreach (var p in phones)
                {
                    // The VB original used IsNumeric() here, which has no
                    // size limit. The earlier C# port used int.TryParse,
                    // which silently rejected any 10-digit number greater
                    // than ~2.1 billion (e.g. 9415551234 — every Sarasota /
                    // Tampa area code) — that's why no phones were making
                    // it through to SWN. long.TryParse handles the full
                    // 10-digit range and matches IsNumeric semantics.
                    if (p.SwnCall)
                    {
                        long _;
                        if (!string.IsNullOrEmpty(p.PhoneNumber) && long.TryParse(p.PhoneNumber, out _))
                        {
                            var built = GetPhone(p);
                            if (built != null)
                            {
                                phonelist.Add(built);
                            }
                        }
                    }
                    if (p.SwnText)
                    {
                        if (p.PhoneType == null)
                        {
                            p.PhoneType = "Other";
                        }
                        long _;
                        var type = p.PhoneType.ToLower().Trim();
                        if (!string.IsNullOrEmpty(p.PhoneNumber)
                            && long.TryParse(p.PhoneNumber, out _)
                            && (type.Contains("mobile") || type.Contains("cell")))
                        {
                            textlist.Add(GetPhoneSMS(p));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(objEmployee.Email) && IsEmail(objEmployee.Email))
                {
                    emailList.Add(new ContactPntEmailType { Address = objEmployee.Email, Label = "Work" });
                }
                if (!string.IsNullOrEmpty(objEmployee.PersonalEmail) && IsEmail(objEmployee.PersonalEmail))
                {
                    emailList.Add(new ContactPntEmailType { Address = objEmployee.PersonalEmail, Label = "Home" });
                }

                if (emailList.Count > 0) contactPointsList.EmailContactPoints = emailList.ToArray();
                if (textlist.Count > 0) contactPointsList.TextMessageContactPoints = textlist.ToArray();
                if (phonelist.Count > 0) contactPointsList.VoiceContactPoints = phonelist.ToArray();

                // Custom fields
                var departmentName = GetDepartmentName(objEmployee.DepartmentId);
                if (!string.IsNullOrEmpty(departmentName))
                {
                    customFieldsList.Add(new ContactCustomField { Name = "Department", Value = departmentName });
                }
                if (!string.IsNullOrEmpty(objEmployee.JobTitle))
                {
                    customFieldsList.Add(new ContactCustomField { Name = "Title", Value = objEmployee.JobTitle });
                }
                var countyName = GetCountyName(objEmployee.CountyId);
                if (!string.IsNullOrEmpty(countyName))
                {
                    customFieldsList.Add(new ContactCustomField { Name = "County", Value = countyName });
                }
                if (objEmployee.OfficeLocationId.HasValue)
                {
                    var loc = _locations.GetById(objEmployee.OfficeLocationId.Value);
                    if (loc != null && !string.IsNullOrEmpty(loc.Description))
                    {
                        customFieldsList.Add(new ContactCustomField { Name = "Location", Value = loc.Description });
                    }
                }

                // Address/identity fields
                if (!string.IsNullOrEmpty(objEmployee.Address)) contact.Address1 = objEmployee.Address;
                if (!string.IsNullOrEmpty(objEmployee.City)) contact.City = objEmployee.City;
                contact.FirstName = objEmployee.FirstName;
                contact.LastName = objEmployee.LastName;
                contact.Id = objEmployee.EmployeeId.ToString();
                if (!string.IsNullOrEmpty(objEmployee.MiddleInitial)) contact.MiddleName = objEmployee.MiddleInitial;
                contact.PreferredLanguage = "en-US";
                contact.State = !string.IsNullOrEmpty(objEmployee.State) ? objEmployee.State : "FL";
                if (customFieldsList.Count > 0) contact.CustomFields = customFieldsList.ToArray();
                if (!string.IsNullOrEmpty(objEmployee.Zip)) contact.ZipCode = objEmployee.Zip;
                contact.TimeZoneId = 1000001;
                contact.Country = "United States";
                contact.ContactPointsListRequest = contactPointsList;

                return contact;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Ports private IsEmail(email) from SWNServiceRequests.vb (lines
        /// 597-603). Uses <see cref="MatchEmailPattern"/> verbatim from VB.
        /// </summary>
        private bool IsEmail(string email)
        {
            if (email == null) return false;
            return Regex.IsMatch(email, MatchEmailPattern);
        }

        /// <summary>
        /// Centralised exception logging via <see cref="SwnInterfaceLogController"/>.
        /// VB source relied on a global LogException(ex); we scope it to the
        /// module's interface-log table.
        /// </summary>
        private void LogException(Exception ex)
        {
            try
            {
                _log.LogProcess("SWNServiceRequests", ex == null ? null : ex.ToString(), null);
            }
            catch
            {
                // Swallow logging failures - we never want logging to mask the
                // original exception the caller is already handling.
            }
        }

        #endregion

        #region Data access helpers

        // Thin wrappers that route through the module's controllers.

        private List<EmployeeInfo> GetActiveEmployees()
        {
            return _employees.GetActive().ToList();
        }

        private List<PhoneInfo> GetEmployeePhones(int employeeId)
        {
            return _phones.GetForEmployee(employeeId).ToList();
        }

        private List<int> GetEmployeeGroupIds(int employeeId)
        {
            return _memberships.GetForEmployee(employeeId).Select(m => m.GroupId).ToList();
        }

        private List<int> GetGroupMembershipIds(int groupId)
        {
            return _memberships.GetForGroup(groupId).Select(m => m.EmployeeId).ToList();
        }

        private List<int> GetSwnGroupIds()
        {
            return _groups.GetSwnGroups().Select(g => g.GroupID).ToList();
        }

        private List<GroupInfo> GetSwnGroupsMissingFrom(List<int> existingIds)
        {
            return _groups.GetSwnGroups().Where(g => !existingIds.Contains(g.GroupID)).ToList();
        }

        private GroupInfo GetGroup(int groupId)
        {
            return _groups.GetById(groupId);
        }

        private string GetDepartmentName(int? departmentId)
        {
            if (!departmentId.HasValue) return null;
            var grp = _groups.GetById(departmentId.Value);
            return grp == null ? null : grp.GroupName;
        }

        private string GetCountyName(int? countyId)
        {
            if (!countyId.HasValue) return null;
            var county = _counties.GetById(countyId.Value);
            return county == null ? null : county.CountyName;
        }

        #endregion
    }
}
