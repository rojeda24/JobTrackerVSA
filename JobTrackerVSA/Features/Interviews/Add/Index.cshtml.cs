using JobTrackerVSA.Web.Features.JobApplications.GetDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.PortableExecutable;
using static JobTrackerVSA.Web.Domain.Interview;

namespace JobTrackerVSA.Web.Features.Interviews.Add
{
    public class IndexModel(IMediator mediator) : PageModel
    {
        [BindProperty]
        public AddInterviewCommand Form { get; set; } = null!;

        // To show context to the user
        public JobApplicationHeaderViewModel JobHeader { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid jobId, CancellationToken ct)
        {
            // 1. Use the helper passing the jobId from the route
            if (!await TryPopulateHeader(jobId, ct))
            {
                return NotFound();
            }

            // 2. Initialize the form now that we know the job exists
            var now = DateTime.UtcNow; //Includes seconds
            var nowHourMinute = now.AddTicks(-(now.Ticks % TimeSpan.TicksPerMinute)); //Excludes seconds and milliseconds
            Form = new AddInterviewCommand(jobId, nowHourMinute, InterviewType.General, string.Empty);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                // Use the helper passing the ID from the hidden field in the form
                await TryPopulateHeader(Form.JobApplicationId, ct);
                return Page();
            }

            var result = await mediator.Send(Form, ct);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                await TryPopulateHeader(Form.JobApplicationId, ct);
                return Page();
            }

            TempData["SuccessMessage"] = "Interview scheduled successfully!";
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
