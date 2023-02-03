using Microsoft.EntityFrameworkCore;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.Services;
using SealWatch.Data.Database;
using SealWatch.Data.Model;
using Serilog;

namespace SealWatch.Code.CutterLayer;

public class CutterAccessLayer : ICutterAccessLayer
{
    private AnalyseService _analyseService = new();
    private int _accuracy;

    public CutterAccessLayer(int accuracy)
    {
        _accuracy = accuracy;
    }

    public List<Cutter> GetList()
    {
        return SealWatchDbContext.NewContext().Set<Cutter>().ToList();
    }

    public CutterEditDto GetEditData(int id)
    {
        using var context = SealWatchDbContext.NewContext();

        var cutter = context.Set<Cutter>().Find(id);
        if (cutter is null)
            return new();

        return new CutterEditDto()
        {
            Id = cutter.Id,
            ProjectId = cutter.ProjectId,
            SerialNumber = cutter.SerialNumber,
            MillingDuration_y = cutter.MillingDuration_y,
            MillingPerDay_h = cutter.MillingPerDay_h,
            MillingStart = cutter.MillingStart,
            MillingStop = cutter.MillingStop,
            SealOrdered = cutter.SealOrdered,
            WorkDays = cutter.WorkDays,
            LifeSpan_h = cutter.LifeSpan_h,
        };
    }

    public List<AnalysedCutterDto> GetAnalysedCutters(string? search = null, int? daysLeftFilter = null, int? fromProjectId = null, int accuracy = 0)
    {
        using var context = SealWatchDbContext.NewContext();

        var projects = context.Set<Project>().Include(item => item.Cutters);
        var cutters = new List<AnalysedCutterDto>();

        if (fromProjectId is not null)
            projects = projects.Where(x => x.Id == fromProjectId).Include(item => item.Cutters);

        foreach (Project project in projects)
        {
            if (project.Cutters is null || project.Cutters.Count is 0)
                continue;

            cutters.AddRange(project.Cutters.Select(cutter => new AnalysedCutterDto()
            {
                Location = project.Location,

                Id = cutter.Id,
                SerialNumber = cutter.SerialNumber,
                MillingStart = cutter.MillingStart,
                MillingStop = _analyseService.CalcFailureDate(cutter.MillingStart, cutter.WorkDays, cutter.MillingPerDay_h, cutter.LifeSpan_h),
                SealOrdered = cutter.SealOrdered,
                WorkDays = cutter.WorkDays,
                LifeSpan_h = cutter.LifeSpan_h,
                MillingPerDay_h = cutter.MillingPerDay_h,
                MillingDuration_y = cutter.MillingDuration_y
            }));
        }

        foreach (AnalysedCutterDto cutter in cutters)
        {
            cutter.DaysLeft = _analyseService.CalcRelativeTimeInDays(cutter.MillingStop, accuracy: _accuracy);
            cutter.Durability = _analyseService.CalcDurability(cutter.MillingStart, cutter.MillingStop, accuracy: _accuracy);
        }

        if (search is not null)
            cutters = cutters.Where(x => x.SerialNumber.ToLower().Contains(search.ToLower())).ToList();

        if (daysLeftFilter is not null)
            cutters = cutters.Where(x => x.DaysLeft <= daysLeftFilter).ToList();

        return cutters;
    }

    public void CreateOrUpdate(CutterEditDto cutterDto)
    {
        if (cutterDto is null)
            return;

        using var context = SealWatchDbContext.NewContext();
        var cutter = context.Set<Cutter>().Find(cutterDto.Id);

        if (cutter is not null)
            context.Set<Cutter>().Remove(cutter);

        var newCutter = new Cutter()
        {
            Id = cutterDto.Id,
            ProjectId = cutterDto.ProjectId,
            SerialNumber = cutterDto.SerialNumber,
            MillingStart = cutterDto.MillingStart,
            MillingDuration_y = cutterDto.MillingDuration_y,
            MillingPerDay_h = cutterDto.MillingPerDay_h,
            LifeSpan_h = cutterDto.LifeSpan_h,
            WorkDays = cutterDto.WorkDays,
            SealOrdered = false,
            MillingStop = _analyseService.CalcFailureDate(cutterDto.MillingStart, cutterDto.WorkDays, cutterDto.MillingPerDay_h, cutterDto.LifeSpan_h)
        };

        context.Set<Cutter>().Add(newCutter);
        context.SaveChanges();
    }

    public void Remove(int id)
    {
        using var context = SealWatchDbContext.NewContext();

        var cutter = context.Set<Cutter>().Find(id);
        if (cutter is null)
        {
            Log.Error($"Tried to delete non-existing cutter - ID: {id}");
            return;
        }

        context.Remove(cutter);
        context.SaveChanges();
    }

    public void Order(int id)
    {
        using var context = SealWatchDbContext.NewContext();

        var cutter = context.Set<Cutter>().Find(id);
        if (cutter is null)
        {
            Log.Error($"Tried to order non-existing cutter - ID: {id}");
            return;
        }

        cutter.SealOrdered = !cutter.SealOrdered;
        context.SaveChanges();
    }
}