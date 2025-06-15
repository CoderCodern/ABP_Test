using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace TestABP.Domain.CloudFolders
{
    public class CloudFolder : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int Level { get; set; }
        public bool IsLeaf { get; set; }
        public long? ParentId { get; set; }
        public string Code { get; set; }
        public string CombineName { get; set; }

    }
}
