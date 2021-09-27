/// Written by: Yulia Danilova
/// Creation Date: 14th of May, 2021
/// Purpose: Interface for the application's configuration
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using System.Collections.Generic;
#endregion

namespace Devonia.Infrastructure.Configuration
{
    public interface IAppConfig
    {
        #region ================================================================ PROPERTIES =================================================================================
        string ConfigurationFilePath { get; }
        List<UserConfigEntity> Users { get; set; }
        DialogsConfigEntity Dialogs { get; set; }
        ApplicationConfigEntity Application { get; set; }
        MiscellaneousConfigEntity Miscellaneous { get; set; }
        AuthenticationConfigEntity Authentication { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Saves the application's configuration settings
        /// </summary>
        Task UpdateConfigurationAsync();
        #endregion
    }
}
