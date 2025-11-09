using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace FrontEndMicroService.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RegisterModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var identityServiceUrl = _configuration["IdentityService:Url"] ?? "http://localhost:5003/api/identity";
            var client = _httpClientFactory.CreateClient();

            var registerRequest = new
            {
                email = Email,
                password = Password
            };

            var jsonContent = JsonSerializer.Serialize(registerRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{identityServiceUrl}/register", content);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Registration successful! You can now log in.";
                    // Clear form
                    Email = string.Empty;
                    Password = string.Empty;
                    ConfirmPassword = string.Empty;
                    ModelState.Clear();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (errorResponse?.Errors != null && errorResponse.Errors.Any())
                        {
                            ErrorMessage = string.Join(", ", errorResponse.Errors.Select(e => e.Description));
                        }
                        else
                        {
                            ErrorMessage = "Registration failed. Please try again.";
                        }
                    }
                    catch
                    {
                        ErrorMessage = "Registration failed. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error connecting to identity service: {ex.Message}";
            }

            return Page();
        }

        private class ErrorResponse
        {
            public List<ErrorDetail>? Errors { get; set; }
        }

        private class ErrorDetail
        {
            public string Code { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}