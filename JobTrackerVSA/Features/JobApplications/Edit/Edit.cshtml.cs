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

            if (result == null) return NotFound();

            Form = result;
            return Page();
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
