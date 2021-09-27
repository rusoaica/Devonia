/// Written by: Yulia Danilova
/// Creation Date: 27th of June, 2021
/// Purpose: Interface for the view model for the FolderBrowseDialog view
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.ViewModels.Common.MVVM;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.FolderBrowser
{
    public interface IFolderBrowserDialogVM : IBaseModel
    {
        #region =============================================================== PROPERTIES ==================================================================================
        string InitialFolder { get; set; }
        string SelectedDirectories { get; set; }
        bool ShowNewFolderButton { get; set; }
        bool AllowMultiselection { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows a new instance of the folder browser dialog
        /// </summary>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the folder browser dialog</returns>
        Task<NotificationResult> ShowAsync();
        #endregion
    }
}
