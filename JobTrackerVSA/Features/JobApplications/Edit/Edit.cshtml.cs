using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Shared;
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
            Form.Id = id;

            // Centralized data loading
            var result = await PopulateViewDataAsync(cancellationToken, forceFullLoad: true);

            if (result.IsFailure)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                // If model state is invalid, we MUST reload the read-only data (Interviews)
                // before returning the Page to avoid NullReferenceException in Razor.
                await PopulateViewDataAsync(cancellationToken, forceFullLoad: false);
                return Page();
            }

            var command = new EditJobApplicationCommand(
                Form.Id,
                Form.CompanyName,
                Form.Position,
                Form.JobDescriptionUrl,
                Form.AppliedAt,
                Form.Status,
                Form.Notes);

            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                // Same logic: failure in domain/persistence requires data repopulation.
                await PopulateViewDataAsync(cancellationToken, forceFullLoad: false);
                return Page();
            }

            return RedirectToPage("/JobApplications/List/Index");
        }

        /// <summary>
        /// Fetches existing data from the database to populate the UI.
        /// Used in both Initial GET and Postback failures.
        /// </summary>
        private async Task<Result<Unit>> PopulateViewDataAsync(CancellationToken cancellationToken, bool forceFullLoad)
        {
            var result = await mediator.Send(new GetJobApplicationForEditQuery(Form.Id), cancellationToken);

            if (result.IsFailure)
            {
                return Result<Unit>.Failure(result.Error);
            }

            // Map data from the Query result to our BindProperty 'Form'
            var data = result.Value;

            // Mapping read-only/auxiliary data (Always required for Razor)
            Form.Interviews = data.Interviews;

            // Mapping editable fields only if it's the first time loading (GET)
            if (forceFullLoad)
            {
                Form.CompanyName = data.CompanyName;
                Form.Position = data.Position;
                Form.JobDescriptionUrl = data.JobDescriptionUrl;
                Form.AppliedAt = data.AppliedAt;
                Form.Status = data.Status;
                Form.Notes = data.Notes;
            }

            return Result<Unit>.Success(Unit.Value);
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

            public List<InterviewSummaryViewModel> Interviews { get; set; } = [];
        }

        public record InterviewSummaryViewModel(Guid Id, DateTime ScheduledAt, string Type, string? Notes);
    }
}
