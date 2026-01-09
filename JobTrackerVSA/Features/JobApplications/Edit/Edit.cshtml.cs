using JobTrackerVSA.Web.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public class EditModel(IMediator mediator) : PageModel
    {
        [BindProperty]
        public InputModel Form { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetJobApplicationForEditQuery(id), cancellationToken);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToPage("/JobApplications/List/Index");
            }

            Form = result.Value;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please, check and correct any error in form";
                return Page();
            }

            await mediator.Send(new EditJobApplicationCommand(
            Form.Id,
            Form.CompanyName,
            Form.Position,
            Form.JobDescriptionUrl,
            Form.AppliedAt,
            Form.Status,
            Form.Notes
            ), cancellationToken);

            return RedirectToPage("/JobApplications/List/Index");
        }

        public class InputModel
        {
            public Guid Id { get; set; }
            public string CompanyName { get; set; } = string.Empty;
            public string Position { get; set; } = string.Empty;
            public string? JobDescriptionUrl { get; set; }
            public DateTime AppliedAt { get; set; }
            public JobApplication.ApplicationStatus Status { get; set; }
            public string? Notes { get; set; }

        }
    }
}
