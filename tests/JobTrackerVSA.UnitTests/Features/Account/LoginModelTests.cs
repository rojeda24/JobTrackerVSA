using FluentAssertions;
using JobTrackerVSA.Web.Features.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NSubstitute;

namespace JobTrackerVSA.UnitTests.Features.Account
{
    public class LoginModelTests
    {
        [Fact]
        public void OnGet_Should_Redirect_When_UserIsAlreadyAuthenticated()
        {
            // Arrange
            var loginModel = new LoginModel();
            
            // Mock HttpContext and User
            var httpContext = new DefaultHttpContext();
            var userPrincipal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(
                    new[] { new System.Security.Claims.Claim("sub", "user1") }, 
                    "TestAuthType" // AuthenticationType must be set for IsAuthenticated to be true
                )
            );
            httpContext.User = userPrincipal;

            loginModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            
            // Act
            loginModel.OnGet("/");

            // Assert
            httpContext.Response.StatusCode.Should().Be(302);
            httpContext.Response.Headers["Location"].ToString().Should().Be("/");
        }

        [Fact]
        public async Task OnPostDemoAsync_Should_SignIn_And_Redirect()
        {
            // Arrange
            var loginModel = new LoginModel();
            
            // Setup Authentication Service Mock
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };
            
            loginModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await loginModel.OnPostDemoAsync();

            // Assert
            // 1. Verify Redirect
            var redirectResult = result.Should().BeOfType<LocalRedirectResult>().Subject;
            redirectResult.Url.Should().Be("/");

            // 2. Verify SignIn was called
            await authService.Received().SignInAsync(
                httpContext, 
                Arg.Any<string>(), // Scheme
                Arg.Is<System.Security.Claims.ClaimsPrincipal>(p => 
                    p.HasClaim(c => c.Type == "is_demo" && c.Value == "true") &&
                    p.Identity!.Name == "Demo User"
                ), 
                Arg.Any<AuthenticationProperties?>()
            );
        }
    }
}
