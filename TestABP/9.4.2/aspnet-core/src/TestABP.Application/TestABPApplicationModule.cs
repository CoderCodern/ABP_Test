using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TestABP.Authorization;

namespace TestABP
{
    [DependsOn(
        typeof(TestABPCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class TestABPApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<TestABPAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(TestABPApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
