using SealWatch.Code.CutterLayer;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.HistoryLayer.Interfaces;
using SealWatch.Code.ProjectLayer;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Wpf.Custom;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using SealWatch.Wpf.ViewModels.Dialogs;
using SealWatch.Wpf.Views.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Visibility = SealWatch.Code.Enums.Visibility;

namespace SealWatch.Wpf.ViewModels;

public class ProjectViewModel : BaseViewModel
{
    private readonly IProjectAccessLayer _projectAccessLayer;
    private readonly ICutterAccessLayer _cutterAccessLayer;
    private readonly IHistoryAccessLayer _historyAccessLayer;
    private readonly IUserInputService _userInputService;

    private Visibility _visibility = Visibility.All;
    private int _lastSelectedProjectId = 0;

    public ProjectViewModel(
        IProjectAccessLayer projectAccessLayer,
        ICutterAccessLayer cutterAccessLayer,
        IHistoryAccessLayer historyAccessLayer,
        IUserInputService userInputService)
    {
        _projectAccessLayer = projectAccessLayer;
        _cutterAccessLayer = cutterAccessLayer;
        _historyAccessLayer = historyAccessLayer;
        _userInputService = userInputService;
    }

    public void Loaded() => LoadProjects();

    public ICommand NewProjectCommand => new DelegateCommand()
    {
        ObjectCommandAction = (x) =>
        {
            if (SelectedProject is not null)
                _lastSelectedProjectId = SelectedProject.Id;

            SubmitToolDialog(x.ToString()!);

            LoadProjects();
        }
    };

    public ICommand ProjectToolCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => SelectedProject is not null,
        ObjectCommandAction = (x) =>
        {
            _lastSelectedProjectId = SelectedProject!.Id;

            SubmitToolDialog(x.ToString()!);
            LoadProjects();
        }
    };

    public ICommand CutterToolCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => SelectedCutter is not null,
        ObjectCommandAction = (x) =>
        {
            _lastSelectedProjectId = SelectedProject!.Id;

            SubmitToolDialog(x.ToString()!);
            LoadProjects();
        }
    };

    public ICommand ChangeProjectVisability => new DelegateCommand()
    {
        ObjectCommandAction = (x) =>
        {
            object? visibility = null;
            Enum.TryParse(typeof(Visibility), x.ToString(), out visibility);

            if (visibility is not null)
                _visibility = (Visibility)visibility;

            LoadProjects();
        }
    };

    private ProjectListDto? _selectedProject;
    public ProjectListDto? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            OnPropertyChanged();

            SelectedCutter = null;
            LoadCutterCards();
        }
    }

    private AnalysedCutterDto? _selectedCutter;
    public AnalysedCutterDto? SelectedCutter
    {
        get => _selectedCutter;
        set
        {
            _selectedCutter = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<CutterCard> CutterCards { get; set; } = new();
    public ObservableCollection<ProjectListDto> Projects { get; set; } = new();

    private string _cutterSearchText = String.Empty;
    public string CutterSearchText
    {
        get => _cutterSearchText;
        set
        {
            _cutterSearchText = value;
            OnPropertyChanged();

            if (CutterCards.Count > 0)
                LoadCutterCards(CutterSearchText);
        }
    }

    private bool _noCuttersAvailable = true;
    public bool NoCuttersAvailable
    {
        get => _noCuttersAvailable;
        set
        {
            _noCuttersAvailable = value;
            OnPropertyChanged();
        }
    }

    private void LoadProjects()
    {
        CutterCards.Clear();
        SelectedCutter = null;

        Projects = new(_projectAccessLayer.GetList(_cutterSearchText, _visibility));

        if (_lastSelectedProjectId is not 0)
            SelectedProject = Projects.FirstOrDefault(x => x.Id == _lastSelectedProjectId);

        OnPropertyChanged(nameof(Projects));
    }

    private void LoadCutterCards(string? search = null)
    {
        if (SelectedProject is null || SelectedProject.Cutters is null || SelectedProject.Cutters.Count is 0)
            return;

        CutterCards.Clear();

        var cutters = _cutterAccessLayer.GetAnalysedCutters(projectId: SelectedProject.Id, search: search);
        cutters.ForEach(cutter => CutterCards.Add(new CutterCard() { Cutter = cutter }));
        OnPropertyChanged(nameof(CutterCards));

        NoCuttersAvailable = CutterCards.Count <= 0 ? true : false;
    }

    private void SubmitToolDialog(string tool)
    {
        if (tool == "ProjectDelete" && _userInputService.UserConfirmPopUp("Löschen"))
        {
            _projectAccessLayer.Remove(SelectedProject!.Id);
        }
        else if (tool == "ProjectDone" && _userInputService.UserConfirmPopUp("Erledigt"))
        {
            _projectAccessLayer.Done(SelectedProject!.Id);
        }
        else if (tool == "CutterDelete" && _userInputService.UserConfirmPopUp("Löschen"))
        {
            _cutterAccessLayer.Remove(SelectedCutter!.Id);
        }
        else if (tool.Contains("Dialog"))
        {
            var dialog = GetDialog(tool);

            if (dialog is not null)
                dialog.ShowDialog();
        }

        Window? GetDialog(string tool) => tool switch
        {
            "ProjectEditDialog" => new CreateOrUpdateView(new CreateOrUpdateProjectViewModel(_projectAccessLayer, _userInputService)
            {
                Id = SelectedProject!.Id
            }),
            "ProjectHistoryDialog" => new HistoryView(new HistoryViewModel(_projectAccessLayer, _historyAccessLayer)
            {
                RefId = SelectedProject!.Id.ToString(),
                Guid = _projectAccessLayer.GetGuid()
            }),
            "ProjectDetailsDialog" => new DetailsView(new DetailsViewModel(_projectAccessLayer)
            {
                Id = SelectedProject!.Id
            }),
            "ProjectAddDialog" => new CreateOrUpdateView(new CreateOrUpdateProjectViewModel(_projectAccessLayer, _userInputService)
            {
                Id = 0
            }),
            "CutterEditDialog" => new CreateOrUpdateCutterView(new CreateOrUpdateCutterViewModel(_projectAccessLayer, _cutterAccessLayer, _userInputService)
            {
                Id = SelectedCutter!.Id
            }),
            "CutterAddDialog" => new CreateOrUpdateCutterView(new CreateOrUpdateCutterViewModel(_projectAccessLayer, _cutterAccessLayer, _userInputService)
            {
                Id = 0,
                ProjectId = SelectedProject!.Id
            }),
            _ => null
        };
    }

    internal void UpdateSelectedCutter(AnalysedCutterDto cutter) => SelectedCutter = cutter;
}