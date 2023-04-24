using System.Security.Claims;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;


namespace eventRadar.Auth
{
    public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement, IUserOwnedResource>
    {
        List<string> tokenBlacklist = new List<string>();
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOwnerRequirement requirement, IUserOwnedResource resource)
        {
            if (context.User.IsInRole(SystemRoles.Administrator) || context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) == resource.UserId)
            {
                context.Succeed(requirement);
            } 
            return Task.CompletedTask;
        }
    }
    public record ResourceOwnerRequirement : IAuthorizationRequirement;
}
