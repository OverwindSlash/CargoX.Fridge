using Abp.MultiTenancy;
using CargoX.Fridge.Authorization.Users;

namespace CargoX.Fridge.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
