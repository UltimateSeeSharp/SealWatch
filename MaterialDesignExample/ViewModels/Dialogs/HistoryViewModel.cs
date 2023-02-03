using SealWatch.Code.HistoryLayer.Interfaces;
using SealWatch.Code.HistotyLayer;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Wpf.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels.Dialogs;

public class HistoryViewModel : BaseViewModel
{
    private readonly IHistoryAccessLayer _historyAccessLayer;
    private readonly IProjectAccessLayer _projectAccessLayer;
    private string? _refId;
    private Guid? _guid;

    public event EventHandler? CloseWindow;

    public HistoryViewModel(IProjectAccessLayer projectAccessLayer, IHistoryAccessLayer historyAccessLayer)
    {
        _historyAccessLayer = historyAccessLayer;
        _projectAccessLayer = projectAccessLayer;
    }

    private ObservableCollection<HistoryListDto> _history = new();
    public ObservableCollection<HistoryListDto> History
    {
        get => _history;
        set
        {
            if (_history == value) return;
            _history = value;
            OnPropertyChanged();
        }
    }

    public Guid? Guid
    {
        get => _guid.Value;
        set
        {
            _guid = value;
            Load();
        }
    }

    public string? RefId
    {
        get => _refId;
        set
        {
            _refId = value;
            Load();
        }
    }

    public void Loaded() => History = new(_historyAccessLayer.GetList(Guid!.Value, RefId!));

    private void Load()
    {
        if (_refId is null && _guid is null)
            return;

        History = new ObservableCollection<HistoryListDto>(_historyAccessLayer.GetList(_guid.Value, _refId));
    }

    public ICommand CloseCommand => new DelegateCommand()
    {
        CommandAction = () => CloseWindow!.Invoke(this, EventArgs.Empty)
    };
}