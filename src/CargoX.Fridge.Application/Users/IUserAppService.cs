using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CargoX.Fridge.Roles.Dto;
using CargoX.Fridge.Users.Dto;

namespace CargoX.Fridge.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
