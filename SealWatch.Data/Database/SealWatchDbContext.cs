using Bogus;
using Microsoft.EntityFrameworkCore;
using SealWatch.Data.Extensions;
using SealWatch.Data.Model;
using Serilog;

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

    public static SealWatchDbContext NewContext(bool useInMemory = true)
    {
        var dbOptions = new DbContextOptionsBuilder();
        var connectionString = @"Data Source=lwnsvsql02;Initial Catalog=FraesenMgmt;trusted_connection=true;";

        if (useInMemory)
        {
            dbOptions.UseInMemoryDatabase("SealWatch");
        }
        else
        {
            dbOptions.UseSqlServer(connectionString);
        }

        var context = new SealWatchDbContext(dbOptions.Options);

        if (!useInMemory)
        {
            try
            {
                context.Database.CanConnect();
            }
            catch
            {
                Log.Fatal("Could not connect to database");
                Environment.Exit(0);
            }
        }

        bool shouldSeed = true;
        if (shouldSeed)
        {
            context.Seed();
        }

        return context;
    }


    private static bool isSeeded = false;
    public void Seed()
    {
        if (isSeeded)
            return;

        isSeeded = true;

        var random = new Random();

        var fakeProjects = new Faker<Project>()
            .RuleFor(x => x.Location, location => location.Address.City())
            .RuleFor(x => x.Blades, blades => blades.Random.Int(1, 10))
            .RuleFor(x => x.SlitDepth_m, depth => depth.Random.Int(1, 10))
            .RuleFor(x => x.StartDate, date => date.Date.Between(DateTime.Now, DateTime.Now.AddDays(-100)));

        var fakeCutters = new Faker<Cutter>()
            .RuleFor(x => x.SerialNumber, number => "181-" + number.Random.Int(100, 999).ToString())
            .RuleFor(x => x.MillingStart, date => date.Date.Between(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-30)))
            .RuleFor(x => x.MillingStop, stopDate => stopDate.Date.Between(DateTime.Now.AddDays(30), DateTime.Now.AddDays(300)))
            .RuleFor(x => x.WorkDays, days => days.Random.Int(1, 7))
            .RuleFor(x => x.MillingPerDay_h, perDay => perDay.Random.Int(1, 23))
            .RuleFor(x => x.MillingDuration_y, years => years.Random.Int(1, 5))
            .RuleFor(x => x.SealOrdered, ordered => ordered.Random.Bool())
            .RuleFor(x => x.LifeSpan_h, lifespan => 600);

        var projects = fakeProjects.Generate(20);

        AddRange(projects);
        SaveChanges();

        foreach (Project project in projects)
        {
            List<Cutter> cutters = fakeCutters.Generate(random.Next(2, 10));
            cutters.Select(x => x.ProjectId = project.Id);
            project.Cutters = cutters;
            SaveChanges();
        }

        var list = Set<Project>().ToList();
    }
}

