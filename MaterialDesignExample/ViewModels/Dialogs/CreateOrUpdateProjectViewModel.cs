using SealWatch.Code.ProjectLayer;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels.Dialogs;

public class CreateOrUpdateProjectViewModel : BaseViewModel
{
    private readonly IProjectAccessLayer _projectAccessLayer;
    private readonly IUserInputService _userInputService;

    public event EventHandler? CloseWindow;

    public CreateOrUpdateProjectViewModel(IProjectAccessLayer projectAccessLayer, IUserInputService userInputService)
    {
        _projectAccessLayer = projectAccessLayer;
        _userInputService = userInputService;
        _project = new();
    }

    public int Id { get; set; }
    public string WindowTitle => Id is 0 ? "Projekt hinzufügen" : "Projekt editieren";

    private ProjectEditDto _project = new();
    public ProjectEditDto Project
    {
        get => _project;
        set
        {
            if (_project == value) return;
            _project = value;
            OnPropertyChanged();
        }
    }

    public void Loaded()
    {
        if (Id is not 0)
            Project = _projectAccessLayer.GetEditData(Id);
    }

    public ICommand SubmitCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => Project is not null,
        CommandAction = () =>
        {
            var validationErrors = _userInputService.GetProjectValidationErrors(Project);
            if (validationErrors.Count > 0)
            {
                var error = validationErrors.First();
                MessageBox.Show(error.ErrorDescription, error.PropertyName);
                return;
            }

            _projectAccessLayer.AddOrEdit(Project);
            CloseWindow!.Invoke(this, EventArgs.Empty);
        }
    };

    public ICommand CloseCommand => new DelegateCommand()
    {
        CommandAction = () => CloseWindow!.Invoke(this, EventArgs.Empty)
    };
}