using CourtCounsel.Desktop.Data;
using CourtCounsel.Desktop.Models;
using CourtCounsel.Desktop.Services;

namespace CourtCounsel.Desktop.ViewModels;

// Mirrors Admin.ascx.cs — the only screen with a hard authorization gate
// in the whole app (IsAuthorized false => the view shows an access-denied
// message instead of the tabs, matching the web module's redirect-to-Search).
public class AdminViewModel : ViewModelBase
{
    public bool IsAuthorized => AuthorizationService.IsAdmin;

    public LookupTabViewModel CaseTypesTab { get; }
    public LookupTabViewModel AttorneysTab { get; }
    public LookupTabViewModel CountiesTab { get; }
    public LookupTabViewModel PhasesTab { get; }
    public LookupTabViewModel RequestorsTab { get; }
    public LookupTabViewModel ActionsTab { get; }
    public LookupTabViewModel TimeSpentTab { get; }

    public AdminViewModel()
    {
        var caseTypeRepo = new CaseTypeRepository();
        CaseTypesTab = new LookupTabViewModel("Case Type", false,
            () => caseTypeRepo.GetAll().Select(x => new LookupItem { Id = x.CaseTypeId, Name = x.CaseType }),
            item => caseTypeRepo.Insert(new CaseTypeInfo { CaseType = item.Name }),
            item => caseTypeRepo.Update(new CaseTypeInfo { CaseTypeId = item.Id, CaseType = item.Name }),
            caseTypeRepo.Delete);

        var attorneyRepo = new AttorneyRepository();
        AttorneysTab = new LookupTabViewModel("Attorney", true,
            () => attorneyRepo.GetAll().Select(x => new LookupItem { Id = x.AttorneyId, Name = x.AttorneyName, IsActive = x.IsActive }),
            item => attorneyRepo.Insert(new AttorneyInfo { AttorneyName = item.Name, IsActive = item.IsActive }),
            item => attorneyRepo.Update(new AttorneyInfo { AttorneyId = item.Id, AttorneyName = item.Name, IsActive = item.IsActive }),
            attorneyRepo.Delete);

        var countyRepo = new CountyRepository();
        CountiesTab = new LookupTabViewModel("County", false,
            () => countyRepo.GetAll().Select(x => new LookupItem { Id = x.CountyId, Name = x.County }),
            item => countyRepo.Insert(new CountyInfo { County = item.Name }),
            item => countyRepo.Update(new CountyInfo { CountyId = item.Id, County = item.Name }),
            countyRepo.Delete);

        var phaseRepo = new PhaseRepository();
        PhasesTab = new LookupTabViewModel("Phase", false,
            () => phaseRepo.GetAll().Select(x => new LookupItem { Id = x.PhaseId, Name = x.Phase }),
            item => phaseRepo.Insert(new PhaseInfo { Phase = item.Name }),
            item => phaseRepo.Update(new PhaseInfo { PhaseId = item.Id, Phase = item.Name }),
            phaseRepo.Delete);

        var requestorRepo = new RequestorRepository();
        RequestorsTab = new LookupTabViewModel("Requestor", true,
            () => requestorRepo.GetAll().Select(x => new LookupItem { Id = x.RequestorId, Name = x.RequestorName, IsActive = x.IsActive }),
            item => requestorRepo.Insert(new RequestorInfo { RequestorName = item.Name, IsActive = item.IsActive }),
            item => requestorRepo.Update(new RequestorInfo { RequestorId = item.Id, RequestorName = item.Name, IsActive = item.IsActive }),
            requestorRepo.Delete);

        var actionRepo = new ActionTakenRepository();
        ActionsTab = new LookupTabViewModel("Action Taken", false,
            () => actionRepo.GetAll().Select(x => new LookupItem { Id = x.ActionId, Name = x.Action }),
            item => actionRepo.Insert(new ActionTakenInfo { Action = item.Name }),
            item => actionRepo.Update(new ActionTakenInfo { ActionId = item.Id, Action = item.Name }),
            actionRepo.Delete);

        var timeSpentRepo = new TimeSpentRepository();
        TimeSpentTab = new LookupTabViewModel("Time Spent", true,
            () => timeSpentRepo.GetAll().Select(x => new LookupItem { Id = x.TimeSpanId, Name = x.TimeSpan, IsActive = x.IsActive }),
            item => timeSpentRepo.Insert(new TimeSpentInfo { TimeSpan = item.Name, IsActive = item.IsActive ?? true }),
            item => timeSpentRepo.Update(new TimeSpentInfo { TimeSpanId = item.Id, TimeSpan = item.Name, IsActive = item.IsActive ?? true }),
            timeSpentRepo.Delete);
    }
}
