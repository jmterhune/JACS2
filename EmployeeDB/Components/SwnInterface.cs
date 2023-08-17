using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using tjc.Modules.EmployeeDB.Components.Services;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class SwnInterface
    {
        public static Contact AddUpdateSwnContact(Employee employee, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            bool contactExists = false;
            ContactInAccount contactInAccount = ContactExists(employee.EmployeeId.ToString(), swnServiceIdentifier, swnSubscriptionKey, token);
            if (contactInAccount != null)
                contactExists = contactInAccount.Is_contact_in_account;
            Contact contact = null;
            string[] addressLines = employee.Address.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string line1 = "";
            string line2 = "";
            if (addressLines.Length > 0)
            {
                line1 = addressLines[0];
            }
            if (addressLines.Length > 1)
            {
                for (int i = 1; i < addressLines.Length; i++)
                {
                    line2 += addressLines[i];
                }
            }
            //Addresses <--
            ContactAddressModel contactAddress = new ContactAddressModel { First_address = line1, Second_address = line2, City = employee.City, Country = "United States", State = employee.State, Zip_code = employee.Zip, Address_type = "Primary", Building = "", Facility_location = "", Floor = "", Province = "" };
            List<ContactAddressModel> addressList = new List<ContactAddressModel>
            {
                contactAddress
            };
            //-->
            //Custom Fields <--
            List<CustomFieldModel> listCustomFields = new List<CustomFieldModel>();
            if (!string.IsNullOrEmpty(employee.CountyName))
            {
                CustomFieldModel county = new CustomFieldModel { Custom_field_name = "County", Custom_field_value = employee.CountyName };
                listCustomFields.Add(county);
            }
            if (!string.IsNullOrEmpty(employee.DepartmentName))
            {
                CustomFieldModel department = new CustomFieldModel { Custom_field_name = "Department", Custom_field_value = employee.DepartmentName };
                listCustomFields.Add(department);
            }
            if (employee.OfficeLocation != null)
            {
                CustomFieldModel location = new CustomFieldModel { Custom_field_name = "Location", Custom_field_value = employee.OfficeLocation.Description };
                listCustomFields.Add(location);
            }
            else if (!string.IsNullOrEmpty(employee.LocationName))
            {
                CustomFieldModel location = new CustomFieldModel { Custom_field_name = "Location", Custom_field_value = employee.LocationName };
                listCustomFields.Add(location);
            }

            if (!string.IsNullOrEmpty(employee.JobTitle))
            {
                CustomFieldModel title = new CustomFieldModel { Custom_field_name = "Title", Custom_field_value = employee.JobTitle };
                listCustomFields.Add(title);
            }
            //-->
            //Contact Points <--
            List<ContactPointModel> listContactPoints = new List<ContactPointModel>();
            foreach (Phone phone in employee.Phones)
            {
                if (phone.SwnText)
                    listContactPoints.Add(new ContactPointModel { Address = string.IsNullOrEmpty(phone.PhoneNumber) ? "" : phone.PhoneNumber, Name = string.IsNullOrEmpty(phone.PhoneType) ? "" : phone.PhoneType, Type = "Text", Carrier = "SWN Global SMS", Extension = string.IsNullOrEmpty(phone.Extension) | phone.SwnExcludeExtension ? "" : phone.Extension, Cascade_order = phone.PhoneCascade < 0 ? 0 : phone.PhoneCascade, Country_code = "1" }); ; ;
                if (phone.SwnCall)
                    listContactPoints.Add(new ContactPointModel { Address = string.IsNullOrEmpty(phone.PhoneNumber) ? "" : phone.PhoneNumber, Name = string.IsNullOrEmpty(phone.PhoneType) ? "" : phone.PhoneType, Type = "Phone", Carrier = "SWN Global SMS", Extension = string.IsNullOrEmpty(phone.Extension) | phone.SwnExcludeExtension ? "" : phone.Extension, Cascade_order = 0, Country_code = "1" });
            }
            if (!string.IsNullOrEmpty(employee.Email))
                listContactPoints.Add(new ContactPointModel { Address = employee.Email, Name = "Work", Type = "Email", Extension = "", Carrier = "", Cascade_order = 0, Country_code = "1" });
            if (!string.IsNullOrEmpty(employee.PersonalEmail))
                listContactPoints.Add(new ContactPointModel { Address = employee.PersonalEmail, Name = "Home", Type = "Email", Extension = "", Carrier = "", Cascade_order = 0, Country_code = "1" });
            List<string> accessGroups = new List<string>();
            foreach (Group group in employee.Groups)
            {
                accessGroups.Add(group.GroupId.ToString());
            }

            ContactRequest contactRequest = new ContactRequest
            {
                Employee_id = employee.EmployeeId.ToString(),
                First_name = employee.FirstName,
                Last_name = employee.LastName,
                Middle_name = string.IsNullOrEmpty(employee.MiddleInitial) ? "" : employee.MiddleInitial,
                Division = string.IsNullOrEmpty(employee.DepartmentName) ? "" : employee.DepartmentName,
                Full_name = employee.FullName,
                Id = employee.EmployeeId.ToString(),
                Language = "en-US",
                Time_zone = "US/Eastern",
                Addresses = addressList,
                Company = "12th Judicial Circuit Court of Florida",
                Custom_fields = listCustomFields,
                Contact_points = listContactPoints
            };
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);
            if (!contactExists)
            {
                contact = client.POSTContactsAsync(swnServiceIdentifier, swnSubscriptionKey, authorization, contactRequest).Result;
                if (employee.Groups.Count() > 0)
                {
                    List<string> groups = employee.Groups.Select(x => x.GroupId.ToString()).ToList();
                    AddContactToSwnGroup(groups, employee.EmployeeId.ToString(), swnServiceIdentifier, swnSubscriptionKey, token);
                }
            }
            else
            {
                contact = client.PUTContactsIdAsync(employee.EmployeeId.ToString(), swnServiceIdentifier, swnSubscriptionKey, authorization, contactRequest).Result;
                if (employee.Groups.Count() > 0)
                {
                    List<string> groups = employee.Groups.Select(x => x.GroupId.ToString()).ToList();
                    AddContactToSwnGroup(groups, employee.EmployeeId.ToString(), swnServiceIdentifier, swnSubscriptionKey, token);
                }
            }
            return contact;
        }
        public static void AddContactToSwnGroup(List<string> groupIds, string employeeId, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);
            GroupMemberGroupModel addGroupMemberGroup = new GroupMemberGroupModel { Groups = groupIds };
            client.POSTContactsIdGroupsAsync(employeeId, swnServiceIdentifier, swnSubscriptionKey, authorization, addGroupMemberGroup).Wait();
        }
        public static void RemoveContactFromSwnGroup(string groupId, string employeeId, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);
            client.IdAsync(employeeId, groupId, swnServiceIdentifier, swnSubscriptionKey, authorization).Wait();

        }
        public static void DeleteSwnContactById(string employeeId, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DELETEContactsIdAsync(employeeId, swnServiceIdentifier, swnSubscriptionKey, authorization).Wait();

        }
        public static SwgGroupMemberResponse GetSwnContactIds(string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);

            return client.IdMembersAsync("MASTER", null, swnServiceIdentifier, swnSubscriptionKey, authorization).Result;
        }
        public static List<string> GetContactGroupMemberships(string contactId, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            try
            {
                SwnClient client = new SwnClient(new HttpClient());
                var authorization = new AuthenticationHeaderValue("Bearer", token);
                var groupMemberships = client.GETContactsIdGroupsAsync(contactId, swnServiceIdentifier, swnSubscriptionKey, authorization).Result;
                if (groupMemberships.Count() > 0)
                    return groupMemberships.Where(x => x.Id != "MASTER").Select(g => g.Id).ToList();
                return new List<string>();
            }
            catch (Exception exc)
            {

                throw exc;
            }

        }
        public static Contact GetContactById(string employeeId, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);

            return client.GETContactsIdAsync(employeeId, swnServiceIdentifier, swnSubscriptionKey, authorization).Result;

        }
        public static ContactInAccount ContactExists(string employeeId, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);

            return client.IdIscontactinaccountAsync(employeeId, swnServiceIdentifier, swnSubscriptionKey, authorization).Result;

        }
        public static string GetToken(string swnServiceIdentifier, string swnSubscriptionKey, LoginRequest loginRequest)
        {
            TokenInformation sessionToken = SessionVariables.SwnToken;
            if (sessionToken == null)
            {
                sessionToken = Helper.CreateSwnToken(swnServiceIdentifier, swnSubscriptionKey, loginRequest);
                SessionVariables.SwnToken = sessionToken;
            }
            return sessionToken.Token;

        }
        public static SwgGroupDetails GetSwnGroup(string groupId, string swnServiceIdentifier, string swnSubscriptionKey, string token)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);
            SwgGroupDetails groupDetails = client.GETGroupsIdAsync(groupId, swnServiceIdentifier, swnSubscriptionKey, authorization).Result;
            return groupDetails;
        }
        public static SwgGroupDetails AddSwnGroup(Group group, GroupMemberModel groupModel, string swnServiceIdentifier, string swnSubscriptionKey, string token, bool isNew)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var authorization = new AuthenticationHeaderValue("Bearer", token);

            ContactGroupRequest contactGroupRequest = new ContactGroupRequest
            {
                Id = group.GroupId.ToString(),
                Name = group.GroupName,
                Description = group.GroupName,
                Type = "Static",
                Members = groupModel,
            };
            if (isNew)
            {
                SwgGroupResponseDetails groupDetails = client.POSTGroupsAsync(swnServiceIdentifier, swnSubscriptionKey, authorization, contactGroupRequest).Result;
                return groupDetails.Groups.FirstOrDefault();

            }
            else
            {
                var emptyContent = client.PUTGroupsIdAsync(group.GroupId.ToString(), swnServiceIdentifier, swnSubscriptionKey, authorization, contactGroupRequest).Result;
                return new SwgGroupDetails();
            }
        }
    }
}

