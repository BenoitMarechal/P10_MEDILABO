using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEndMicroService.Pages.Patients
{

        public class EditModel : PageModel
        {
            private readonly HttpClient _httpClient;

            [BindProperty]
            public PatientDTO Patient { get; set; } = new();

            public EditModel(IHttpClientFactory httpClientFactory)
            {
                _httpClient = httpClientFactory.CreateClient("Patients");
            }

            public async Task<IActionResult> OnGetAsync(Guid id)
            {
                Patient = await _httpClient.GetFromJsonAsync<PatientDTO>($"patients/{id}");
                if (Patient == null) return NotFound();
                return Page();
            }

            public async Task<IActionResult> OnPostAsync()
            {
                if (!ModelState.IsValid)
                    return Page();

                var response = await _httpClient.PutAsJsonAsync($"patients/{Patient.Id}", Patient);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update patient.");
                    return Page();
                }
            TempData["SuccessMessage"] = "Patient updated successfully!";
            return Redirect("/");
            }
        }

}