using Microsoft.EntityFrameworkCore;
using SealWatch.Code.CutterLayer;
using SealWatch.Code.Enums;
using SealWatch.Code.ProjectLayer.Intefaces;
using SealWatch.Data.Database;
using SealWatch.Data.Model;

namespace SealWatch.Code.ProjectLayer;

public class ProjectAccessLayer : IProjectAccessLayer
{
    public List<ProjectListDto> GetList(string? search = null, Visibility visibility = Visibility.All)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var data = context.Set<Project>().Include(item => item.Cutters);
            var query = data.Select(x => x);

            if (visibility is Visibility.Deleted)
                query = data.Where(item => item.IsDeleted);

            else if (visibility is Visibility.Done)
                query = query.Where(item => item.IsDone);

            else
                query = query.Where(item => !item.IsDone && !item.IsDeleted);

            if (search is not null)
                query = query.Where(item => item.Location.ToLower().Contains(search.ToLower()));

            return query.Select(item => new ProjectListDto()
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
    }

    public ProjectDetailDto GetDetails(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var data = context.Set<Data.Model.Project>().Include(item => item.Cutters);

            var item = data.FirstOrDefault(item => item.Id == id);
            if (item == null)
            {
                return null;
            }

            return new ProjectDetailDto()
            {
                Id = item.Id,
                Location = item.Location,
                Blades = item.Blades,
                SlitDepth_m = item.SlitDepth_m,
                StartDate = item.StartDate,
                Cutters = item.Cutters,

                ChangeDate = item.ChangeDate,
                ChangeUser = item.ChangeUser,
                CreateDate = item.CreateDate,
                CreateUser = item.CreateUser,
                DeleteDate = item.DeleteDate,
                DeleteUser = item.DeleteUser,

                IsDeleted = item.IsDeleted,
                IsDone = item.IsDone,
            };
        }
    }

    public ProjectEditDto GetEditData(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Data.Model.Project>()
                              .Include(item => item.Cutters)
                              .ToList()
                              .Where(item => item.Id == id)
                              .FirstOrDefault();

            if (item == null) return null;

            return new ProjectEditDto()
            {
                Id = item.Id,
                Location = item.Location,
                Blades = item.Blades,
                SlitDepth_m = item.SlitDepth_m,
                StartDate = item.StartDate,
                Cutters = item.Cutters
            };
        }
    }

    public List<CutterAnalyseDto> GetAnalyticData()
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var cutterList = new List<CutterAnalyseDto>();
            var projects = context.Set<Project>().Include(item => item.Cutters);

            foreach (var project in projects)
            {
                if (project.Cutters is null) continue;
                foreach (var cutter in project.Cutters)
                {
                    cutterList.Add(new CutterAnalyseDto()
                    {
                        Id = cutter.Id,
                        Serialnumber = cutter.SerialNumber,
                        Location = project.Location,
                        MillingStart = cutter.MillingStart,
                        MillingStop = cutter.MillingStop,
                        SealOrdered = cutter.SealOrdered,
                        WorkDays = cutter.WorkDays,
                        LifeSpan_h = cutter.LifeSpan_h,
                        MillingPerDay_h = cutter.MillingPerDay_h,
                        MillingDuration_y = cutter.MillingDuration_y
                    });
                }
            }
            foreach (var cutter in cutterList)
            {
                var totalDays1Percent = (cutter.MillingStop - cutter.MillingStart).TotalDays / 100;
                var daysPassed = (DateTime.Now - cutter.MillingStart).TotalDays;
                var pace = daysPassed / totalDays1Percent;
                cutter.Durability = (int)pace;
                cutter.DaysLeft = (int)((totalDays1Percent * 100) - daysPassed);
            }
            return cutterList;
        }
    }

    public void CreateOrUpdate(ProjectEditDto projectDto)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var project = context.Set<Project>().Find(projectDto.Id);
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
            var data = context.Set<Project>().ToList();
        }
    }

    public void Remove(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Data.Model.Project>().Find(id);
            if (item != null)
            {
                item.IsDeleted = !item.IsDeleted;
                context.SaveChanges();
            }
        }
    }

    public void Done(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Data.Model.Project>().Find(id);
            if (item is not null)
            {
                item.IsDone = !item.IsDone;
                context.SaveChanges();
            }
        }
    }


    public List<Cutter> GetCutterByProjectId(int id, bool sealOrdered = false, string? search = null)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Data.Model.Project>().Include(item => item.Cutters).Where(item => item.Id == id).FirstOrDefault();

            if (item == null) throw new ArgumentOutOfRangeException("Project not found");

            var list = item.Cutters.Where(item => item.SealOrdered == sealOrdered);

            return list.ToList();
        }
    }

    public bool ProjectExists(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            return context.Set<Data.Model.Project>().Find(id) == null ? false : true;
        }
    }

    public bool CutterExists(string serialNumber)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Data.Model.Cutter>().ToList();
            var existingItem = item.Where(item => item.SerialNumber == serialNumber).FirstOrDefault();
            return existingItem == null;
        }
    }

    public Guid GetGuid()
    {
        return typeof(Project).GUID;
    }
}