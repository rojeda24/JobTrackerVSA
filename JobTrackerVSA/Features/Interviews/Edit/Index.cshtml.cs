using JobTrackerVSA.Web.Features.JobApplications.GetDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.Interviews.Edit
{
    public class IndexModel(IMediator mediator) : PageModel
    {
        [BindProperty]
        public EditInterviewCommand Form { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
        {
            var interviewResult = await mediator.Send(new GetInterviewForEditQuery(id), cancellationToken);
            if (interviewResult.IsFailure) return NotFound();
            Form = interviewResult.Value;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return Page();

            var result = await mediator.Send(Form, cancellationToken);

            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error);
                return Page();
            }

            return RedirectToPage("/JobApplications/Edit/Edit", new { id = Form.JobApplicationId });
        }
    }
}
