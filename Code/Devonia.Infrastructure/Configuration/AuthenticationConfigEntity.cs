/// Written by: Yulia Danilova
/// Creation Date: 14th of May, 2021
/// Purpose: Model for strongly typed authentication configuration values

namespace Devonia.Infrastructure.Configuration
{
    public class AuthenticationConfigEntity
    {
        #region =============================================================== PROPERTIES ==================================================================================
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecurityAnswer { get; set; }
        public string SecurityQuestion { get; set; }
        public bool Autologin { get; set; }
        public bool RememberCredentials { get; set; }
        #endregion
    }
}
