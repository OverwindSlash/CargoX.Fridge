using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace CargoX.Fridge.Controllers
{
    public abstract class FridgeControllerBase: AbpController
    {
        protected FridgeControllerBase()
        {
            LocalizationSourceName = FridgeConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
