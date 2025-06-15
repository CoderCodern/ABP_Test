using System.Threading.Tasks;
using Abp.Application.Services;
using TestABP.Sessions.Dto;

namespace TestABP.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
