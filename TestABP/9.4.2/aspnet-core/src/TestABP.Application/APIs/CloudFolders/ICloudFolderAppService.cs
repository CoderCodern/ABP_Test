using System.Threading.Tasks;
using Abp.Application.Services;
using TestABP.Domain.CloudFolders.Dto;

namespace TestABP.APIs.CloudFolders
{
    public interface ICloudFolderAppService : IApplicationService
    {
        Task<TreeFolderDto> GetAll(FolderFilterDto input);
        Task<CloudFolderDto> GetDetail(long folderId);
        Task<CreateCloudFolderDto> Create(CreateCloudFolderDto input);
    }
}
