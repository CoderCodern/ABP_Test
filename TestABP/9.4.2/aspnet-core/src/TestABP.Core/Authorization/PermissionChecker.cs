using Abp.Authorization;
using TestABP.Authorization.Roles;
using TestABP.Authorization.Users;

namespace TestABP.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
