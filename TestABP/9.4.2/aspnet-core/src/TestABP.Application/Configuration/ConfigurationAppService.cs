using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using TestABP.Configuration.Dto;

namespace TestABP.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : TestABPAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
