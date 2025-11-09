using Microsoft.AspNetCore.Http;

namespace FrontEndMicroService.DTOs.Helpers
{
    public static class AuthHelper
    {
        public static string? GetAuthToken(HttpContext httpContext)
        {
            // Try to get token from session first
            var token = httpContext.Session.GetString("AuthToken");

            // If not in session, try cookie
            if (string.IsNullOrEmpty(token))
            {
                httpContext.Request.Cookies.TryGetValue("AuthToken", out token);
            }

            return token;
        }

        public static bool IsAuthenticated(HttpContext httpContext)
        {
            var token = GetAuthToken(httpContext);
            return !string.IsNullOrEmpty(token);
        }

        public static string? GetUserEmail(HttpContext httpContext)
        {
            return httpContext.Session.GetString("UserEmail");
        }
    }
}