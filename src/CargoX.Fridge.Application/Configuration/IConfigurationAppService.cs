using System.Threading.Tasks;
using CargoX.Fridge.Configuration.Dto;

namespace CargoX.Fridge.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
