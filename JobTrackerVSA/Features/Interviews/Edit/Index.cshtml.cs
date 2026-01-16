using JobTrackerVSA.Web.Features.JobApplications.GetDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.PortableExecutable;

namespace JobTrackerVSA.Web.Features.Interviews.Edit
{
    public class IndexModel(IMediator mediator) : PageModel
    {
        [BindProperty]
        public EditInterviewCommand Form { get; set; } = default!;

        // To show context to the user
        public JobApplicationHeaderViewModel JobHeader { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
        {

            // 1. Populate Interview
            var interviewResult = await mediator.Send(new GetInterviewForEditQuery(id), cancellationToken);
            if (interviewResult.IsFailure) return NotFound();
            Form = interviewResult.Value;

            // 2. Use the helper passing the jobId from the route
            if (!await TryPopulateHeader(Form.JobApplicationId, cancellationToken))
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                // Use the helper passing the ID from the hidden field in the form
                await TryPopulateHeader(Form.JobApplicationId, cancellationToken);
                return Page();
            }

            var result = await mediator.Send(Form, cancellationToken);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                ModelState.AddModelError("", result.Error);
                await TryPopulateHeader(Form.JobApplicationId, cancellationToken);
                return Page();
            }

            TempData["SuccessMessage"] = "Interview updated successfully!";
            return RedirectToPage("/JobApplications/Edit/Edit", new { id = Form.JobApplicationId });
        }
        private async Task<bool> TryPopulateHeader(Guid jobId, CancellationToken ct)
        {
            var result = await mediator.Send(new GetJobApplicationHeaderQuery(jobId), ct);

            if (result.IsFailure) return false;

            JobHeader = result.Value;
            return true;
        }
    }
}
