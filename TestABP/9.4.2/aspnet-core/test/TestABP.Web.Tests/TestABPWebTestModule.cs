using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TestABP.EntityFrameworkCore;
using TestABP.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace TestABP.Web.Tests
{
    [DependsOn(
        typeof(TestABPWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class TestABPWebTestModule : AbpModule
    {
        public TestABPWebTestModule(TestABPEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(TestABPWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(TestABPWebMvcModule).Assembly);
        }
    }
}