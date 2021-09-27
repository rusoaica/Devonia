/// Written by: Yulia Danilova
/// Creation Date: 14th of May, 2021
/// Purpose: Model for strongly typed miscellaneous configuration values

namespace Devonia.Infrastructure.Configuration
{
    public class MiscellaneousConfigEntity
    {
        #region =============================================================== PROPERTIES ==================================================================================
        public string SelectedTheme { get; set; }
        public bool UsesAuthentication { get; set; }
        #endregion
    }
}
