using Abp.Authorization;
using CargoX.Fridge.Authorization.Roles;
using CargoX.Fridge.Authorization.Users;

namespace CargoX.Fridge.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
