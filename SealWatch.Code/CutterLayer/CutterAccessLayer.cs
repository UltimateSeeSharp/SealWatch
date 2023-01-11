using Microsoft.EntityFrameworkCore;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.Extensions;
using SealWatch.Data.Database;
using SealWatch.Data.Model;

namespace SealWatch.Code.CutterLayer;

public class CutterAccessLayer : ICutterAccessLayer
{   
    public List<Cutter> GetList()
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            return context.Set<Cutter>().ToList();
        }
    }

    public List<CutterAnalyseDto> GetAnalyticData(string? search = null)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var list = new List<CutterAnalyseDto>();
            var projects = context.Set<Project>().Include(item => item.Cutters);

            foreach (var project in projects)
            {
                if (project.Cutters is null) continue;

                if (search is not null)
                {
                    project.Cutters = project.Cutters.Where(x => x.SerialNumber.ToLower().Contains(search.ToLower())).ToList();
                }

                foreach (var cutter in project.Cutters)
                {
                    list.Add(new CutterAnalyseDto()
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

            foreach (var cutter in list)
            {
                var totalDays1Percent = (cutter.MillingStop - cutter.MillingStart).TotalDays / 100;
                var daysPassed = (DateTime.Now - cutter.MillingStart).TotalDays;
                var pace = daysPassed / totalDays1Percent;
                cutter.Durability = Math.Round(pace);
                if (cutter.Durability > 100)
                {
                    cutter.Durability = cutter.Durability * (-1);
                }
                cutter.DaysLeft = (int)((totalDays1Percent * 100) - daysPassed);
            }
            return list;
        }
    }

    public CutterEditDto? GetEditData(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Cutter>().Find(id);
            return new CutterEditDto()
            {
                Id = item.Id,
                ProjectId = item.ProjectId,
                SerialNumber = item.SerialNumber,
                MillingDuration_y = item.MillingDuration_y,
                MillingPerDay_h = item.MillingPerDay_h,
                MillingStart = item.MillingStart,
                MillingStop = item.MillingStop,
                SealOrdered = item.SealOrdered,
                WorkDays = item.WorkDays,
                LifeSpan_h = item.LifeSpan_h,
            };
        }
    }

    public void CreateOrUpdate(CutterEditDto cutterDto)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            if (cutterDto is null) return;
            if (cutterDto.Id is 0)
            {
                context.Set<Cutter>().Add(new Cutter()
                {
                    Id = cutterDto.Id,
                    ProjectId = cutterDto.ProjectId,
                    SerialNumber = cutterDto.SerialNumber,
                    MillingStart = cutterDto.MillingStart,
                    MillingDuration_y = cutterDto.MillingDuration_y,
                    MillingPerDay_h = cutterDto.MillingPerDay_h,
                    LifeSpan_h = cutterDto.LifeSpan_h,
                    WorkDays = cutterDto.WorkDays,
                    MillingStop = GetFailureDates(cutterDto.WorkDays, cutterDto.MillingPerDay_h, cutterDto.LifeSpan_h, cutterDto.MillingStart).FirstOrDefault(),
                    SealOrdered = false,
                });
            }
            else
            {
                var cutter = context.Set<Cutter>().Find(cutterDto.Id);
                if (cutter is null) return;

                context.Set<Cutter>().Remove(cutter);
                context.Set<Cutter>().Add(new Cutter()
                {
                    Id = cutter.Id,
                    ProjectId = cutter.ProjectId,

                    MillingStop = cutterDto.MillingStop,
                    SealOrdered = cutter.SealOrdered,
                    MillingStart = cutterDto.MillingStart,
                    SerialNumber = cutterDto.SerialNumber,
                    MillingDuration_y = cutterDto.MillingDuration_y,
                    MillingPerDay_h = cutterDto.MillingPerDay_h,
                    LifeSpan_h = cutterDto.LifeSpan_h,
                    WorkDays = cutterDto.WorkDays,
                });
            }
            context.SaveChanges();
            var data = context.Set<Cutter>().ToList();
        }
    }

    public void Remove(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Cutter>().Find(id);
            if (item is not null)
            {
                context.Remove(item);
                context.SaveChanges();
            }
        }
    }

    public void Order(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            var item = context.Set<Data.Model.Cutter>().Find(id);
            if (item is not null)
            {
                item.SealOrdered = !item.SealOrdered;
                context.SaveChanges();
            }
        }
    }

    public Cutter GetCutterById(int id)
    {
        using (var context = SealWatchDbContext.NewContext())
        {
            return context.Set<Cutter>().Find(id)!;
        }
    }




    public DateTime GetFailureDate(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart)
    {
        var currentDate = millingStart;
        var hoursPerWeek = workDays * millingPerDay;
        var weeksLeft = lifeSpan / hoursPerWeek;
        return currentDate.AddDays(weeksLeft * 7);
    }

    public List<DateTime> GetFailureDates(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart)
    {
        var dateList = new List<DateTime>();
        for (int x = 0; x < 20; x++)
        {
            var startDate = millingStart;
            if (dateList.Count > 0)
            {
                startDate = dateList.Last();
            }
            dateList.Add(GetFailureDate(workDays, millingPerDay, lifeSpan, startDate));
        }
        return dateList;
    }
}