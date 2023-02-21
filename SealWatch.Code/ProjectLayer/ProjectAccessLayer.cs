using Microsoft.EntityFrameworkCore;
using SealWatch.Code.Enums;
using SealWatch.Code.Extensions;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Data.Database;
using SealWatch.Data.Model;
using Serilog;

namespace SealWatch.Code.ProjectLayer;

/// <summary>
/// Access Layer between Front-End and DataLayer (Database)
/// </summary>
public class ProjectAccessLayer : IProjectAccessLayer
{
    /// <summary>
    /// Generates a sequence of cutters if cutters available
    /// </summary>
    /// <returns>Sequence of Cutter or Enumerable.Empty<T></returns>

    /// <summary>
    /// Generates a sequence of projects if projects available
    /// Parameter null checking in AccessLayerQueryHelpers
    /// </summary>
    /// <param name="search">Search text for project location</param>
    /// <param name="visibility">Searches depending on all, done & deleted</param>
    /// <returns>Sequence of ProjectListDto</returns>
    public IEnumerable<ProjectListDto> GetList(string? search = null, Visibility visibility = Visibility.All)
    {
        var projects = SealWatchDbContext.NewContext()
            .Set<Project>()
            .Include(item => item.Cutters);

        projects = projects.FilterVisibility(visibility);
        projects = projects.FilterSerialNumber(search);

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

    /// <summary>
    /// Generates a template for editing a project
    /// based on given id
    /// </summary>
    /// <param name="id">Requested Project Id</param>
    /// <returns>Dto for editing</returns>

    //  ToDo: check null for receiving from function
    public ProjectEditDto? GetEditData(int id)
    {
        Project? project = SealWatchDbContext.NewContext()
            .Set<Project>()
            .Include(x => x.Cutters)
            .FirstOrDefault(x => x.Id == id);

        if (project is null)
            return null;

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
    /// <summary>
    /// Generates a template for viewing project details
    /// </summary>
    /// <param name="id">Requested Project Id</param>
    /// <returns></returns>

    //  ToDo: check null for receiving from function
    public ProjectDetailDto? GetDetails(int id)
    {
        Project? project = SealWatchDbContext.NewContext()
            .Set<Project>()
            .Include(item => item.Cutters)
            .FirstOrDefault(x => x.Id == id);

        if (project is null)
            return null;

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
            CreateDate = project.CreateDate,
            CreateUser = project.CreateUser,
            DeleteDate = project.DeleteDate,
            ChangeUser = project.ChangeUser is null ? String.Empty : project.ChangeUser,
            DeleteUser = project.DeleteUser is null ? String.Empty : project.DeleteUser,
        };
    }

    /// <summary>
    /// Searches for existingProject and rewrites changes
    /// If project wasn't found creates new and writes changes
    /// </summary>
    /// <param name="projectDto">Project template to add/edit</param>
    public void AddOrEdit(ProjectEditDto? projectDto)
    {
        if (projectDto is null)
            return;

        var context = SealWatchDbContext.NewContext();
        Project? project = context
            .Set<Project>()
            .Find(projectDto?.Id);

        if (project is null)
        {
            project = new();
            context.Add(project);
        }

        if (projectDto is null)
            return;

        project.Location = projectDto.Location;
        project.Blades = projectDto.Blades;
        project.SlitDepth_m = projectDto.SlitDepth_m;
        project.StartDate = projectDto.StartDate.RemoveTime();

        context.SaveChanges();
    }

    /// <summary>
    /// Removes project
    /// </summary>
    /// <param name="id">Requested project id</param>
    public void Remove(int id)
    {
        var context = SealWatchDbContext.NewContext();

        Project? project = context.Set<Project>().Find(id);
        if (project is null)
        {
            Log.Error($"Tried to remove non-existing project - ID: {id}");
            return;
        }

        project.IsDeleted = !project.IsDeleted;
        context.SaveChanges();
    }

    /// <summary>
    /// Marks project as done
    /// </summary>
    /// <param name="id">Requested project id</param>
    public void Done(int id)
    {
        var context = SealWatchDbContext.NewContext();

        Project? project = context.Set<Project>().Find(id);
        if (project is null)
        {
            Log.Error($"Tried to mark non-existing project as done - ID: {id}");
            return;
        }

        project.IsDone = !project.IsDone;
        context.SaveChanges();
    }

    /// <summary>
    /// Checks if project exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool ProjectExists(int id)
    {
        Project? project = SealWatchDbContext.NewContext()
            .Set<Project>()
            .Find(id);

        return project is not null;
    }

    public Guid GetGuid() => typeof(Project).GUID;
}