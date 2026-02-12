using JobTrackerVSA.Web.Features.JobApplications.Delete;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace JobTrackerVSA.Web.Features.JobApplications.List
{
    public class IndexModel(IMediator mediator) : PageModel
    {
        public PagedList<JobApplicationSummaryViewModel> PagedApplications { get; private set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            var query = new GetJobApplicationsQuery(PageNumber);
            var appsResult = await mediator.Send(query, cancellationToken);

            if (appsResult.IsSuccess)
                PagedApplications = appsResult.Value;
            else
                TempData["ErrorMessage"] = appsResult.Error;

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new DeleteJobApplicationCommand(id), cancellationToken);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
            }
            else
            {
                TempData["SuccessMessage"] = "Job application deleted successfully.";
            }

            return RedirectToPage();
        }
    }
}
