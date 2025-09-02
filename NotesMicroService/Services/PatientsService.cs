using System.Text.Json;

namespace NotesMicroService.Services
{
    public class PatientsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PatientsService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public PatientsService(IHttpClientFactory httpClientFactory, ILogger<PatientsService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PatientsService");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<bool> PatientExistsAsync(Guid patientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/patients/{patientId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check if patient {PatientId} exists", patientId);
                return false;
            }
        }

        public async Task<PatientDto?> GetPatientAsync(Guid patientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/patients/{patientId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PatientDto>(content, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get patient {PatientId}", patientId);
                return null;
            }
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/patients");
                if (!response.IsSuccessStatusCode)
                    return Enumerable.Empty<PatientDto>();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<PatientDto>>(content, _jsonOptions) ?? Enumerable.Empty<PatientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get all patients");
                return Enumerable.Empty<PatientDto>();
            }
        }

        public async Task<bool> IsPatientActiveAsync(Guid patientId)
        {
            try
            {
                var patient = await GetPatientAsync(patientId);
                return patient?.IsActive == true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check if patient {PatientId} is active", patientId);
                return false;
            }
        }
    }

    // DTO to match your Patient model structure
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
        // Add other properties as needed
    }
}