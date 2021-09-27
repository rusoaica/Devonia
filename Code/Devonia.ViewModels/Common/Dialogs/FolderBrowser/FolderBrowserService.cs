/// Written by: Yulia Danilova
/// Creation Date: 30th of June, 2021
/// Purpose: Explicit implementation of abstract custom folder browser service
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.Infrastructure.Dialog;
using Devonia.ViewModels.Common.Dispatcher;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.FolderBrowser
{
    /// <summary>
    /// A service that shows folder browser dialogs
    /// </summary>
    public class FolderBrowserService : IFolderBrowserService
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IDispatcher dispatcher;
        private readonly Func<IFolderBrowserDialogVM> folderBrowserVM;
        #endregion
      
        #region =============================================================== PROPERTIES ==================================================================================
        public string InitialFolder { get; set; }
        public string SelectedDirectories { get; set; }
        public bool AllowMultiselection { get; set; }
        public bool ShowNewFolderButton { get; set; }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use</param>
        /// <param name="folderBrowserVM">A Func that will force the creation of a new folder browser dialog viewmodel instance on each call</param>
        /// <remarks>When using simple constructor injection, the folder instance is created only once, and can no longer trigger new views after the 
        /// first one is disposed, therefor, the need of a new viewmodel on each call. Method injection could have been an approach too, but that would 
        /// require changing the signatures of the Show() methods in the interface, which is unacceptable. Service Locator pattern is also unacceptable, 
        /// and the viewmodel is not aware of a DI container anyway. Sending the DI container as injected parameter is definitely unacceptable.</remarks>
        public FolderBrowserService(IDispatcher dispatcher, Func<IFolderBrowserDialogVM> folderBrowserVM)
        {
            this.dispatcher = dispatcher;
            this.folderBrowserVM = folderBrowserVM;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows a new custom folder browser dialog
        /// </summary>
        /// <returns>A <see cref="NotificationResult"/> value representing the result of the custom folder browser dialog</returns>
        public async Task<NotificationResult> ShowAsync()
        {
            return await await dispatcher?.DispatchAsync(new Func<Task<NotificationResult>>(async () =>
            {
                IFolderBrowserDialogVM folderBrowserDialogVM = folderBrowserVM.Invoke();
                folderBrowserDialogVM.InitialFolder = InitialFolder;
                folderBrowserDialogVM.SelectedDirectories = SelectedDirectories;
                folderBrowserDialogVM.ShowNewFolderButton = ShowNewFolderButton;
                folderBrowserDialogVM.AllowMultiselection = AllowMultiselection;
                // display the folder browse dialog as modal, and get its result
                NotificationResult result = await folderBrowserDialogVM.ShowAsync();
                // after folder browse dialog is closed, relay the selected folders
                SelectedDirectories = folderBrowserDialogVM.SelectedDirectories;
                return result;
            }));
        }
        #endregion
    }
}
