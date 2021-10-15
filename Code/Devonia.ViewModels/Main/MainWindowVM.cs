/// Written by: Yulia Danilova
/// Creation Date: 11th of November, 2020
/// Purpose: View Model for the main application Window
#region ========================================================================= USING =====================================================================================
using Devonia.Infrastructure.Enums;
using Devonia.Infrastructure.Notification;
using Devonia.Models.Common.Models.Common;
using Devonia.Models.Core.Options;
using Devonia.ViewModels.Common.Models;
using Devonia.ViewModels.Common.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Devonia.Infrastructure.Configuration;

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
        private readonly IAppConfig appConfig;

        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public IAsyncCommand ViewOpenedAsync_Command { get; private set; }
        public AutoCompleteFilterPredicate<object> SearchMediaLibrary_Command { get; private set; }
        public IAsyncCommand SelectedExtensionChangedAsync_Command { get; private set; }
        public IAsyncCommand CreateNewFolder_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        private string version = "1.2.6.1";
        public string Version
        {
            get { return version; }
            set { version = value; Notify(); }
        }

        private string currentPath = "/mnt/STORAGE/MULTIMEDIA/MUSIC/";
        public string CurrentPath
        {
            get { return currentPath; }
            set
            {
                currentPath = value; Notify();
            }
        }

        private bool isFavoritePath;
        public bool IsFavoritePath
        {
            get { return isFavoritePath; }
            set { isFavoritePath = value; Notify(); }
        }


        private SearchEntity selectedExtensionFilter;
        public SearchEntity SelectedExtensionFilter
        {
            get { return selectedExtensionFilter; }
            set { selectedExtensionFilter = value; Notify(); }
        }

        private ObservableCollection<FileSystemEntity> sourceDirectories = new ObservableCollection<FileSystemEntity>();
        public ObservableCollection<FileSystemEntity> SourceDirectories
        {
            get { return sourceDirectories; }
            set { sourceDirectories = value; Notify(); }
        }
        
        
        private ObservableCollection<SearchEntity> sourceSearchDirectories = new ObservableCollection<SearchEntity>();
        public ObservableCollection<SearchEntity> SourceSearchDirectories
        {
            get { return sourceSearchDirectories; }
            set { sourceSearchDirectories = value; Notify(); }
        }

        private SearchEntity searchDirectoriesSelectedItem;
        public SearchEntity SearchDirectoriesSelectedItem
        {
            get { return searchDirectoriesSelectedItem; }
            set { searchDirectoriesSelectedItem = value; Notify(); }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="appOptions">Injected application options</param>
        /// <param name="notificationService">Injected notification service</param>
        public MainWindowVM(IAppOptions appOptions, INotificationService notificationService, IAppConfig appConfig)
        {
            this.appOptions = appOptions;
            this.appConfig = appConfig;
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
        /// Checks if the current path is saved in the favorite paths 
        /// </summary>
        /// <returns>True if the current path is saved in the favorite paths, False otherwise</returns>
        private bool GetIsFavoritePath()
        {
            return SearchDirectoriesSelectedItem != null && appConfig.Explorer.FavoritePaths != null &&
                   appConfig.Explorer.FavoritePaths.Where(e => e == SearchDirectoriesSelectedItem.Value.ToString()).Count() > 0;
        }
        
        /// <summary>
        /// Gets the name of all directories in <paramref name="path"/> and adds them to the directories searcheable autocomplete box
        /// </summary>
        /// <param name="path">The path for which to get all directories</param>
        private async Task GetPathDirectoriesAsync(string path)
        {
            List<SearchEntity> temp = new List<SearchEntity>();
            // get a list of all subdirectories of the provided path argument
            await Task.Run(() =>
            {
                foreach (string directory in path.Split(Path.DirectorySeparatorChar))
                    if (!string.IsNullOrEmpty(directory))
                        temp.Add(new SearchEntity() { Text = directory, Value = path.Substring(0, path.IndexOf(directory) + directory.Length) + Path.DirectorySeparatorChar });
            });
            SourceSearchDirectories = new ObservableCollection<SearchEntity>(temp);
            // set the selected search directory to the last item in the list, which should be similar to the current path
            if (SourceSearchDirectories.Count > 0)
                SearchDirectoriesSelectedItem = SourceSearchDirectories[SourceSearchDirectories.Count - 1];
            if (SearchDirectoriesSelectedItem == null)
                throw new Exception("Selected search directory cannot be null!");
            // check if the current path is in the list of favorite paths
            IsFavoritePath = GetIsFavoritePath();
            // re-check if the current path permits adding new folders
            //await dispatcher.DispatchAsync(() => ShowNewFolderDialog_Command.RaiseCanExecuteChanged(), null);
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
            //HideProgressBar();
        }
        #endregion
    }
}
