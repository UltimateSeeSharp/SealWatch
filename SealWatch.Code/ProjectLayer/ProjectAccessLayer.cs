using Microsoft.EntityFrameworkCore;
using SealWatch.Code.Enums;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Data.Database;
using SealWatch.Data.Model;

namespace SealWatch.Code.ProjectLayer;

public class ProjectAccessLayer : IProjectAccessLayer
{
    public List<ProjectListDto> GetList(string? search = null, Visibility visibility = Visibility.All)
    {
        using var context = SealWatchDbContext.NewContext();

        var projects = context.Set<Project>().Include(item => item.Cutters).ToList();

        if (visibility is Visibility.All)
            projects = projects.Where(x => !x.IsDone && !x.IsDeleted).ToList();

        if (visibility is Visibility.Deleted)
            projects = projects.Where(x => x.IsDeleted).ToList();

        if (visibility is Visibility.Done)
            projects = projects.Where(x => x.IsDone).ToList();

        if (search is not null)
            projects = projects.Where(x => x.Location.ToLower().Contains(search.ToLower())).ToList();

        return projects.Select(item => new ProjectListDto()
        {
            Id = item.Id,
            Location = item.Location,
            StartDate = item.StartDate,
            Blades = item.Blades,
            SlitDepth_m = item.SlitDepth_m,
            Cutters = item.Cutters,
            IsDeleted = item.IsDeleted,
            IsDone = item.IsDone
        }).ToList();
    }

    public ProjectEditDto GetEditData(int id)
    {
        using var context = SealWatchDbContext.NewContext();

        var projects = context.Set<Project>().Include(x => x.Cutters);
        var project = projects.First(x => x.Id == id);
        if (project is null)
            return new();

        return new ProjectEditDto()
        {
            Id = project.Id,
            Location = project.Location,
            Blades = project.Blades,
            SlitDepth_m = project.SlitDepth_m,
            StartDate = project.StartDate,
            Cutters = project.Cutters
        };
    }

    public ProjectDetailDto GetDetails(int id)
    {
        using var context = SealWatchDbContext.NewContext();

        var projects = context.Set<Project>().Include(item => item.Cutters);
        var project = projects.FirstOrDefault(x => x.Id == id);

        if (project is null)
            return new();

        return new ProjectDetailDto()
        {
            Id = project.Id,
            Location = project.Location,
            Blades = project.Blades,
            SlitDepth_m = project.SlitDepth_m,
            StartDate = project.StartDate,
            Cutters = project.Cutters,
            IsDeleted = project.IsDeleted,
            IsDone = project.IsDone,

            ChangeDate = project.ChangeDate,
            ChangeUser = project.ChangeUser,
            CreateDate = project.CreateDate,
            CreateUser = project.CreateUser,
            DeleteDate = project.DeleteDate,
            DeleteUser = project.DeleteUser,
        };
    }

    public void CreateOrUpdate(ProjectEditDto? projectDto)
    {
        if (projectDto is null)
            return;

        projectDto.StartDate = new(projectDto.StartDate.Year, projectDto.StartDate.Month, projectDto.StartDate.Day);

        using var context = SealWatchDbContext.NewContext();

        var project = context.Set<Project>().Find(projectDto?.Id);
        if (project is null)
        {
            project = new();
            context.Add(project);
        }

        project.Location = projectDto.Location;
        project.Blades = projectDto.Blades;
        project.SlitDepth_m = projectDto.SlitDepth_m;
        project.StartDate = projectDto.StartDate;
        context.SaveChanges();
    }

    public void Remove(int id)
    {
        using var context = SealWatchDbContext.NewContext();

        var project = context.Set<Project>().Find(id);
        if (project is null)
            return;

        project.IsDeleted = !project.IsDeleted;
        context.SaveChanges();
    }

    public void Done(int id)
    {
        using var context = SealWatchDbContext.NewContext();

        var project = context.Set<Project>().Find(id);
        if (project is null)
            return;

        project.IsDone = !project.IsDone;
        context.SaveChanges();
    }

    public bool ProjectExists(int id)
    {
        using var context = SealWatchDbContext.NewContext();
        return context.Set<Project>().Find(id) is null ? false : true;
    }

    public Guid GetGuid()
    {
        return typeof(Project).GUID;
    }
}