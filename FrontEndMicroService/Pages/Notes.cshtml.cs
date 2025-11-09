using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FrontEndMicroService.Pages
{
    public class NotesModel : PageModel
    {
        private readonly ILogger<NotesModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public List<NoteDTO> Notes { get; set; } = new();
        public PatientDTO? Patient { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string? PatientId { get; set; }

        public NotesModel(ILogger<NotesModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public string NewNoteContent { get; set; } = string.Empty;

        private HttpClient CreateAuthenticatedClient()
        {
            var token = HttpContext.Session.GetString("JWT");
            var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "http://apigateway:5000";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri($"{gatewayBaseUrl}/");

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Check if user is logged in
            var token = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Patient ID is required");
            }

            PatientId = id;

            try
            {
                var httpClient = CreateAuthenticatedClient();

                // Fetch patient information
                _logger.LogInformation("Fetching patient information for ID: {PatientId}", id);
                Patient = await httpClient.GetFromJsonAsync<PatientDTO>($"patients/{id}");

                if (Patient == null)
                {
                    return NotFound($"Patient with ID {id} not found");
                }

                // Fetch notes for this patient
                _logger.LogInformation("Fetching notes for patient ID: {PatientId}", id);
                var notesUrl = $"notes/patient/{id}";
                _logger.LogInformation("Calling notes endpoint: {Url}", notesUrl);
                Notes = await httpClient.GetFromJsonAsync<List<NoteDTO>>(notesUrl) ?? new List<NoteDTO>();
                _logger.LogInformation("Found {Count} notes for patient {PatientName}", Notes.Count, $"{Patient.FirstName} {Patient.LastName}");

                // Fetch diagnosis for this patient
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
                var httpClient = CreateAuthenticatedClient();
                var response = await httpClient.GetAsync($"notes/diagnosis/{patientId}");

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
                        _logger.LogWarning("Response was not valid JSON, treating as plain text");
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

        public async Task<IActionResult> OnPostDeleteNoteAsync(Guid id, Guid noteId)
        {
            // `id` = patientId (from route /notes/patient/{id})
            // `noteId` = note being deleted
            var httpClient = CreateAuthenticatedClient();

            var response = await httpClient.DeleteAsync($"notes/{noteId}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete note.");
                return Page();
            }

            // Refresh page with same patient id
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostAddNoteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(NewNoteContent))
            {
                ModelState.AddModelError("NewNoteContent", "Note content cannot be empty.");
                return await OnGetAsync(id); // reload page with validation error
            }

            try
            {
                var httpClient = CreateAuthenticatedClient();

                var newNote = new
                {
                    PatientId = id,
                    Content = NewNoteContent
                };

                var response = await httpClient.PostAsJsonAsync("notes", newNote);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully added note for patient {PatientId}", id);
                    return RedirectToPage(new { id }); // reload notes list
                }
                else
                {
                    _logger.LogWarning("Failed to add note for patient {PatientId}. Status: {StatusCode}",
                        id, response.StatusCode);
                    ModelState.AddModelError("", "Failed to add note. Please try again.");
                    return await OnGetAsync(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding note for patient {PatientId}", id);
                ModelState.AddModelError("", "Unexpected error while adding note.");
                return await OnGetAsync(id);
            }
        }
    }
}