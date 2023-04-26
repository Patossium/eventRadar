using System.Security.Claims;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;


namespace eventRadar.Auth
{
    public class ResourceUserAuthorizationHandler : AuthorizationHandler<ResourceUserRequirement, IUserOwnedResource>
    {
        List<string> tokenBlacklist = new List<string>();
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceUserRequirement requirement, IUserOwnedResource resource)
        {
            if (context.User.IsInRole(SystemRoles.Administrator) || context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) == resource.UserId)
            {
                context.Succeed(requirement);
            } 
            return Task.CompletedTask;
        }
    }
    public record ResourceUserRequirement : IAuthorizationRequirement;
}
