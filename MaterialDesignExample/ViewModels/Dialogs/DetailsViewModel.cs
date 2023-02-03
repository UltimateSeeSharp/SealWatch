using SealWatch.Code.ProjectLayer;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Wpf.Extensions;
using System;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels.Dialogs;

public class DetailsViewModel : BaseViewModel
{
    private readonly IProjectAccessLayer _projectAccessLayer;
    
    public event EventHandler? CloseWindow;

    public DetailsViewModel(IProjectAccessLayer projectAccessLayer)
    {
        _projectAccessLayer = projectAccessLayer;  
    }

    public int Id { get; set; }

    private ProjectDetailDto _project = new();
    public ProjectDetailDto Project
    {
        get => _project;
        set
        {
            if (_project == value) return;
            _project = value;
            OnPropertyChanged();
        }
    }

    public void Loaded() => Project = _projectAccessLayer.GetDetails(Id);

    public ICommand CloseCommand => new DelegateCommand()
    {
        CommandAction = () => CloseWindow!.Invoke(this, EventArgs.Empty)
    };
} 