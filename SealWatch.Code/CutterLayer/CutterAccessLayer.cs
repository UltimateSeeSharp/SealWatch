using Microsoft.EntityFrameworkCore;
using SealWatch.Code.CutterLayer.Interfaces;
using SealWatch.Code.Enums;
using SealWatch.Code.Services.Interface;
using SealWatch.Data.Database;
using SealWatch.Data.Model;
using SealWatch.Code.Extensions;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace SealWatch.Code.CutterLayer;

/// <summary>
/// Access Layer between Front-End and DataLayer (Database)
/// </summary>
public class CutterAccessLayer : ICutterAccessLayer
{
    private IAnalyseService _analyseService;
    private int _accuracy;

    public CutterAccessLayer(IAnalyseService analyseService, int accuracy)
    {
        _analyseService = analyseService;
        _accuracy = accuracy;
    }

    /// <summary>
    /// Generates a sequence of cutters if cutters available
    /// </summary>
    /// <returns>Sequence of Cutter or Enumerable.Empty<T></returns>
    public IEnumerable<Cutter> GetList()
    {
        var cutters = SealWatchDbContext.NewContext().Set<Cutter>();
        return cutters is null ? Enumerable.Empty<Cutter>() : cutters;
    }

    /// <summary>
    /// Generates a sequence of all soil types
    /// </summary>
    /// <returns>Sequence of soil types</returns>
    public IEnumerable<string> GetSoilTypes() => new string[] { "Harter Boden", "Normaler Boden", "Weicher Boden" };

    /// <summary>
    /// Generates a template for editing a cutter
    /// based on given id
    /// </summary>
    /// <param name="id">Requested cutter id</param>
    /// <returns>Dto for editing</returns>
    public CutterEditDto GetEditData(int id)
    {
        using var context = SealWatchDbContext.NewContext();
        
        CutterEditDto? cutter = context
            .Set<Cutter>()
            .Where(x => x.Id == id)
            .Select(cutter => new CutterEditDto()
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
            SoilType = cutter.SoilType

        }).FirstOrDefault();

        return cutter is null ? new() : cutter;
    }

    /// <summary>
    /// Analyzes/Gets a sequence of analyzed (millingStop, daysLeft, durability) cutters
    /// </summary>
    /// <param name="search">Searches depending on serial number</param>
    /// <param name="timeframe">Searches cutters which have maintenance in this time frame</param>
    /// <param name="fromProjectId">Selects cutter from specific project</param>
    /// <param name="accuracy">Accuracy for decimal places for analyzed parameters (daysLeft, durability)</param>
    /// <returns>Sequence of analyzed cutters</returns>
    public IEnumerable<AnalysedCutterDto> GetAnalysedCutters(string? search = null, Timeframe? timeframe = null, int? fromProjectId = null, int accuracy = 0)
    {
        var projects = SealWatchDbContext.NewContext().Set<Project>().Include(item => item.Cutters);

        if (!projects.Any())
            return Enumerable.Empty<AnalysedCutterDto>();

        if (fromProjectId is not null)
            projects = projects.Where(x => x.Id == fromProjectId).Include(item => item.Cutters);

        //  Gets cutters from projects
        IEnumerable<AnalysedCutterDto> cutters = Enumerable.Empty<AnalysedCutterDto>();
        foreach (Project project in projects)
        {
            if (project.Cutters is null || project.Cutters.Count is 0)
                continue;

            var analysedCutters = project.Cutters.Select(cutter => new AnalysedCutterDto()
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
                MillingDuration_y = cutter.MillingDuration_y,
                SoilType = cutter.SoilType
            }).ToArray();

            cutters = cutters.Concat(analysedCutters);
        }

        //  Analyze cutters
        foreach (AnalysedCutterDto cutter in cutters)
        {
            cutter.DaysLeft = _analyseService.CalcRelativeTimeInDays(cutter.MillingStop, DateTime.Now, accuracy: _accuracy);
            cutter.Durability = _analyseService.CalcDurability(cutter.MillingStart, cutter.MillingStop, DateTime.Now, accuracy: _accuracy);
        }

        if (search is not null)
            cutters = cutters.Where(x => x.SerialNumber.ToLower().Contains(search.ToLower())).ToArray();

        return timeframe switch
        {
            Timeframe.Year => cutters.Where(c => c.DaysLeft <= (int)timeframe),
            Timeframe.Month => cutters.Where(c => c.DaysLeft <= (int)timeframe),
            Timeframe.Week => cutters.Where(c => c.DaysLeft <= (int)timeframe),
            _ => cutters
        };
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
            MillingStart = cutterDto.MillingStart.RemoveTime(),
            MillingDuration_y = cutterDto.MillingDuration_y,
            MillingPerDay_h = cutterDto.MillingPerDay_h,
            LifeSpan_h = cutterDto.LifeSpan_h,
            WorkDays = cutterDto.WorkDays,
            SealOrdered = false,
            MillingStop = _analyseService.CalcFailureDate(cutterDto.MillingStart, cutterDto.WorkDays, cutterDto.MillingPerDay_h, cutterDto.LifeSpan_h),
            SoilType = cutterDto.SoilType
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
            Log.Error($"CutterAccessLayer - Remove | Tried to delete non-existing cutter - ID: {id}");
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
            Log.Error($"CutterAccessLayer - Remove | Tried to order non-existing cutter - ID: {id}");
            return;
        }

        cutter.SealOrdered = !cutter.SealOrdered;
        context.SaveChanges();
    }
}