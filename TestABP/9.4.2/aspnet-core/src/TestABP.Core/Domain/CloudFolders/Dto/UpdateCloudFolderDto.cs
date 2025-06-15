using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace TestABP.Domain.CloudFolders.Dto
{
    [AutoMapTo(typeof(CloudFolder))]
    public class UpdateCloudFolderDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? IsSyncToCloud { get; set; }
    }
}
