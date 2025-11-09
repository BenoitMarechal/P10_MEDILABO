using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FrontEndMicroService.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public List<PatientDTO> Patients { get; set; } = new();
        public string Username { get; set; } = "";

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //  Ensure user is authenticated
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No JWT found in session, redirecting to login...");
                return RedirectToPage("/Login");
            }

            Username = HttpContext.Session.GetString("Username") ?? "User";

            try
            {
                var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "http://apigateway";

                var httpClient = _httpClientFactory.CreateClient();

                httpClient.BaseAddress = new Uri($"{gatewayBaseUrl}/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                _logger.LogInformation("Fetching patients via Ocelot Gateway at: {Url}", httpClient.BaseAddress);

                var response = await httpClient.GetAsync("patients");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized (401): Token likely expired, redirecting to login.");
                    HttpContext.Session.Clear();
                    return RedirectToPage("/Login");
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var patients = JsonSerializer.Deserialize<List<PatientDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (patients != null)
                    Patients = patients;

                _logger.LogInformation("Fetched {Count} patients", Patients.Count);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Ocelot gateway failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching patients.");
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
                var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "http://apigateway";

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri($"{gatewayBaseUrl}/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.DeleteAsync($"patients/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized (401) during delete - redirecting to login");
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
