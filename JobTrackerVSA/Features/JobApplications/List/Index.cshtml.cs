using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace JobTrackerVSA.Web.Features.JobApplications.List
{
    public class IndexModel(IMediator mediator) : PageModel
    {
        public List<JobApplicationSummaryViewModel> Applications { get; private set; } = [];
        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            var query = new GetJobApplicationsQuery();
            var appsResult = await mediator.Send(query, cancellationToken);

            if (appsResult.IsSuccess)
                Applications = appsResult.Value;
            else
                TempData["ErrorMessage"] = appsResult.Error;

            return Page();
        }
    }
}
