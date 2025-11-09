using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FrontEndMicroService.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LoginModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (!string.IsNullOrEmpty(token))
            {
                Response.Redirect("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var identityServiceUrl = _configuration["IdentityService:Url"] ?? "http://localhost:5000/identity";
            var client = _httpClientFactory.CreateClient();

            var loginRequest = new
            {
                email = Email,
                password = Password
            };

            var jsonContent = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{identityServiceUrl}/login?useCookies=false", content);
                _logger.LogInformation("Login response status: {StatusCode}", response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Login response body: {Body}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    //  FIX: property name must match your IdentityController response
                    if (!string.IsNullOrEmpty(loginResponse?.Token))
                    {
                        // Save JWT to session
                        HttpContext.Session.SetString("JWT", loginResponse.Token);
                        HttpContext.Session.SetString("Username", Email);

                        // Optional cookie
                        Response.Cookies.Append("JWT", loginResponse.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false, // set true in prod with HTTPS
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddHours(1)
                        });

                        _logger.LogInformation("Login successful, redirecting to Index");
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        ErrorMessage = "Login succeeded but token missing in response.";
                    }
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to identity service");
                ErrorMessage = $"Error connecting to identity service: {ex.Message}";
            }

            return Page();
        }

        private class LoginResponse
        {
            public string? Token { get; set; } //  match backend response
            public DateTime? Expiration { get; set; }
        }
    }
}
