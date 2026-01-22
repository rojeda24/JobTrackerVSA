using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnGet(string returnUrl = "/")
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(returnUrl);
            }

            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            return Challenge(authenticationProperties, Auth0Constants.AuthenticationScheme);
        }
    }
}
