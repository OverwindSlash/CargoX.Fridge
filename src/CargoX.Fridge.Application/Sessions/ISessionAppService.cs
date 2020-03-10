using System.Threading.Tasks;
using Abp.Application.Services;
using CargoX.Fridge.Sessions.Dto;

namespace CargoX.Fridge.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
