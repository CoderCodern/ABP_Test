using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace TestABP.Controllers
{
    public abstract class TestABPControllerBase: AbpController
    {
        protected TestABPControllerBase()
        {
            LocalizationSourceName = TestABPConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
