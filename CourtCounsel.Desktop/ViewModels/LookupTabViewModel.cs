using System.Collections.ObjectModel;
using System.Windows;

namespace CourtCounsel.Desktop.ViewModels;

public class LookupItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool? IsActive { get; set; }
}

// Generic replacement for the Admin screen's 7 near-identical lookup-CRUD
// tabs (Case Types, Attorneys, Counties, Phases, Requestors, Actions,
// Time Spent) — same add/edit/delete shape for every one of them, just
// wired to a different repository and a different "has IsActive?" flag.
public class LookupTabViewModel : ViewModelBase
{
    private readonly Func<IEnumerable<LookupItem>> _loadAll;
    private readonly Action<LookupItem> _insert;
    private readonly Action<LookupItem> _update;
    private readonly Action<int> _delete;

    public string EntityLabel { get; }
    public bool SupportsActive { get; }

    public ObservableCollection<LookupItem> Items { get; } = new();

    private bool _isEditorOpen;
    public bool IsEditorOpen { get => _isEditorOpen; set => SetField(ref _isEditorOpen, value); }

    private int _editId;
    public int EditId { get => _editId; set => SetField(ref _editId, value); }

    private string _editName = "";
    public string EditName { get => _editName; set => SetField(ref _editName, value); }

    private bool _editIsActive = true;
    public bool EditIsActive { get => _editIsActive; set => SetField(ref _editIsActive, value); }

    public string EditorTitle => EditId > 0 ? $"Edit {EntityLabel}" : $"Add {EntityLabel}";

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelEditCommand { get; }

    public LookupTabViewModel(
        string entityLabel, bool supportsActive,
        Func<IEnumerable<LookupItem>> loadAll,
        Action<LookupItem> insert, Action<LookupItem> update, Action<int> delete)
    {
        EntityLabel = entityLabel;
        SupportsActive = supportsActive;
        _loadAll = loadAll;
        _insert = insert;
        _update = update;
        _delete = delete;

        AddCommand = new RelayCommand(BeginAdd);
        EditCommand = new RelayCommand(BeginEdit);
        DeleteCommand = new RelayCommand(Delete);
        SaveCommand = new RelayCommand(Save);
        CancelEditCommand = new RelayCommand(() => IsEditorOpen = false);

        Reload();
    }

    private void Reload()
    {
        Items.Clear();
        foreach (var i in _loadAll()) Items.Add(i);
    }

    private void BeginAdd()
    {
        EditId = 0;
        EditName = "";
        EditIsActive = true;
        OnPropertyChanged(nameof(EditorTitle));
        IsEditorOpen = true;
    }

    private void BeginEdit(object? parameter)
    {
        if (parameter is not LookupItem item) return;
        EditId = item.Id;
        EditName = item.Name;
        EditIsActive = item.IsActive ?? true;
        OnPropertyChanged(nameof(EditorTitle));
        IsEditorOpen = true;
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(EditName)) return;

        var item = new LookupItem
        {
            Id = EditId,
            Name = EditName.Trim(),
            IsActive = SupportsActive ? EditIsActive : (bool?)null
        };

        if (EditId > 0) _update(item); else _insert(item);

        IsEditorOpen = false;
        Reload();
    }

    private void Delete(object? parameter)
    {
        if (parameter is not LookupItem item) return;
        var result = MessageBox.Show($"Delete '{item.Name}'?", "Confirm Delete",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        _delete(item.Id);
        Reload();
    }
}
