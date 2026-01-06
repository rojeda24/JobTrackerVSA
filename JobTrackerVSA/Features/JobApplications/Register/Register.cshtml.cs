using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.JobApplications.Register
{
    public class RegisterModel(IMediator mediator) : PageModel
    {
        [BindProperty]
        public RegisterJobApplicationCommand Command { get; set; } = default!;
        public void OnGet()
        {
            Command = new RegisterJobApplicationCommand
            {
                CompanyName = string.Empty,
                Position = string.Empty,
                AppliedAt = DateTime.Today 
            };
        }
        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await mediator.Send(Command, cancellationToken);

            return RedirectToPage("/Index"); // TODO Change to the List of Applications
        }
    }
}
