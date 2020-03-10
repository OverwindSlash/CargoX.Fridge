using System.Threading.Tasks;
using Abp.Application.Services;
using CargoX.Fridge.Authorization.Accounts.Dto;

namespace CargoX.Fridge.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
