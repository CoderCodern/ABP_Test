using System.Threading.Tasks;
using Abp.Application.Services;
using TestABP.Authorization.Accounts.Dto;

namespace TestABP.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
