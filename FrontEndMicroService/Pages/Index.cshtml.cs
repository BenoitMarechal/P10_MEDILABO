using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEndMicroService.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public List<PatientDTO> Patients { get; set; } = new();
        public string Username { get; set; } = "";

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            Username = HttpContext.Session.GetString("Username") ?? "User";

            try
            {
                var httpClient = _httpClientFactory.CreateClient("Patients");

                // Add JWT token to request
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                _logger.LogInformation("Client base address: {BaseAddress}", httpClient.BaseAddress);
                _logger.LogInformation("Using JWT token: {Token}", token?.Substring(0, Math.Min(token.Length, 50)) + "...");
                _logger.LogInformation("Making request to: patients");

                // Call through the gateway
                var patients = await httpClient.GetFromJsonAsync<List<PatientDTO>>("patients");
                _logger.LogInformation("Fetched {Count} patients", patients?.Count ?? 0);

                if (patients != null)
                {
                    Patients = patients;
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401"))
            {
                _logger.LogWarning("Token expired or invalid - redirecting to login");
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch patients through Ocelot gateway");
                // Keep Patients as empty list so page still renders
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("Patients");

                // Add JWT token to request
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Delete patient
                var response = await httpClient.DeleteAsync($"patients/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired during delete - redirecting to login");
                    HttpContext.Session.Clear();
                    return RedirectToPage("/Login");
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to delete patient {Id}: {StatusCode}", id, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting patient {Id}", id);
            }

            return RedirectToPage();
        }
    }
}