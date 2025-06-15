using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestABP.Domain.CloudFolders;
using TestABP.Domain.CloudFolders.Dto;

namespace TestABP.APIs.CloudFolders
{
    public class CloudFolderAppService : TestABPAppServiceBase, ICloudFolderAppService
    {
        private readonly CloudFolderManager _cloudFolderManager;
        public CloudFolderAppService(CloudFolderManager cloudFolderManager)
        {
            _cloudFolderManager = cloudFolderManager;
        }

        [HttpPost]
        public Task<TreeFolderDto> GetAll(FolderFilterDto input)
        {
            return _cloudFolderManager.GetAll(input);
        }

        [HttpGet]
        public Task<CloudFolderDto> GetDetail(long folderId)
        {
            return _cloudFolderManager.GetDetail(folderId);
        }

        [HttpPost]
        public Task<CreateCloudFolderDto> Create(CreateCloudFolderDto input)
        {
            return _cloudFolderManager.Create(input);
        }

        [HttpPost]
        public Task<UpdateCloudFolderDto> Update(UpdateCloudFolderDto input)
        {
            return _cloudFolderManager.Update(input);
        }
    }
}
