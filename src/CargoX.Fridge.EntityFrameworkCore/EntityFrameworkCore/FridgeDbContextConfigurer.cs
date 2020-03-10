using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CargoX.Fridge.EntityFrameworkCore
{
    public static class FridgeDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<FridgeDbContext> builder, string connectionString)
        {
            //builder.UseSqlServer(connectionString);
            builder.UseMySql(connectionString);

        }

        public static void Configure(DbContextOptionsBuilder<FridgeDbContext> builder, DbConnection connection)
        {
            //builder.UseSqlServer(connection);
            builder.UseMySql(connection);
        }
    }
}
