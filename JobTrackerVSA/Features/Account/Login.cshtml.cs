using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public void OnGet(string returnUrl = "/")
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                Response.Redirect(returnUrl);
            }
        }

        public async Task OnPostLoginAsync(string returnUrl = "/")
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        }
    }
}
