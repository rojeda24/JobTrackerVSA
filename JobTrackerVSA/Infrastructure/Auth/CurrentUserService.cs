using System.Security.Claims;

namespace JobTrackerVSA.Web.Infrastructure.Auth
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string? UserId
        {
            get
            {
                var user = httpContextAccessor.HttpContext?.User;
                // Auth0 uses ClaimTypes.NameIdentifier
                return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }
    }
}
