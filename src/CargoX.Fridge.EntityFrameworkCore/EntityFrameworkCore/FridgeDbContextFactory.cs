using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using CargoX.Fridge.Configuration;
using CargoX.Fridge.Web;

namespace CargoX.Fridge.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class FridgeDbContextFactory : IDesignTimeDbContextFactory<FridgeDbContext>
    {
        public FridgeDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FridgeDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            FridgeDbContextConfigurer.Configure(builder, configuration.GetConnectionString(FridgeConsts.ConnectionStringName));

            return new FridgeDbContext(builder.Options);
        }
    }
}
