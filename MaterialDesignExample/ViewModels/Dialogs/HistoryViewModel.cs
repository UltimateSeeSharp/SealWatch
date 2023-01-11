using SealWatch.Code.HistoryLayer;
using SealWatch.Code.HistotyLayer;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels.Dialogs;

public class HistoryViewModel : BaseViewModel
{
    private readonly IHistoryAccessLayer _historyAccessLayer;
    private readonly IProjectAccessLayer _projectAccessLayer;

    public event EventHandler? CloseWindow;
    private Guid? _guid;
    private string? _refId;

    public HistoryViewModel(IProjectAccessLayer projectAccessLayer, IHistoryAccessLayer historyAccessLayer)
    {
        _historyAccessLayer = historyAccessLayer;
        _projectAccessLayer = projectAccessLayer;
    }

    public void Loaded()
    {

        History = new(_historyAccessLayer.GetList(Guid!.Value, RefId!));
    }

    public ICommand CloseCommand => new DelegateCommand()
    {
        CommandAction = () => CloseWindow!.Invoke(this, EventArgs.Empty)
    };


    private void Load()
    {
        if (_refId is not null && _guid is not null)
        {
            History = new ObservableCollection<HistoryListDto>(_historyAccessLayer.GetList(_guid.Value, _refId));
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

    private ObservableCollection<HistoryListDto> _history;
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
}