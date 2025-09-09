using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEndMicroService.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;

        public List<PatientDTO> Patients { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("Patients");
        }

        public async Task OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Client base address: {BaseAddress}", _httpClient.BaseAddress);

                // Call through the gateway
                var patients = await _httpClient.GetFromJsonAsync<List<PatientDTO>>("patients");

                _logger.LogInformation("Fetched {Count} patients", patients?.Count ?? 0);

                if (patients != null)
                {
                    Patients = patients;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch patients through Ocelot gateway");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                // Sends DELETE to your backend microservice as a route: patients/{id}
                var response = await _httpClient.DeleteAsync($"patients/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to delete patient {Id}: {StatusCode}", id, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting patient {Id}", id);
            }

            // Refresh the current page
            return RedirectToPage();
        }

    }
}