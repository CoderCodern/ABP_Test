using Abp.AutoMapper;

namespace TestABP.Domain.CloudFolders.Dto
{
    [AutoMapTo(typeof(CloudFolder))]
    public class CreateCloudFolderDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
        public bool? IsSyncToCloud { get; set; }
    }
}
