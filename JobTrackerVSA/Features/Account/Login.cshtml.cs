using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace JobTrackerVSA.Web.Features.Account;

[AllowAnonymous]
public class LoginModel(IConfiguration config) : PageModel
{
    public void OnGet(string returnUrl = "/")
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect(returnUrl);
        }
    }

    public async Task<IActionResult> OnPostDemoAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, config["Maintenance:DemoUserId"]!),
            new Claim(ClaimTypes.Name, "Demo User"),
            new Claim(ClaimTypes.Email, "demo@public.com"),
            new Claim("is_demo", "true")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return LocalRedirect("/");
    }

    public async Task OnPostLoginAsync(string returnUrl = "/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }
}
