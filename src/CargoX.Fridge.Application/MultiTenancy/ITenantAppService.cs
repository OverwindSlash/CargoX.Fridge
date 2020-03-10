using Abp.Application.Services;
using CargoX.Fridge.MultiTenancy.Dto;

namespace CargoX.Fridge.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

