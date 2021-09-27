/// Written by: Yulia Danilova
/// Creation Date: 03rd of July, 2021
/// Purpose: Explicit implementation of abstract custom file browser service
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.Infrastructure.Dialog;
using System.Collections.Generic;
using Devonia.ViewModels.Common.Dispatcher;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.FileBrowser
{
    /// <summary>
    /// A service that shows file browser dialogs
    /// </summary>
    public class FileBrowserService : IFileBrowserService
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IDispatcher dispatcher;
        private readonly Func<IFileBrowserDialogVM> fileBrowserVM;
        #endregion

        #region =============================================================== PROPERTIES ==================================================================================
        public List<string> Filter { get; set; }
        public string SelectedFiles { get; set; }
        public string InitialFolder { get; set; }
        public bool ShowNewFolderButton { get; set; }
        public bool AllowMultiselection { get; set; }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use</param>
        /// <param name="fileBrowserVM">A Func that will force the creation of a new file browser dialog viewmodel instance on each call</param>
        /// <remarks>When using simple constructor injection, the file browser dialog instance is created only once, and can no longer trigger new views after the 
        /// first one is disposed, therefore the need of a new viewmodel on each call. Method injection could have been an approach too, but that would 
        /// require changing the signatures of the Show() methods in the interface, which is unacceptable. Service Locator pattern is also unacceptable, 
        /// and the viewmodel is not aware of a DI container anyway. Sending the DI container as injected parameter is definitely unacceptable.</remarks>
        public FileBrowserService(IDispatcher dispatcher, Func<IFileBrowserDialogVM> fileBrowserVM)
        {
            this.dispatcher = dispatcher;
            this.fileBrowserVM = fileBrowserVM;
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
                IFileBrowserDialogVM fileBrowserDialogVM = fileBrowserVM.Invoke();
                fileBrowserDialogVM.Filter = Filter;
                fileBrowserDialogVM.InitialFolder = InitialFolder;
                fileBrowserDialogVM.SelectedFiles = SelectedFiles;
                fileBrowserDialogVM.AllowMultiselection = AllowMultiselection;
                fileBrowserDialogVM.ShowNewFolderButton = ShowNewFolderButton;
                // display the file browse dialog as modal, and await its result
                NotificationResult result = await fileBrowserDialogVM.ShowAsync();
                // after file browse dialog is closed, relay the selected filenames
                SelectedFiles = fileBrowserDialogVM.SelectedFiles;
                return result; 
            }));
        }
        #endregion
    }
}
