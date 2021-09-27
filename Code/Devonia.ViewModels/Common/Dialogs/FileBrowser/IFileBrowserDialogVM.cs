/// Written by: Yulia Danilova
/// Creation Date: 03rd of July, 2021
/// Purpose: Interface for the view model for the FileBrowseDialog view
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using System.Collections.Generic;
using Devonia.ViewModels.Common.MVVM;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.FileBrowser
{
    public interface IFileBrowserDialogVM : IBaseModel
    {
        #region =============================================================== PROPERTIES ==================================================================================
        List<string> Filter { get; set; }
        string SelectedFiles { get; set; }
        string InitialFolder { get; set; }
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
