namespace IdentityMicroService.Services
{
    public interface IJWTService
    {  
        string GenerateToken(string username, IEnumerable<string> roles = null);
    }
}
