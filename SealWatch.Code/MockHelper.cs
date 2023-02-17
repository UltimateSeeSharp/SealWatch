using SealWatch.Data.Database;
using SealWatch.Mocking;
using SealWatch.Mocking.Model;

namespace SealWatch.Code;

/// <summary>
/// Generates and saves mock data in database (inMemory)
/// Gets controlled by Wpf.Bootstrapper
/// </summary>
public static class MockHelper
{
    public static void Mock(SealWatchDbContext context)
    {
        var random = new Random();

        List<Data.Model.Project> projects = Mocker.GenerateProjects(8).Select(x => new Data.Model.Project()
        {
            Blades = x.Blades,
            SlitDepth_m = x.SlitDepth_m,
            Location = x.Location,
            StartDate = x.StartDate
        }).ToList();

        context.AddRange(projects);
        context.SaveChanges();

        foreach (Data.Model.Project project in projects)
        {
            List<Data.Model.Cutter> cutters = Mocker.GetCutters(random.Next(2, 10)).Select(x => new Data.Model.Cutter()
            {
                SerialNumber = x.SerialNumber,
                MillingStart = x.MillingStart,
                MillingStop = x.MillingStop,
                WorkDays = x.WorkDays,
                MillingPerDay_h = x.MillingPerDay_h,
                MillingDuration_y = x.MillingDuration_y,
                SealOrdered = x.SealOrdered,
                LifeSpan_h = x.LifeSpan_h,
                SoilType = x.SoilType
            }).ToList();

            cutters.Select(x => x.ProjectId = project.Id);
            project.Cutters = cutters;
        }

        context.SaveChanges();
    }
}