using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using TestABP.Authorization.Roles;
using TestABP.Authorization.Users;
using TestABP.MultiTenancy;
using TestABP.Domain.CloudFolders;

namespace TestABP.EntityFrameworkCore
{
    public class TestABPDbContext : AbpZeroDbContext<Tenant, Role, User, TestABPDbContext>
    {
        public DbSet<CloudFolder> CloudFolders { get; set; }

        public TestABPDbContext(DbContextOptions<TestABPDbContext> options)
            : base(options)
        {
        }
    }
}
