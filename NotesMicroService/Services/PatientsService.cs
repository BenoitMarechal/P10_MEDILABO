using NotesMicroService.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotesMicroService.Services
{
    public class PatientsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PatientsService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientsService(IHttpClientFactory httpClientFactory, ILogger<PatientsService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("PatientsService");
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                // Ensure we only set once to avoid duplicate headers
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }
        }

        public async Task<bool> PatientExistsAsync(Guid patientId)
        {
            try
            {
                AddAuthHeader();
                var response = await _httpClient.GetAsync($"api/patients/{patientId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check if patient {PatientId} exists", patientId);
                return false;
            }
        }

        public async Task<PatientDTO?> GetPatientAsync(Guid patientId)
        {
            try
            {
                AddAuthHeader();
                var response = await _httpClient.GetAsync($"api/patients/{patientId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Patient API response: {Content}", content);
                return JsonSerializer.Deserialize<PatientDTO>(content, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get patient {PatientId}", patientId);
                return null;
            }
        }

        public async Task<IEnumerable<PatientDTO>> GetAllPatientsAsync()
        {
            try
            {
                AddAuthHeader();
                var response = await _httpClient.GetAsync("api/patients");
                if (!response.IsSuccessStatusCode)
                    return Enumerable.Empty<PatientDTO>();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<PatientDTO>>(content, _jsonOptions) ?? Enumerable.Empty<PatientDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get all patients");
                return Enumerable.Empty<PatientDTO>();
            }
        }
    }
}