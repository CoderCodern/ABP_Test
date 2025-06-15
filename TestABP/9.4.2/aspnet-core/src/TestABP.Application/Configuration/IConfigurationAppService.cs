using System.Threading.Tasks;
using TestABP.Configuration.Dto;

namespace TestABP.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
