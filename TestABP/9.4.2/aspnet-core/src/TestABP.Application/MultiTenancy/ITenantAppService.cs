using Abp.Application.Services;
using TestABP.MultiTenancy.Dto;

namespace TestABP.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

