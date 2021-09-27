/// Written by: Yulia Danilova
/// Creation Date: 27th of September, 2021
/// Purpose: Model for strongly typed user accounts

namespace Devonia.Infrastructure.Configuration
{
    public class UserConfigEntity
    {
        #region =============================================================== PROPERTIES ==================================================================================
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        #endregion
    }
}
