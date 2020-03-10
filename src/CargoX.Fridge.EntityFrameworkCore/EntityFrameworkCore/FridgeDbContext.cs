using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using CargoX.Fridge.Authorization.Roles;
using CargoX.Fridge.Authorization.Users;
using CargoX.Fridge.MultiTenancy;

namespace CargoX.Fridge.EntityFrameworkCore
{
    public class FridgeDbContext : AbpZeroDbContext<Tenant, Role, User, FridgeDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public FridgeDbContext(DbContextOptions<FridgeDbContext> options)
            : base(options)
        {
        }
    }
}
