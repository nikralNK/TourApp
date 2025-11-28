using ShelterAppProduction.Models;

namespace ShelterAppProduction.Services
{
    public static class SessionManager
    {
        public static User CurrentUser { get; set; }
        public static bool IsAuthenticated => CurrentUser != null;
        public static bool IsAdmin => CurrentUser != null && CurrentUser.Role == "Admin";
    }
}
