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
        public event Action<string> FolderBrowsed;
     
        public delegate bool AutoCompleteFilterPredicate<T>(string search, T item);

        private readonly IAppOptions appOptions;
        private readonly IAppConfig appConfig;

        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public IAsyncCommand ViewOpenedAsync_Command { get; private set; }
        public AutoCompleteFilterPredicate<object> SearchMediaLibrary_Command { get; private set; }
        public IAsyncCommand SelectedExtensionChangedAsync_Command { get; private set; }
        public IAsyncCommand CreateNewFolder_Command { get; private set; }
        public ISyncCommand SetControlSpecialMode_Command { get; private set; }
        public ISyncCommand SetShiftSpecialMode_Command { get; private set; }
        public ISyncCommand<FileSystemExplorerLayouts> ChangeExplorerLayout_Command { get; private set; }
        public ISyncCommand SelectedPageChanged_Command { get; private set; }
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
        
        
        
        
        ObservableCollection<ExplorerPageVM> sourceExplorerPages = new ObservableCollection<ExplorerPageVM>();
        public ObservableCollection<ExplorerPageVM> SourceExplorerPages
        {
            get { return sourceExplorerPages; }
            set { sourceExplorerPages = value; Notify(); }
        }

        private ExplorerPageVM selectedExplorerPage;
        public ExplorerPageVM SelectedExplorerPage
        {
            get { return selectedExplorerPage; }
            set { selectedExplorerPage = value; Notify(); }
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
            SetControlSpecialMode_Command = new SyncCommand(SetControlSpecialMode);
            SetShiftSpecialMode_Command = new SyncCommand(SetShiftSpecialMode);
            ChangeExplorerLayout_Command = new SyncCommand<FileSystemExplorerLayouts>(ChangeExplorerLayout);
            SelectedPageChanged_Command = new SyncCommand(SelectedPageChanged);
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

        /// <summary>
        /// Signals the currently selected explorer page that the control key is pressed 
        /// </summary>
        public void SetControlSpecialMode()
        {
            // set all explorer pages control key pressed status to false  
            foreach (ExplorerPageVM page in sourceExplorerPages)
                page.IsCtrlPressed = false;
            // set control key pressed status only for the currently selected explorer page 
            if (selectedExplorerPage != null)
                selectedExplorerPage.IsCtrlPressed = true;
        }
        
        /// <summary>
        /// Signals the currently selected explorer page that the shift key is pressed 
        /// </summary>
        public void SetShiftSpecialMode()
        {
            // set all explorer pages shift key pressed status to false  
            foreach (ExplorerPageVM page in sourceExplorerPages)
                page.IsShiftPressed = false;
            // set shift key pressed status only for the currently selected explorer page 
            if (selectedExplorerPage != null)
                selectedExplorerPage.IsShiftPressed = true;
        }
        
        /// <summary>
        /// Resets the shift and control keys status for the currently selected explorer page to false 
        /// </summary>
        public void ResetSpecialModes()
        {
            if (selectedExplorerPage != null)
            {
                selectedExplorerPage.IsShiftPressed = false;
                selectedExplorerPage.IsCtrlPressed = false;
            }
        }

        /// <summary>
        /// Changes the display of the currently selected file system explorer page to <paramref name="layout"/>
        /// </summary>
        /// <param name="layout">The layout to apply to the currently selected file system explorer page</param>
        private void ChangeExplorerLayout(FileSystemExplorerLayouts layout)
        {
            ShowProgressBar();
            if (selectedExplorerPage != null)
                selectedExplorerPage.Layout = layout;
            HideProgressBar();
        }

        public void NavigateToPath(string path)
        {
            ShowProgressBar();
            if (selectedExplorerPage != null && selectedExplorerPage.CurrentPath != path)
                selectedExplorerPage.CurrentPath = path;
            HideProgressBar();
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
            ExplorerPageVM explorer_one = new ExplorerPageVM();
            explorer_one.Id = 1;
            explorer_one.ExplorerPageClosed += OnExplorerPageClosed;
            explorer_one.CurrentPath = Directory.Exists("/mnt/STORAGE/MULTIMEDIA/MUSIC/") ? "/mnt/STORAGE/MULTIMEDIA/MUSIC/" : @"Z:\MULTIMEDIA\MUSIC\";
            explorer_one.FolderBrowsed += OnFolderBrowsed;
            SourceExplorerPages.Add(explorer_one);
        
            ExplorerPageVM explorer_two = new ExplorerPageVM();
            explorer_two.CurrentPath = Directory.Exists("/mnt/STORAGE/MULTIMEDIA/MUSIC/") ? "/mnt/STORAGE/MULTIMEDIA/" : @"Z:\MULTIMEDIA\";
            explorer_two.Id = 2;
            explorer_two.ExplorerPageClosed += OnExplorerPageClosed;
            explorer_two.FolderBrowsed += OnFolderBrowsed;
            SourceExplorerPages.Add(explorer_two);
           
            ExplorerPageVM explorer_three = new ExplorerPageVM();
            explorer_three.CurrentPath = Directory.Exists("/mnt/STORAGE/MULTIMEDIA/ANIME/") ? "/mnt/STORAGE/MULTIMEDIA/ANIME/" : @"Z:\MULTIMEDIA\ANIME\";
            explorer_three.Id = 3;
            explorer_three.ExplorerPageClosed += OnExplorerPageClosed;
            explorer_three.FolderBrowsed += OnFolderBrowsed;
            SourceExplorerPages.Add(explorer_three);
            HideProgressBar();
        }

        private void OnFolderBrowsed(string path)
        {
            FolderBrowsed?.Invoke(path);
        }

        /// <summary>
        /// Closes an explorer page identified by <paramref name="pageId"/>
        /// </summary>
        /// <param name="pageId">The id of the explorer page to be closed</param>
        private void OnExplorerPageClosed(int pageId)
        {
            // do not close the page if its the only one left
            if (sourceExplorerPages.Count > 1)
            {
                // get the page that initiated the close request
                ExplorerPageVM pageToBeClosed = sourceExplorerPages.Where(page => page.Id == pageId)
                                                                   .First();
                // unsubscribe selected page's events, to avoid memory leaks
                pageToBeClosed.ExplorerPageClosed -= OnExplorerPageClosed;
                pageToBeClosed.FolderBrowsed -= OnFolderBrowsed;
                // remove selected page and notify the UI
                SourceExplorerPages.Remove(pageToBeClosed);
                Notify(nameof(SourceExplorerPages));
            }
        }

        private void SelectedPageChanged()
        {
            
        }
        #endregion
    }
}
