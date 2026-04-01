using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace tjc.Modules.jacs.Services.ViewModels
{
    // ---------------------------------------------------------------------------
    // Standard envelope used for ALL clerk API responses surfaced to the browser.
    // HTTP status carries success/failure; "error" is null on success, "data" is
    // null on failure.  This mirrors the pattern used throughout the internal API
    // (EventSearchResult, CountyResult, etc.).
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Generic response wrapper returned to the browser for every clerk API call.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ClerkApiResult<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        public static ClerkApiResult<T> Success(T data) =>
            new ClerkApiResult<T> { Data = data, Error = null };

        public static ClerkApiResult<T> Failure(string error) =>
            new ClerkApiResult<T> { Data = default, Error = error };
    }

    // ---------------------------------------------------------------------------
    // Clerk API response payloads — deserialized from the clerk's raw JSON so we
    // control what fields reach the browser and can rename/normalise as needed.
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Payload returned by the clerk's AddEvent endpoint on HTTP 201.
    /// Raw clerk JSON: { "EventId": 123456, "error": "" }
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ClerkAddEventResponse
    {
        /// <summary>The clerk-assigned event ID for the newly created hearing.</summary>
        [JsonProperty("clerk_event_id")]
        public long ClerkEventId { get; set; }
    }

    /// <summary>
    /// Payload returned to the browser after a successful UpdateEvent call.
    /// The clerk returns HTTP 200 with { "error": "..." } only on failure; on
    /// success the body is empty / ignored.  We return a simple acknowledgement.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ClerkWriteAckResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// One case record returned by the clerk's GetCase endpoint.
    /// Raw clerk JSON array element:
    /// {
    ///   "CaseId": 12345,
    ///   "CaseNumber": "58-2025-SC-006484-XXXA-SC",
    ///   "Notes": "case Notes",
    ///   "Petitioner": "Smith, John",
    ///   "PetitionerAttyBar": "0001587",
    ///   "PetitionerEmail": "attorney@lawfirm.com",
    ///   "Respondent": "Smith, Jane",
    ///   "RespondentAttyBar": "0154871",
    ///   "RespondentEmail": "attorney@lawfirm.com",
    ///   "Telephone": "555-1234",
    ///   "Status": 1
    /// }
    /// Property names are normalised to snake_case for consistency with the rest
    /// of the API.  The JsonProperty attributes map from the clerk's PascalCase.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ClerkCaseResult
    {
        [JsonProperty("clerk_case_id")]
        public long ClerkCaseId { get; set; }

        [JsonProperty("case_number")]
        public string CaseNumber { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>Maps to Plaintiff / Petitioner / Ward depending on court type.</summary>
        [JsonProperty("petitioner")]
        public string Petitioner { get; set; }

        [JsonProperty("petitioner_atty_bar")]
        public string PetitionerAttyBar { get; set; }

        [JsonProperty("petitioner_email")]
        public string PetitionerEmail { get; set; }

        /// <summary>Maps to Defendant / Respondent / Patient depending on court type.</summary>
        [JsonProperty("respondent")]
        public string Respondent { get; set; }

        [JsonProperty("respondent_atty_bar")]
        public string RespondentAttyBar { get; set; }

        [JsonProperty("respondent_email")]
        public string RespondentEmail { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    /// <summary>
    /// Raw deserialization target for the clerk's GetCase response before mapping.
    /// The clerk sends PascalCase; Newtonsoft handles the mapping via attributes.
    /// </summary>
    internal class ClerkCaseRaw
    {
        [JsonProperty("CaseId")]
        public long CaseId { get; set; }

        [JsonProperty("CaseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("Notes")]
        public string Notes { get; set; }

        [JsonProperty("Petitioner")]
        public string Petitioner { get; set; }

        [JsonProperty("PetitionerAttyBar")]
        public string PetitionerAttyBar { get; set; }

        [JsonProperty("PetitionerEmail")]
        public string PetitionerEmail { get; set; }

        [JsonProperty("Respondent")]
        public string Respondent { get; set; }

        [JsonProperty("RespondentAttyBar")]
        public string RespondentAttyBar { get; set; }

        [JsonProperty("RespondentEmail")]
        public string RespondentEmail { get; set; }

        [JsonProperty("Telephone")]
        public string Telephone { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        public ClerkCaseResult ToViewModel() => new ClerkCaseResult
        {
            ClerkCaseId = CaseId,
            CaseNumber = CaseNumber,
            Notes = Notes,
            Petitioner = Petitioner,
            PetitionerAttyBar = PetitionerAttyBar,
            PetitionerEmail = PetitionerEmail,
            Respondent = Respondent,
            RespondentAttyBar = RespondentAttyBar,
            RespondentEmail = RespondentEmail,
            Telephone = Telephone,
            Status = Status
        };
    }

    /// <summary>
    /// Raw deserialization target for the clerk's GetEvent response.
    /// {
    ///   "CaseId": 12345,
    ///   "JudgeId": 154,
    ///   "EventTypeId": 1535,
    ///   "OtherEventType": "User Value",
    ///   "EventDateTime": "2026-04-15 10:30:00",
    ///   "Duration": 15,
    ///   "CourtRoomId": 25,
    ///   "Notes": "REAL PROPERTY - DOCS COMPLETE"
    /// }
    /// </summary>
    internal class ClerkGetEventRaw
    {
        [JsonProperty("CaseId")]
        public long CaseId { get; set; }

        [JsonProperty("JudgeId")]
        public long JudgeId { get; set; }

        [JsonProperty("EventTypeId")]
        public long EventTypeId { get; set; }

        [JsonProperty("OtherEventType")]
        public string OtherEventType { get; set; }

        [JsonProperty("EventDateTime")]
        public DateTime? EventDateTime { get; set; }

        [JsonProperty("Duration")]
        public int Duration { get; set; }

        [JsonProperty("CourtRoomId")]
        public long CourtRoomId { get; set; }

        [JsonProperty("Notes")]
        public string Notes { get; set; }
    }

    /// <summary>
    /// Raw deserialization target for the clerk's AddEvent response.
    /// { "EventId": 123456, "error": "" }
    /// </summary>
    internal class ClerkAddEventRaw
    {
        [JsonProperty("EventId")]
        public long EventId { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    /// <summary>
    /// Raw deserialization target for clerk write endpoints that return only an
    /// error field on failure (UpdateEvent, CancelEvent, RescheduleEvent).
    /// { "error": "Descriptive error message" }
    /// </summary>
    internal class ClerkWriteErrorRaw
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        public bool IsSuccess => string.IsNullOrWhiteSpace(Error);
    }
}