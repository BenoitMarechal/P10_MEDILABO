using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEndMicroService.Pages.Patients
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly HttpClient _httpClient;

        public CreateModel(ILogger<CreateModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("Patients");
        }

        [BindProperty]
        public PatientDTO Patient { get; set; } = new PatientDTO();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed. Errors: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return Page();
            }

            try
            {
                _logger.LogInformation("Creating new patient: {FirstName} {LastName}", Patient.FirstName, Patient.LastName);
                _logger.LogInformation("Client base address: {BaseAddress}", _httpClient.BaseAddress);

                // Set the ID for new patient
                Patient.Id = Guid.NewGuid();

                // Log the complete patient object as JSON
                var patientJson = JsonSerializer.Serialize(Patient, new JsonSerializerOptions { WriteIndented = true });
                _logger.LogInformation("Complete Patient Object: {PatientJson}", patientJson);

                // Call through the gateway - same pattern as your IndexModel
                var response = await _httpClient.PostAsJsonAsync("patients", Patient);

                // Log response details
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response Status: {StatusCode}, Content: {ResponseContent}",
                    response.StatusCode, responseContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully created patient with ID: {PatientId}", Patient.Id);
                    _logger.LogInformation("Redirecting to home page...");
                    return Redirect("/");
                    // return RedirectToPage("./Index");
                }
                else
                {
                    _logger.LogWarning("Failed to create patient. Status: {StatusCode}, Reason: {ReasonPhrase}",
                        response.StatusCode, response.ReasonPhrase);
                    ModelState.AddModelError(string.Empty, $"Failed to create patient. Server returned: {response.StatusCode}");
                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while creating patient");
                ModelState.AddModelError(string.Empty, "Unable to connect to the patient service. Please try again.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating patient");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return Page();
            }
        }
    }
}