using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace TestABP.Domain.CloudFolders.Dto
{
    [AutoMapTo(typeof(CloudFolder))]
    public class CloudFolderDto : EntityDto<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int Level { get; set; }
        public bool IsLeaf { get; set; }
        public long? ParentId { get; set; }
        public string Code { get; set; }
        public string CombineName { get; set; }
    }
}
