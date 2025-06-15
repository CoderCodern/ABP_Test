using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TestABP.Authorization.Roles;
using TestABP.Authorization.Users;
using TestABP.Configuration;
using TestABP.Localization;
using TestABP.MultiTenancy;
using TestABP.Timing;

namespace TestABP
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class TestABPCoreModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public TestABPCoreModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            TestABPLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = TestABPConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();
            
            Configuration.Localization.Languages.Add(new LanguageInfo("fa", "فارسی", "famfamfam-flags ir"));
            
            Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = TestABPConsts.DefaultPassPhrase;
            SimpleStringCipher.DefaultPassPhrase = TestABPConsts.DefaultPassPhrase;

            ConfigureCloudinary();
        }

        private void ConfigureCloudinary()
        {
            IocManager.Register<CloudinaryConfig>();
            var cloudinaryConfig = IocManager.Resolve<CloudinaryConfig>();

            cloudinaryConfig.CloudName = _appConfiguration[AppSettingNames.CloudName];
            cloudinaryConfig.APISecret = _appConfiguration[AppSettingNames.APISecret];
            cloudinaryConfig.APIKey = _appConfiguration[AppSettingNames.APIKey];
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(TestABPCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
