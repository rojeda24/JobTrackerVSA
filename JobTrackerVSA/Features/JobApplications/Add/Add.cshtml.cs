using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.JobApplications.Add
{
    public class AddModel(IMediator mediator) : PageModel
    {
        [BindProperty]
        public AddJobApplicationCommand Command { get; set; } = default!;
        public void OnGet()
        {
            Command = new AddJobApplicationCommand
            {
                CompanyName = string.Empty,
                Position = string.Empty,
                AppliedAt = DateTime.Today 
            };
        }
        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return Page();

            var result = await mediator.Send(Command, cancellationToken);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return Page();
            }

            TempData["SuccessMessage"] = "Application saved! Now, let's schedule an interview.";

            return RedirectToPage("/JobApplications/Edit/Edit", new { id = result.Value });
        }
    }
}
