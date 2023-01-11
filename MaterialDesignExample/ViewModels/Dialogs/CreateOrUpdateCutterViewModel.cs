using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SealWatch.Wpf.ViewModels.Dialogs;

public class CreateOrUpdateCutterViewModel : BaseViewModel
{
    private readonly IProjectAccessLayer _projectAccessLayer;
    private readonly ICutterAccessLayer _cutterAccessLayer;
    private readonly IUserInputService _userInputService;

    public event EventHandler? CloseWindow;

    public CreateOrUpdateCutterViewModel(IProjectAccessLayer projectAccessLayer, ICutterAccessLayer cutterAccessLayer, IUserInputService userInputService)
    {
        _projectAccessLayer = projectAccessLayer;
        _cutterAccessLayer = cutterAccessLayer;
        _userInputService = userInputService;
    }

    public void Loaded()
    {
        Cutter.ProjectId = ProjectId;

        if (Id is not 0)
        {
            Cutter = _cutterAccessLayer.GetEditData(Id);
        }
    }

    public ICommand SubmitCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => Cutter is not null,
        CommandAction = () =>
        {
            var validationErrors = _userInputService.GetInvalidCutterInputs(Cutter);
            if (validationErrors.Count > 0)
            {
                var error = validationErrors.First();
                MessageBox.Show(error.ErrorDescription, error.PropertyName);
                return;
            }

            _cutterAccessLayer.CreateOrUpdate(Cutter);
            CloseWindow!.Invoke(Cutter.ProjectId, EventArgs.Empty);
        }
    };

    public ICommand CloseCommand => new DelegateCommand()
    {
        CommandAction = () => CloseWindow!.Invoke(this, EventArgs.Empty)
    };

    private CutterEditDto _cutter = new();
    public CutterEditDto Cutter
    {
        get => _cutter;
        set
        {
            if (_cutter == value) return;
            _cutter = value;
            OnPropertyChanged();
        }
    }

    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string WindowTitle => Id is 0 ? "Fräse hinzufügen" : "Fräse editieren";
}