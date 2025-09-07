using FrontEndMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEndMicroService.Pages
{
    public class NotesModel : PageModel
    {
        private readonly ILogger<NotesModel>
    _logger;
        private readonly HttpClient _httpClient;

        public List<NoteDTO> Notes { get; set; } = new();
        public PatientDTO? Patient { get; set; }
        public string Diagnosis { get; set; } = string.Empty;   
        
        public string? PatientId { get; set; }

        public NotesModel(ILogger<NotesModel>
            logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("Patients"); // Using same client for gateway
        }

        public async Task<IActionResult>
            OnGetAsync(string id)
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
                Patient = await _httpClient.GetFromJsonAsync<PatientDTO>
                    ($"patients/{id}");

                if (Patient == null)
                {
                    return NotFound($"Patient with ID {id} not found");
                }

                // Fetch notes for this patient
                _logger.LogInformation("Fetching notes for patient ID: {PatientId}", id);
                Notes = await _httpClient.GetFromJsonAsync<List<NoteDTO>>($"notes/patient/{id}") ?? new List<NoteDTO>();

                _logger.LogInformation("Found {Count} notes for patient {PatientName}", Notes.Count, $"{Patient.FirstName} {Patient.LastName}");

                // Fetch diag for this patient
                _logger.LogInformation("Fetching diag for patient ID: {PatientId}", id);
                Diagnosis = await _httpClient.GetFromJsonAsync<string>($"notes/diagnosis/{id}") ?? "Error";

                _logger.LogInformation("Diag for patient {PatientName}", Diagnosis);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data for patient {PatientId}", id);
                return StatusCode(500, "Error loading patient data");
            }
        }
    }


}
