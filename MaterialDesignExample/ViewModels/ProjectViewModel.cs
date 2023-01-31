using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.HistoryLayer;
using SealWatch.Code.ProjectLayer;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Data.Model;
using SealWatch.Wpf.Custom;
using SealWatch.Wpf.Enums;
using SealWatch.Wpf.Extensions;
using SealWatch.Wpf.Service.Interfaces;
using SealWatch.Wpf.ViewModels.Dialogs;
using SealWatch.Wpf.Views.Dialogs;
using System;
using System.Collections.Generic;
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
    private List<CutterCard>? _allCutterCards = new();
    private int _lastSelectedProjectId = 0;

    public ProjectViewModel(IProjectAccessLayer projectAccessLayer, ICutterAccessLayer cutterAccessLayer, IHistoryAccessLayer historyAccessLayer, IUserInputService userInputService)
    {
        _projectAccessLayer = projectAccessLayer;
        _cutterAccessLayer = cutterAccessLayer;
        _historyAccessLayer = historyAccessLayer;
        _userInputService = userInputService;

        LoadProjects();
    }

    public ICommand NewProjectCommand => new DelegateCommand()
    {
        ObjectCommandAction = (x) =>
        {
            if (SelectedProject is not null)
                _lastSelectedProjectId = SelectedProject.Id;

            ShowToolDialog(x.ToString()!);
            LoadProjects();
        }
    };

    public ICommand ProjectToolCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => SelectedProject is not null,
        ObjectCommandAction = (x) =>
        {
            _lastSelectedProjectId = SelectedProject!.Id;

            ShowToolDialog(x.ToString()!);
            LoadProjects();
        }
    };

    public ICommand CutterToolCommand => new DelegateCommand()
    {
        CanExecuteFunc = () => SelectedCutter is not null,
        ObjectCommandAction = (x) =>
        {
            _lastSelectedProjectId = SelectedProject!.Id;

            ShowToolDialog(x.ToString()!);
            LoadProjects();
        }
    };

    public ICommand ChangeProjectType => new DelegateCommand()
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

    private Cutter? _selectedCutter;
    public Cutter? SelectedCutter
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

            if (CutterCards.Count <= 0)
                return;

            var filteredCutters = _allCutterCards!.Where(x => x.Cutter.SerialNumber.ToLower().Contains(_cutterSearchText.ToLower())).ToList();
            CutterCards = new(filteredCutters);
            OnPropertyChanged(nameof(CutterCards));
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
        Projects = new(_projectAccessLayer.GetList(_cutterSearchText, _visibility));
        SelectedCutter = null;
        CutterCards.Clear();

        if (_lastSelectedProjectId is not 0)
        {
            SelectedProject = Projects.FirstOrDefault(x => x.Id == _lastSelectedProjectId);
            LoadCutterCards();
        }

        OnPropertyChanged(nameof(Projects));
        OnPropertyChanged(nameof(CutterCards));
    }

    private void LoadCutterCards()
    {
        CutterCards.Clear();

        if (SelectedProject?.Cutters?.Count > 0)
        {
            SelectedProject!.Cutters!.ForEach(cutter => CutterCards.Add(new CutterCard() { Cutter = cutter }));
            _allCutterCards = CutterCards.ToList();
        }

        NoCuttersAvailable = CutterCards.Count > 0 ? false : true;
        OnPropertyChanged(nameof(CutterCards));
    }

    private void ShowToolDialog(string tool)
    {
        if (tool == "ProjectDelete" && UserConfirmed("Löschen"))
        {
            _projectAccessLayer.Remove(SelectedProject!.Id);
        }
        else if (tool == "ProjectDone" && UserConfirmed("Erledigt"))
        {
            _projectAccessLayer.Done(SelectedProject!.Id);
        }
        else if (tool == "CutterDelete" && UserConfirmed("Löschen"))
        {
            _cutterAccessLayer.Remove(SelectedCutter!.Id);
        }
        else if (tool.Contains("Dialog"))
        {
            var dialog = GetDialog(tool);
            if (dialog is null) return;
            dialog.ShowDialog();
        }

        Window? GetDialog(string tool)
        {
            return tool switch
            {
                "ProjectEditDialog" => new CreateOrUpdateView(new CreateOrUpdateViewModel(_projectAccessLayer, _userInputService)
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
                "ProjectAddDialog" => new CreateOrUpdateView(new CreateOrUpdateViewModel(_projectAccessLayer, _userInputService)
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

        bool UserConfirmed(string message)
        {
            return _userInputService.UserConfirmPopUp(message);
        }
    }

    internal void UpdateSelectedCutter(Cutter cutter) => SelectedCutter = cutter;
}