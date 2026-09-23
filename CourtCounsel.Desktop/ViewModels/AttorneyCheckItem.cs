using CourtCounsel.Desktop.Models;

namespace CourtCounsel.Desktop.ViewModels;

// Shared by Reports and Data Sheet — both use the same "checkbox list of
// attorneys, active first then inactive" multi-select widget.
public class AttorneyCheckItem : ViewModelBase
{
    public AttorneyInfo Attorney { get; }
    public string DisplayName => Attorney.AttorneyName;
    public bool IsActive => Attorney.IsActive == true;

    private bool _isChecked;
    public bool IsChecked { get => _isChecked; set => SetField(ref _isChecked, value); }

    public AttorneyCheckItem(AttorneyInfo attorney)
    {
        Attorney = attorney;
    }
}
