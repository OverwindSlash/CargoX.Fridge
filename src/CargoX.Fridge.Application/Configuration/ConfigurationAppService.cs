using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using CargoX.Fridge.Configuration.Dto;

namespace CargoX.Fridge.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : FridgeAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
