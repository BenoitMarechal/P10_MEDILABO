using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEndMicroService.Pages
{
    public class NotesModel : PageModel
    {
        private readonly ILogger<NotesModel> _logger;
        private readonly HttpClient _httpClient;
        public List<NoteDTO> Notes { get; set; } = new();
        public PatientDTO? Patient { get; set; }
        public string Diagnosis { get; set; } = string.Empty;

        public string? PatientId { get; set; }

        public NotesModel(ILogger<NotesModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("Patients");
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Patient ID is required");
            }

            PatientId = id;

            try
            {
                // Fetch patient information
                _logger.LogInformation("Fetching patient information for ID: {PatientId}", id);
                Patient = await _httpClient.GetFromJsonAsync<PatientDTO>($"patients/{id}");

                if (Patient == null)
                {
                    return NotFound($"Patient with ID {id} not found");
                }

                // Fetch notes for this patient
                _logger.LogInformation("Fetching notes for patient ID: {PatientId}", id);
                var notesUrl = $"notes/patient/{id}";
                _logger.LogInformation("Calling notes endpoint: {Url}", notesUrl);
                Notes = await _httpClient.GetFromJsonAsync<List<NoteDTO>>(notesUrl) ?? new List<NoteDTO>();
                _logger.LogInformation("Found {Count} notes for patient {PatientName}", Notes.Count, $"{Patient.FirstName} {Patient.LastName}");

                // Fetch diagnosis for this patient - improved error handling
                _logger.LogInformation("Fetching diagnosis for patient ID: {PatientId}", id);
                await FetchDiagnosisAsync(id);

                // Temporary fallback for testing UI
                if (string.IsNullOrEmpty(Diagnosis) || Diagnosis.Contains("Error") || Diagnosis.Contains("not available"))
                {
                    _logger.LogWarning("Using temporary diagnosis for testing");
                    Diagnosis = "Borderline"; // Temporary for testing - remove this later
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data for patient {PatientId}", id);
                return StatusCode(500, "Error loading patient data");
            }
        }

        private async Task FetchDiagnosisAsync(string patientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"notes/diagnosis/{patientId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Try to parse as JSON first (in case it returns {"diagnosis": "value"})
                    try
                    {
                        using var doc = JsonDocument.Parse(content);
                        if (doc.RootElement.TryGetProperty("diagnosis", out var diagnosisElement))
                        {
                            Diagnosis = diagnosisElement.GetString() ?? "No diagnosis available";
                        }
                        else if (doc.RootElement.ValueKind == JsonValueKind.String)
                        {
                            Diagnosis = doc.RootElement.GetString() ?? "No diagnosis available";
                        }
                        else
                        {
                            Diagnosis = content.Trim('"'); // Remove quotes if it's a quoted string
                        }
                    }
                    catch (JsonException)
                    {
                        // If it's not valid JSON, treat as plain text
                        Diagnosis = content.Trim('"');
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to fetch diagnosis for patient {PatientId}. Status: {StatusCode}",
                        patientId, response.StatusCode);
                    Diagnosis = "Diagnosis not available";
                }

                _logger.LogInformation("Diagnosis for patient {PatientId}: {Diagnosis}", patientId, Diagnosis);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while fetching diagnosis for patient {PatientId}", patientId);
                Diagnosis = "Error retrieving diagnosis";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching diagnosis for patient {PatientId}", patientId);
                Diagnosis = "Error retrieving diagnosis";
            }
        }
    }
}