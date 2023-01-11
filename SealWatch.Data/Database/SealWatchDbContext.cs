using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SealWatch.Data.Extensions;
using SealWatch.Data.Model;

namespace SealWatch.Data.Database;

public class SealWatchDbContext : DbContext
{
    public SealWatchDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var type in GetType().Assembly.GetExportedTypes()
                                        .Where(p => typeof(IEntity).IsAssignableFrom(p)))
        {
            if (!type.IsAbstract && !type.IsInterface && type.IsClass)
            {
                modelBuilder.Entity(type);
            }
        }

        var relations = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
        foreach (var relationship in relations)
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    public override int SaveChanges() => Task.Run(async () => await SaveChangesAsync()).Result;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var selectedEntityList = ChangeTracker.Entries<IAuditable>()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)
            .ToList();

        if (!selectedEntityList.Any())
            return await base.SaveChangesAsync(cancellationToken);

        var user = $"{Environment.UserDomainName}\\{Environment.UserName}";

        var changeDate = DateTime.Now;

        var auditList = new List<History>();
        foreach (var entity in selectedEntityList)
        {
            switch (entity.State)
            {
                case EntityState.Deleted:
                    entity.Entity.DeleteDate = changeDate;
                    entity.Entity.DeleteUser = user;
                    entity.Entity.ChangeDate = changeDate;
                    entity.Entity.ChangeUser = user;
                    entity.Entity.IsDeleted = true;
                    entity.State = EntityState.Modified;

                    auditList.Add(new History
                    {
                        ReferenceGuid = entity.Entity.GetType().GUID,
                        ReferenceId = entity.Property("Id").OriginalValue.ToString(),
                        Property = nameof(entity.Entity.IsDeleted),
                        ChangeUser = user,
                        ChangeDate = changeDate,
                        OldValue = false.ToString(),
                        NewValue = true.ToString(),
                    });

                    break;

                case EntityState.Modified:
                    var oldValues = entity.GetDatabaseValues();
                    auditList.AddRange(from property in entity.OriginalValues.Properties
                                       let oldValue = oldValues[property]
                                       let newValue = entity.CurrentValues[property]
                                       where oldValue?.ToString() != newValue?.ToString() && !property.IsForeignKey()
                                       select new History
                                       {
                                           ReferenceGuid = entity.Entity.GetType().GUID,
                                           ReferenceId = entity.Property("Id").OriginalValue.ToString(),
                                           Property = property.Name,
                                           ChangeUser = user,
                                           ChangeDate = changeDate,
                                           OldValue = oldValue?.ToString(),
                                           NewValue = newValue?.ToString(),
                                       });

                    auditList.AddRange(from property in entity.OriginalValues.Properties
                                       let oldValue = oldValues[property]
                                       let newValue = entity.CurrentValues[property]
                                       where oldValue?.ToString() != newValue?.ToString()
                                       where property.IsForeignKey()
                                       let foreignKeys = property.GetContainingForeignKeys()
                                       from foreignKey in foreignKeys
                                       let oldEntity = oldValue == null ? null : Find(foreignKey.PrincipalEntityType.ClrType, oldValue)
                                       let newEntity = newValue == null ? null : Find(foreignKey.PrincipalEntityType.ClrType, newValue)
                                       select new History
                                       {
                                           ReferenceGuid = entity.Entity.GetType().GUID,
                                           ReferenceId = entity.Property("Id").OriginalValue.ToString(),
                                           Property = foreignKey.PrincipalEntityType.ClrType.Name,
                                           ChangeUser = user,
                                           ChangeDate = changeDate,
                                           OldValue = oldEntity?.ToString(),
                                           NewValue = newEntity?.ToString(),
                                       });

                    entity.Entity.ChangeDate = changeDate;
                    entity.Entity.ChangeUser = user;

                    if (!entity.Entity.IsDeleted)
                    {
                        entity.Entity.DeleteDate = null;
                        entity.Entity.DeleteUser = null;
                    }

                    break;

                case EntityState.Added:
                    entity.Entity.CreateDate = changeDate;
                    entity.Entity.CreateUser = user;
                    break;
            }
        }

        if (auditList.Any())
            Set<History>().AddRange(auditList);

        return await base.SaveChangesAsync(cancellationToken);
    }

    public static SealWatchDbContext NewContext(bool useInMemory = false)
    {
        var dbOptions = new DbContextOptionsBuilder();
        //  useInMemory
        var connectionString = @"Data Source=lwnsvsql02;Initial Catalog=FraesenMgmt;trusted_connection=true;";
        dbOptions.UseSqlServer(connectionString);

        var context = new SealWatchDbContext(dbOptions.Options);

        //try
        //{
        //    context.Database.CanConnect();
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"No internet connection! / Permission not granted!. - code: {ex.Message}");
        //    Environment.Exit(0);
        //}

        //  useInMemory
        if (true)
        {
            context.Seed();
        }

        return context;
    }

    private static bool seeded;

    public void Seed()
    {
        //if (!Database.IsInMemory())
        //{
        //    throw new NotSupportedException($"Method {System.Reflection.MethodBase.GetCurrentMethod().Name} is only allowed with InMemory Database!");
        //}

        return;

        if (seeded)
        {
            return;
        }
        seeded = true;

        var project1 = new Project()
        {
            Location = "Petersburg",
            Blades = 2,
            SlitDepth_m = 5,
            Cutters = new List<Cutter>() { },
            StartDate = DateTime.Now.AddDays(-20),
        };

        var project2 = new Project()
        {
            Location = "Amsterdam",
            Blades = 5,
            SlitDepth_m = 3,
            Cutters = new List<Cutter>() { },
            StartDate = DateTime.Now.AddDays(-14),
        };

        var project3 = new Project
        {
            Location = "London",
            Blades = 9,
            SlitDepth_m = 12,
            Cutters = new List<Cutter>() { },
            StartDate = DateTime.Now.AddDays(-53)
        };

        var project5 = new Project
        {
            Location = "Wien",
            Blades = 5,
            SlitDepth_m = 8,
            Cutters = new List<Cutter>() { },
            StartDate = DateTime.Now.AddDays(-53)
        };

        var project4 = new Project
        {
            Location = "Moskau",
            Blades = 9,
            SlitDepth_m = 1,
            Cutters = new List<Cutter>() { },
            StartDate = DateTime.Now.AddDays(-53)
        };

        var project6 = new Project
        {
            Location = "Lissabon",
            Blades = 2,
            SlitDepth_m = 6,
            Cutters = new List<Cutter>() { },
            StartDate = DateTime.Now.AddDays(-53)
        };

        var project7 = new Project
        {
            Location = "Kiev",
            Blades = 4,
            SlitDepth_m = 14,
            Cutters = new List<Cutter>() { },
            StartDate = DateTime.Now.AddDays(-53)
        };

        Add(project1);
        Add(project2);
        Add(project3);
        Add(project4);
        Add(project5);
        Add(project6);
        Add(project7);

        SaveChanges();

        var random = new Random();

        var cutter = new Cutter()
        {
            ProjectId = project1.Id,
            SerialNumber = "181-569",
            MillingStart = DateTime.Now,
            MillingStop = DateTime.Now.AddDays(random.Next(0, 10)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterTwo = new Cutter()
        {
            ProjectId = project2.Id,
            SerialNumber = "181-635",
            MillingStart = new DateTime(2018, 5, 12),
            MillingStop = DateTime.Now.AddDays(random.Next(0, 10)),
            WorkDays = 6,
            MillingPerDay_h = 7,
            MillingDuration_y = 1.5,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterThree = new Cutter()
        {
            ProjectId = project2.Id,
            SerialNumber = "181-784",
            MillingStart = new DateTime(2020, 11, 23),
            MillingStop = DateTime.Now.AddDays(random.Next(0, 10)),
            WorkDays = 4,
            MillingPerDay_h = 9,
            MillingDuration_y = 0.5,
            SealOrdered = false,
            LifeSpan_h = 800
        };

        var cutterFour = new Cutter
        {
            ProjectId = project1.Id,
            SerialNumber = $"181-865",
            MillingStart = new DateTime(2021, 4, 12),
            MillingStop = DateTime.Now.AddDays(-1),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterFive = new Cutter
        {
            ProjectId = project1.Id,
            SerialNumber = $"181-673",
            MillingStart = new DateTime(2021, 11, 2),
            MillingStop = DateTime.Now.AddDays(random.Next(0, 10)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterSix = new Cutter
        {
            ProjectId = project1.Id,
            SerialNumber = $"181-236",
            MillingStart = new DateTime(2021, 6, 18),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterSeven = new Cutter
        {
            ProjectId = project2.Id,
            SerialNumber = $"181-964",
            MillingStart = new DateTime(2021, 8, 17),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEight = new Cutter
        {
            ProjectId = project2.Id,
            SerialNumber = $"181-642",
            MillingStart = new DateTime(2021, 6, 25),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };


        var cutterEightA = new Cutter
        {
            ProjectId = project2.Id,
            SerialNumber = $"181-442",
            MillingStart = DateTime.Now.AddDays(1 - 104).Date,
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEightB = new Cutter
        {
            ProjectId = project1.Id,
            SerialNumber = $"181-652",
            MillingStart = DateTime.Now.AddDays(4 - 104),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEightC = new Cutter
        {
            ProjectId = project4.Id,
            SerialNumber = $"181-662",
            MillingStart = DateTime.Now.AddDays(4 - 104),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEightD = new Cutter
        {
            ProjectId = project5.Id,
            SerialNumber = $"181-672",
            MillingStart = DateTime.Now.AddDays(6 - 104),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEightE = new Cutter
        {
            ProjectId = project6.Id,
            SerialNumber = $"181-682",
            MillingStart = DateTime.Now.AddDays(5 - 104),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEightF = new Cutter
        {
            ProjectId = project3.Id,
            SerialNumber = $"181-692",
            MillingStart = DateTime.Now.AddDays(4 - 104),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEightG = new Cutter
        {
            ProjectId = project7.Id,
            SerialNumber = $"181-640",
            MillingStart = DateTime.Now.AddDays(3 - 104),
            MillingStop = DateTime.Now.AddDays(random.Next(50, 180)),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        var cutterEightH = new Cutter
        {
            ProjectId = project3.Id,
            SerialNumber = $"181-641",
            MillingStart = DateTime.Now,
            MillingStop = DateTime.Now.AddHours(14),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        ; var cutterEightI = new Cutter
        {
            ProjectId = project3.Id,
            SerialNumber = $"181-642",
            MillingStart = DateTime.Now,
            MillingStop = DateTime.Now.AddHours(6),
            WorkDays = 5,
            MillingPerDay_h = 8,
            MillingDuration_y = 1,
            SealOrdered = false,
            LifeSpan_h = 600
        };

        Add(cutter);
        Add(cutterTwo);
        Add(cutterThree);
        Add(cutterFour);
        Add(cutterFive);
        Add(cutterSix);
        Add(cutterSeven);
        Add(cutterEight);
        Add(cutterEightA);
        Add(cutterEightB);
        Add(cutterEightC);
        Add(cutterEightD);
        Add(cutterEightE);
        Add(cutterEightF);
        Add(cutterEightG);
        Add(cutterEightH);
        Add(cutterEightI);
        SaveChanges();

        using var context = NewContext();
        var cutters = context.Set<Cutter>().ToList();
        foreach (var selectedCutter in cutters)
        {
            selectedCutter.MillingStop = GetFailureDate(selectedCutter.WorkDays, selectedCutter.MillingPerDay_h, selectedCutter.LifeSpan_h, selectedCutter.MillingStart);
        }

        SaveChanges();

        var data = context.Set<Cutter>().ToList();
    }

    public DateTime GetFailureDate(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart)
    {
        var currentDate = millingStart;
        var hoursPerWeek = workDays * millingPerDay;
        var weeksLeft = lifeSpan / hoursPerWeek;
        return currentDate.AddDays(weeksLeft * 7);
    }
}

