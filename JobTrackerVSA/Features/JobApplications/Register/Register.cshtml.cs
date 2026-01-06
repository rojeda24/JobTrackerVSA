using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobTrackerVSA.Web.Features.JobApplications.Register
{
    public class RegisterModel(IMediator mediator) : PageModel
    {
        [BindProperty]
        public RegisterJobApplicationCommand Command { get; set; } = default!;
        public void OnGet()
        {
            Command = new RegisterJobApplicationCommand
            {
                CompanyName = string.Empty,
                Position = string.Empty,
                AppliedAt = DateTime.Today // El usuario ve la fecha de hoy por defecto
            };
        }
        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Enviamos el comando al Handler a través de MediatR
            // Observa cómo pasamos el cancellationToken que viene del navegador
            await mediator.Send(Command, cancellationToken);

            // Redirigimos a la lista de postulaciones (que crearemos después) 
            // o al Index por ahora
            return RedirectToPage("/Index");
        }
    }
}
