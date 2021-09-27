/// Written by: Yulia Danilova
/// Creation Date: 26th of November, 2020
/// Purpose: Interface business model for user interface options
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
#endregion

namespace Devonia.Models.Core.Options
{
    public interface IOptionsInterface
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        event Action<string> PropertyChanged;
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        string WeatherUrl { get; set; }
        string BackgroundImagePath { get; set; }
        int SelectedThemeIndex { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the user interface options from the application's configuration
        /// </summary>        
        void GetUserInterfaceOptions();

        /// <summary>
        /// Updates the application's configurations for the user interface
        /// </summary>
        Task UpdateUserInterfaceOptionsAsync();
        #endregion
    }
}
