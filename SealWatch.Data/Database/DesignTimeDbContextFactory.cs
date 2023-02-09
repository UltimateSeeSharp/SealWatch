using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace SealWatch.Data.Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SealWatchDbContext>
{
    private static bool IKnowWhatIAmDoing = true;

    public SealWatchDbContext CreateDbContext(string[] args)
    {
        if (!IKnowWhatIAmDoing)
        {
            throw new Exception($"Error in {GetType().FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}(...) - Do you know what you do?");
        }

        var builder = new DbContextOptionsBuilder<SealWatchDbContext>();

        builder.UseSqlServer(@"Data Source=lwnsvsql02;Initial Catalog=FraesenMgmt;trusted_connection=true;");

        //builder.UseSqlServer(@"Data Source=LWNSVSQLDEV01;Initial Catalog=TraineeLOGIN;Persist Security Info=True;User ID=traineeLOGIN;Password=****");

        return new SealWatchDbContext(builder.Options);
    }
}

