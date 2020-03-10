using Abp.Application.Services.Dto;

namespace CargoX.Fridge.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

