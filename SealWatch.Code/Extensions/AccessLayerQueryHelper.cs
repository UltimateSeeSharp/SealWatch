using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SealWatch.Code.Enums;
using SealWatch.Data.Model;
using System.Security.Cryptography.X509Certificates;

namespace SealWatch.Code.Extensions;

/// <summary>
/// 
/// </summary>
public static class AccessLayerQueryHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="project"></param>
    /// <param name="visibility"></param>
    /// <returns></returns>
    public static IIncludableQueryable<Project, List<Cutter>> FilterVisibility(this IIncludableQueryable<Project, List<Cutter>> project, Visibility visibility)
    {
        return visibility switch
        {
            Visibility.All => project,
            Visibility.Done => project.Where(x => x.IsDone && !x.IsDeleted).Include(x => x.Cutters),
            Visibility.Deleted => project.Where(x => x.IsDeleted).Include(x => x.Cutters),
            _ => project,
        };
    }

    public static IIncludableQueryable<Project, List<Cutter>> FilterSerialNumber(this IIncludableQueryable<Project, List<Cutter>> projects, string? search)
    {
        if (search is null)
            return projects;

        var filteredCutter = projects.Where(x => x.Location.ToLower().Contains(search.ToLower()));
        return filteredCutter.Include(x => x.Cutters);
    }
}