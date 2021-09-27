/// Written by: Yulia Danilova
/// Creation Date: 11th of November, 2020
/// Purpose: View Model for the main application Window
#region ========================================================================= USING =====================================================================================
using Devonia.Infrastructure.Enums;
using Devonia.Infrastructure.Notification;
using Devonia.Models.Common.Models.Common;
using Devonia.Models.Core.Options;
using Devonia.ViewModels.Common.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Devonia.ViewModels.Main
{
    public class MainWindowVM : BaseModel, IMainWindowVM
    {

        #region ============================================================== FIELD MEMBERS ================================================================================
        public event Action Navigated;
        public event Action<bool> ValidationChanged;
        public delegate bool AutoCompleteFilterPredicate<T>(string search, T item);

        private readonly IAppOptions appOptions;
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public IAsyncCommand ViewOpenedAsync_Command { get; private set; }
        public AutoCompleteFilterPredicate<object> SearchMediaLibrary_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        private string version = "1.2.6.1";
        public string Version
        {
            get { return version; }
            set { version = value; Notify(); }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="appOptions">Injected application options</param>
        /// <param name="notificationService">Injected notification service</param>
        public MainWindowVM(IAppOptions appOptions, INotificationService notificationService)
        {
            this.appOptions = appOptions;
            this.notificationService = notificationService;

            ViewOpenedAsync_Command = new AsyncCommand(ViewOpenedAsync);
            SearchMediaLibrary_Command = new AutoCompleteFilterPredicate<object>(SearchMediaLibrary);
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Handles the event risen when a domain model property changes its value and needs to notify the outside world of this change.
        /// Used in MVVM to trigger an update of the bounded properties, taking their values from the domain models corresponding properties
        /// </summary>
        /// <param name="propertyName">The name of the domain model property whose value changed</param>
        private void DomainModelPropertyChanged(string propertyName)
        {
            Notify(propertyName);
        }

        /// <summary>
        /// Custom filter for the media library searching
        /// </summary>
        /// <param name="search">The search term</param>
        /// <param name="item">The item to be searched for <paramref name="search"/></param>
        /// <returns>True if <paramref name="item"/> contains <paramref name="search"/>, False otherwise</returns>
        private bool SearchMediaLibrary(string search, object item)
        {
            SearchEntity element = item as SearchEntity;
            // ignore casing
            search = search.ToLower().Trim();
            // check if at least one search criteria is met
            bool hasEpisode = element.Text.ToLower().Contains(search);
            bool hasTvShow = (element.Value?.ToString().ToLower().Contains(search) ?? false);
            bool hasTags = element.Tags?.Where(t => t.ToLower().Contains(search)).Count() > 0;
            bool hasGenres = element?.Genres.Where(g => g.ToLower().Contains(search)).Count() > 0;
            bool hasActors = element?.Actors.Where(a => a.ToLower().Contains(search)).Count() > 0;
            bool hasRoles = (element?.Hover as string[]).Where(r => r.ToLower().Contains(search)).Count() > 0;
            return hasEpisode || hasTvShow || hasTags || hasGenres || hasActors || hasRoles;
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles the ViewOpenedAsync event of the Window
        /// </summary>
        private async Task ViewOpenedAsync()
        {
            // get the current version of the software
            Version = "v. " + FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion;
            ShowProgressBar();
            WindowTitle = "Devonia";
            IsMediaPlayingIndicatorSocketVisible = true;
            HideProgressBar();
        }
        #endregion
    }
}
