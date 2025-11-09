using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FrontEndMicroService.Pages.Patients
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EditModel> _logger;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public PatientDTO Patient { get; set; } = new();

        public EditModel(ILogger<EditModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateAuthenticatedClient()
        {
            var token = HttpContext.Session.GetString("JWT");
            var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "http://apigateway:80";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri($"{gatewayBaseUrl}/");

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // Check if user is logged in
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No JWT found in session, redirecting to login...");
                return RedirectToPage("/Login");
            }

            try
            {
                var httpClient = CreateAuthenticatedClient();

                _logger.LogInformation("Fetching patient {PatientId} for editing", id);

                Patient = await httpClient.GetFromJsonAsync<PatientDTO>($"patients/{id}");

                if (Patient == null)
                {
                    _logger.LogWarning("Patient {PatientId} not found", id);
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient {PatientId}", id);
                return StatusCode(500, "Error loading patient data");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No JWT found in session, redirecting to login...");
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for patient {PatientId}", Patient.Id);
                return Page();
            }

            try
            {
                var httpClient = CreateAuthenticatedClient();

                _logger.LogInformation("Updating patient {PatientId}", Patient.Id);

                var response = await httpClient.PutAsJsonAsync($"patients/{Patient.Id}", Patient);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized - redirecting to login");
                    HttpContext.Session.Clear();
                    return RedirectToPage("/Login");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to update patient {PatientId}. Status: {StatusCode}, Error: {Error}",
                        Patient.Id, response.StatusCode, errorContent);
                    ModelState.AddModelError(string.Empty, "Failed to update patient.");
                    return Page();
                }

                _logger.LogInformation("Successfully updated patient {PatientId}", Patient.Id);
                TempData["SuccessMessage"] = "Patient updated successfully!";
                return Redirect("/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId}", Patient.Id);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return Page();
            }
        }
    }
}