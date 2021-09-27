/// Written by: Yulia Danilova
/// Creation Date: 30th of June, 2021
/// Purpose: Interface for custom folder browser dialogs
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
#endregion

namespace Devonia.Infrastructure.Dialog
{
    public interface IFolderBrowserService
    {
        #region =============================================================== PROPERTIES ==================================================================================
        string InitialFolder { get; set; }
        bool ShowNewFolderButton { get; set; }
        bool AllowMultiselection { get; set; }
        string SelectedDirectories { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows a new folder browser dialog
        /// </summary>
        /// <returns>A <see cref="NotificationResult"/> representing the result of displaying the custom folder browser dialog</returns>
        Task<NotificationResult> ShowAsync();
        #endregion
    }
}
