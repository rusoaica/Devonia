/// Written by: Yulia Danilova
/// Creation Date: 04th of July, 2021
/// Purpose: Interface for custom file save dialogs
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using System.Collections.Generic;
#endregion

namespace Devonia.Infrastructure.Dialog
{
    public interface IFileSaveService
    {
        #region =============================================================== PROPERTIES ==================================================================================
        List<string> Filter { get; set; }
        string Filename { get; set; }
        string InitialFolder { get; set; }
        bool OverwriteExisting { get; }
        bool ShowNewFolderButton { get; set; }
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
