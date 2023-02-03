using Bogus;
using SealWatch.Mocking.Model;

namespace SealWatch.Mocking;

public static class Mocker
{
    public static Faker<Project> _fakeProjects = new Faker<Project>()
        .RuleFor(x => x.Location, location => location.Address.City())
        .RuleFor(x => x.Blades, blades => blades.Random.Int(1, 10))
        .RuleFor(x => x.SlitDepth_m, depth => depth.Random.Int(1, 10))
        .RuleFor(x => x.StartDate, date => date.Date.Between(DateTime.Now, DateTime.Now.AddDays(-100)));

    private static Faker<Cutter> _fakeCutters = new Faker<Cutter>()
        .RuleFor(x => x.SerialNumber, number => "181-" + number.Random.Int(100, 999).ToString())
        .RuleFor(x => x.MillingStart, date => date.Date.Between(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-30)))
        .RuleFor(x => x.MillingStop, stopDate => stopDate.Date.Between(DateTime.Now.AddDays(30), DateTime.Now.AddDays(300)))
        .RuleFor(x => x.WorkDays, days => days.Random.Int(1, 7))
        .RuleFor(x => x.MillingPerDay_h, perDay => perDay.Random.Int(1, 23))
        .RuleFor(x => x.MillingDuration_y, years => years.Random.Int(1, 5))
        .RuleFor(x => x.SealOrdered, ordered => ordered.Random.Bool())
        .RuleFor(x => x.LifeSpan_h, lifespan => 600);

    public static List<Project> GenerateProjects(int amount) => _fakeProjects.Generate(amount);

    public static List<Cutter> GetCutters(int amount) => _fakeCutters.Generate(amount);
}