using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public string ErrorMessage { get; set; } = "";

    public IActionResult OnGet()
    {
        // If already logged in, redirect to home
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWT")))
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string username, string password)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("Patients");

            var loginRequest = new { username, password };
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("identity/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<JsonElement>(responseJson);

                var token = loginResponse.GetProperty("token").GetString();
                var user = loginResponse.GetProperty("username").GetString();

                // Store in session
                HttpContext.Session.SetString("JWT", token);
                HttpContext.Session.SetString("Username", user);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Login failed. Please try again.";
        }

        return Page();
    }
}