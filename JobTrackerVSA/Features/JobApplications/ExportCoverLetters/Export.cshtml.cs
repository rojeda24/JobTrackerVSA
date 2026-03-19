using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.JobApplications.ExportCoverLetters;

public class ExportModel(IMediator mediator) : PageModel
{
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ExportCoverLettersQuery(), cancellationToken);

        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.Error;
            return Page();
        }

        return File(result.Value, "text/markdown", "CoverLetters_Context.md");
    }
}
