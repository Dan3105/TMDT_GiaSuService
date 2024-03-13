using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GiaSuService.Handler.MyCustomAuthorize
{
    public class ShouldBeAdminRequirementAuthorization : AuthorizationHandler<ShouldRoleRequire>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShouldRoleRequire requirement)
        {
            var roleClaim = context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role);
            if (roleClaim == null || roleClaim.Value.ToLower() != requirement.RoleNameRequire)
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
        
    public class ShouldRoleRequire : IAuthorizationRequirement
    {
        public readonly string RoleNameRequire;
        public ShouldRoleRequire(string _RoleNameRequire) { 
            this.RoleNameRequire = _RoleNameRequire;
        }
    }
}
