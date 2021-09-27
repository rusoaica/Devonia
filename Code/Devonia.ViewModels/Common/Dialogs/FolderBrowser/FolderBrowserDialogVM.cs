/// Written by: Yulia Danilova
/// Creation Date: 27th of June, 2021
/// Purpose: View Model for the custom folder browser dialog
#region ========================================================================= USING =====================================================================================
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using System.Collections.Generic;
using Devonia.ViewModels.Common.MVVM;
using Devonia.ViewModels.Common.Models;
using System.Collections.ObjectModel;
using Devonia.Infrastructure.Notification;
using Devonia.Models.Common.Models.Common;
using Devonia.Infrastructure.Configuration;
using Devonia.ViewModels.Common.Dispatcher;
using Devonia.ViewModels.Common.ViewFactory;
using Devonia.ViewModels.Common.Controls.NavigationTreeView;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.FolderBrowser
{
    public class FolderBrowserDialogVM : BaseModel, IFolderBrowserDialogVM
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAppConfig appConfig;
        private readonly IViewFactory viewFactory;
        private readonly Stack<string> undoStack = new Stack<string>();
        private readonly Stack<string> redoStack = new Stack<string>();
        #endregion

        #region =============================================================== PROPERTIES ==================================================================================
        private string initialFolder;
        public string InitialFolder
        {
            get { return initialFolder; }
            set
            {
                if (!string.IsNullOrEmpty(value) && Directory.Exists(value))
                     initialFolder = value;
            }
        }
        public bool ShowNewFolderButton { get; set; }
        public bool AllowMultiselection { get; set; }
        public NavigationTreeViewVM NavigationTree { get; set; }
        public List<FileSystemEntity> SelectedItems { get; set; }
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public IAsyncCommand CreateNewFolder_Command { get; private set; }
        public IAsyncCommand NavigateUpAsync_Command { get; private set; }
        public IAsyncCommand ViewOpenedAsync_Command { get; private set; }
        public IAsyncCommand NavigateBackAsync_Command { get; private set; }
        public IAsyncCommand NavigateForwardAsync_Command { get; private set; }
        public IAsyncCommand SetIsFavoritePathAsync_Command { get; private set; }
        public IAsyncCommand SearchDirectoriesKeyUpAsync_Command { get; private set; }
        public IAsyncCommand SearchDirectoriesDropDownClosingAsync_Command { get; private set; }
        public IAsyncCommand<string> TreeSelectedItemChangedAsync_Command { get; private set; }
        public IAsyncCommand<FileSystemEntity> FolderMouseDoubleClickAsync_Command { get; private set; }
        public IAsyncCommand<INavigationTreeViewItem> NavigateToSelectedItemAsync_Command { get; private set; }
        public ISyncCommand ConfirmSelection_Command { get; private set; }
        public ISyncCommand DiscardSelection_Command { get; private set; }
        public ISyncCommand ShowNewFolderDialog_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        public int RootNr
        {
            get { return appConfig.Dialogs.DialogNavigationFilterSelectedIndex; }
            set 
            { 
                // update the navigation treeview 
                NavigationTree.RebuildTree(value, false);
                // if navigation filter is set to drives, expand the navigation treeview to current path
                if (value == 0 && SearchDirectoriesSelectedItem != null)
                    NavigationTree.SetInitialPath(SearchDirectoriesSelectedItem.Value.ToString());
                // update the application's configuration for the selected navigation filter 
                appConfig.Dialogs.DialogNavigationFilterSelectedIndex = value;
                appConfig.UpdateConfigurationAsync();
                Notify();
            }
        }

        private string selectedDirectories;
        public string SelectedDirectories
        {
            get { return selectedDirectories; }
            set { selectedDirectories = value; Notify(); ConfirmSelection_Command.RaiseCanExecuteChanged(); }
        }

        private string newFolderName;
        public string NewFolderName
        {
            get { return newFolderName; }
            set { newFolderName = value; Notify(); CreateNewFolder_Command.RaiseCanExecuteChanged(); }
        }

        private bool? dialogResult = null;
        public bool? DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; Notify(); }
        }

        private bool isFavoritePath;
        public bool IsFavoritePath
        {
            get { return isFavoritePath; }
            set { isFavoritePath = value; Notify(); }
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

        private ObservableCollection<string> sourceNavigationTreeViewFilter = new ObservableCollection<string>();
        public ObservableCollection<string> SourceNavigationTreeViewFilter
        {
            get { return sourceNavigationTreeViewFilter; }
            set { sourceNavigationTreeViewFilter = value; Notify(); }
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
        /// Overload c-tor
        /// </summary>
        /// <param name="viewFactory">The injected abstract factory for creating views</param>
        /// <param name="notificationService">The injected service used for displaying notifications</param>
        /// <param name="dispatcher">The injected dispatcher to use</param>
        /// <param name="appConfig">The injected application's configuration</param>
        public FolderBrowserDialogVM(IViewFactory viewFactory, INotificationService notificationService, IDispatcher dispatcher, IAppConfig appConfig)
        {
            // TODO: fix history navigation when navigating to drives
            NavigateUpAsync_Command = new AsyncCommand(NavigateUpAsync);
            DiscardSelection_Command = new SyncCommand(DiscardSelection);
            ViewOpenedAsync_Command = new AsyncCommand(ViewOpenedAsync);
            NavigateBackAsync_Command = new AsyncCommand(NavigateBackAsync);
            NavigateForwardAsync_Command = new AsyncCommand(NavigateForwardAsync);
            SetIsFavoritePathAsync_Command = new AsyncCommand(SetIsFavoritePathAsync);
            ShowNewFolderDialog_Command = new SyncCommand(()=>{ }, CanShowNewFolderDialog);
            CreateNewFolder_Command = new AsyncCommand(CreateNewFolderAsync, CanCreateNewFolder);
            ConfirmSelection_Command = new SyncCommand(ConfirmSelection, CanConfirmSelection);
            SearchDirectoriesKeyUpAsync_Command = new AsyncCommand(SearchDirectoriesKeyUpAsync);
            TreeSelectedItemChangedAsync_Command = new AsyncCommand<string>(TreeSelectedItemChangedAsync);
            FolderMouseDoubleClickAsync_Command = new AsyncCommand<FileSystemEntity>(FolderMouseDoubleClickAsync);
            SearchDirectoriesDropDownClosingAsync_Command = new AsyncCommand(SearchDirectoriesDropDownClosingAsync);
            NavigateToSelectedItemAsync_Command = new AsyncCommand<INavigationTreeViewItem>(NavigateToSelectedItemAsync);
            NavigationTree = new NavigationTreeViewVM(appConfig, appConfig.Dialogs.DialogNavigationFilterSelectedIndex);
            this.appConfig = appConfig;
            this.dispatcher = dispatcher;
            this.viewFactory = viewFactory;
            this.notificationService = notificationService;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Sets the DialogResult value of the folder browser dialog to True
        /// </summary>
        private void ConfirmSelection()
        {
            DialogResult = true;
            CloseView();
        }

        /// <summary>
        /// Sets the DialogResult value of the folder browser dialog to False
        /// </summary>
        private void DiscardSelection()
        {
            DialogResult = false;
            CloseView();
        }

        /// <summary>
        /// Adds or removes the current path from the favorite paths
        /// </summary>
        private async Task SetIsFavoritePathAsync()
        {
            if (SearchDirectoriesSelectedItem != null && appConfig.Dialogs.FavoritePaths != null)
            {
                // if the current path is marked as favorite and its not already added to the favorite paths in the application's configuration, add it
                if (IsFavoritePath && appConfig.Dialogs.FavoritePaths.Where(e => e == SearchDirectoriesSelectedItem.Value.ToString()).Count() == 0)
                    appConfig.Dialogs.FavoritePaths.Add(SearchDirectoriesSelectedItem.Value.ToString());
                // if the current path is removed as favorite and it is present in the favorite paths in the application's configuration, remove it
                else if (!IsFavoritePath && appConfig.Dialogs.FavoritePaths.Where(e => e == SearchDirectoriesSelectedItem.Value.ToString()).Count() > 0)
                    appConfig.Dialogs.FavoritePaths.Remove(SearchDirectoriesSelectedItem.Value.ToString());
                // if favorite paths are displayed, update the user interface navigation list
                if (RootNr == 1 && NavigationTree.RootNr != RootNr)
                    NavigationTree.RebuildTree(RootNr, false);
                // update the application's configuration
                await appConfig.UpdateConfigurationAsync();
            }
        }
        
        /// <summary>
        /// Checks if the current path is saved in the favorite paths 
        /// </summary>
        /// <returns>True if the current path is saved in the favorite paths, False otherwise</returns>
        private bool GetIsFavoritePath()
        {
            return SearchDirectoriesSelectedItem != null && appConfig.Dialogs.FavoritePaths != null &&
                appConfig.Dialogs.FavoritePaths.Where(e => e == SearchDirectoriesSelectedItem.Value.ToString()).Count() > 0;
        }

        /// <summary>
        /// Creates a new folder in the current directory
        /// </summary>
        private async Task CreateNewFolderAsync()
        {
            try
            {
                // try to create the directory in the current location
                Directory.CreateDirectory(SearchDirectoriesSelectedItem.Value.ToString() +
                    (SearchDirectoriesSelectedItem.Value.ToString().EndsWith(Path.DirectorySeparatorChar.ToString()) ? "" : Path.DirectorySeparatorChar.ToString()) +
                    NewFolderName);
                // refresh the directories list for current directory and the navigation treeview
                await GetFoldersAsync(SearchDirectoriesSelectedItem.Value.ToString());
                NavigationTree.SetInitialPath(SearchDirectoriesSelectedItem.Value.ToString());
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
            }
        }

        /// <summary>
        /// Indicates whether the button for displaying the modal new folder name dialog is enabled or not
        /// </summary>
        /// <returns>True if the current location permits adding a new folder, False otherwise</returns>
        private bool CanShowNewFolderDialog()
        {
            return SearchDirectoriesSelectedItem != null && Directory.Exists(SearchDirectoriesSelectedItem.Value.ToString());
        }

        /// <summary>
        /// Validates if the there is a folder selected or not
        /// </summary>
        /// <returns>True if there is a folder selected, False otherwise</returns>
        public bool CanConfirmSelection()
        {
            bool isValid = !string.IsNullOrEmpty(SelectedDirectories);
            if (!isValid)
            {
                ShowHelpButton();
                WindowHelp = "\n";
                if (string.IsNullOrWhiteSpace(SelectedDirectories))
                    WindowHelp += "You must select at least one directory!\n";
            }
            else
                HideHelpButton();
            return isValid;
        }

        /// <summary>
        /// Indicates whether the button for adding a new folder is enabled or not
        /// </summary>
        /// <returns>True if the provided new folder name is a valid folder name, false otherwise</returns>
        private bool CanCreateNewFolder()
        {
            return !string.IsNullOrEmpty(NewFolderName) && 
                   !NewFolderName.Intersect(Path.GetInvalidPathChars()).Any() && 
                   !NewFolderName.Intersect(Path.GetInvalidFileNameChars()).Any();
        }

        /// <summary>
        /// Navigates to the <paramref name="path"/> when the Enter key is pressed on the navigation's treeview selected item
        /// </summary>
        /// <param name="path">The path to navigate to</param>
        private async Task NavigateToSelectedItemAsync(INavigationTreeViewItem path)
        {
            await GetFoldersAsync(path.FullPathName);
        }

        /// <summary>
        /// Provides forward navigation
        /// </summary>
        private async Task NavigateForwardAsync()
        {
            // check if forward navigation is possible
            if (redoStack.Count > 0)
            {
                // get the first path in the forward navigation list
                string path = redoStack.Pop();
                // put the current path in the backward navigation list, before navigating to the forward path
                if (SearchDirectoriesSelectedItem != null)
                    undoStack.Push(SearchDirectoriesSelectedItem.Value.ToString());
                // navigate to the forward path
                await GetFoldersAsync(path);
            }
        }

        /// <summary>
        /// Provides backwards navigation
        /// </summary>
        private async Task NavigateBackAsync()
        {
            // check if backward navigation is possible
            if (undoStack.Count > 0)
            {
                // get the first path in the backward navigation list
                string path = undoStack.Pop();
                // put the current path in the forward navigationlist, before navigating to the backward path
                if (SearchDirectoriesSelectedItem != null)
                    redoStack.Push(SearchDirectoriesSelectedItem.Value.ToString());
                // navigate to the backward path
                await GetFoldersAsync(path);
            }
        }

        /// <summary>
        /// Navigates up one directory level from the current location, if available
        /// </summary>
        private async Task NavigateUpAsync()
        {
            // check if there is a current path selected
            if (SearchDirectoriesSelectedItem != null && !string.IsNullOrEmpty(SearchDirectoriesSelectedItem.Value.ToString()) && Directory.Exists(SearchDirectoriesSelectedItem.Value.ToString()))
            {
                // check if the current path ends with the directory separator char (it is a directory) or not (it is a drive)
                string path = SearchDirectoriesSelectedItem.Value.ToString();
                // if the current path is a directory, remove the last directory separator character from it
                if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
                // if the modified current path still containes the directory separator character (it is still a directory), get the directories located in it
                if (path.Contains(Path.DirectorySeparatorChar.ToString()))
                {
                    // store the current path in the backwards navigation list
                    undoStack.Push(path);
                    // clear the forward navigation list (forward navigation is no longer possible when navigating up)
                    redoStack.Clear();
                    // navigate to the next upwards directory from the current path
                    await GetFoldersAsync(path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar) + 1));
                }
                else
                {
                    // the modified path doesn't contain the directory separation character, it is a drive
                    SearchDirectoriesSelectedItem = null;
                    await GetDrivesAsync();
                }
            }
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
            // recheck if the current path permits adding new folders
            await dispatcher.DispatchAsync(() => ShowNewFolderDialog_Command.RaiseCanExecuteChanged(), null); 
        }

        /// <summary>
        /// Gets the list of folders located at <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path for which to get the list of folders</param>
        private async Task GetFoldersAsync(string path)
        {
            ShowProgressBar();
            List<FileSystemEntity> allDirectories = new List<FileSystemEntity>();
            await Task.Run(async () =>
            {
                try
                {
                    // get the directories in the provided path
                    foreach (string folder in Directory.EnumerateDirectories(path))
                        allDirectories.Add(new FileSystemEntity() { Path = folder, DirType = 2, IconSource = "folder-empty.png" });
                    // get the search list directories for the current path
                    await GetPathDirectoriesAsync(path);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
                {
                    await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
                }
            });
            SourceDirectories = new ObservableCollection<FileSystemEntity>(allDirectories);
            // update the application's settings last path to the current path
            appConfig.Dialogs.LastDirectory = path;
            await appConfig.UpdateConfigurationAsync();
            HideProgressBar();
        }

        /// <summary>
        /// Gets the list of system's drives
        /// </summary>
        private async Task GetDrivesAsync()
        {
            ShowProgressBar();
            List<FileSystemEntity> drives = new List<FileSystemEntity>();
            await Task.Run(async () =>
            {
                try
                {
                    // get the list of drives of the system
                    foreach (DriveInfo folder in DriveInfo.GetDrives())
                        drives.Add(new FileSystemEntity() { Path = folder.Name, DirType = 1, IconSource = "drive.png" });
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
                {
                    await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
                }
            });
            SourceDirectories = new ObservableCollection<FileSystemEntity>(drives);
            // recheck if the current path permits adding new folders
            await dispatcher.DispatchAsync(() => ShowNewFolderDialog_Command.RaiseCanExecuteChanged(), null);
            HideProgressBar();
        }

        /// <summary>
        /// Navigates to a path selected in the navigation search bar
        /// </summary>
        private async Task NavigateSearchablePathAsync()
        {
            // if the path selected in the navigation search bar is valid, navigate to it, otherwise navigate to the last path in the navigation search list
            if (SearchDirectoriesSelectedItem != null && SearchDirectoriesSelectedItem.Value != null && Directory.Exists(SearchDirectoriesSelectedItem.Value.ToString()))
            {
                // store the current path in the undo list 
                undoStack.Push(SearchDirectoriesSelectedItem.Value.ToString());
                await GetFoldersAsync(SearchDirectoriesSelectedItem.Value.ToString());
            }
            else
                SearchDirectoriesSelectedItem = SourceSearchDirectories[SourceSearchDirectories.Count - 1];
        }

        /// <summary>
        /// Shows a new instance of the folder browser dialog
        /// </summary>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the folder browser dialog</returns>
        public async Task<NotificationResult> ShowAsync()
        {
            // display the folder browser dialog view
            IFolderBrowserDialogView view = viewFactory.CreateView<IFolderBrowserDialogView, IFolderBrowserDialogVM>(this);
            await view.ShowDialogAsync();
            return DialogResult == true ? NotificationResult.OK : NotificationResult.None;
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles the Opened event of the view
        /// </summary>
        private async Task ViewOpenedAsync()
        {
            // get the navigation treeview filters
            SourceNavigationTreeViewFilter = new ObservableCollection<string>(NavigationTreeViewRootItemUtils.ListNavigationTreeViewRootItemsByConvention());
            RootNr = appConfig.Dialogs.DialogNavigationFilterSelectedIndex;
            NavigationTree.RootNr = appConfig.Dialogs.DialogNavigationFilterSelectedIndex;
            if (!string.IsNullOrEmpty(InitialFolder) && Directory.Exists(InitialFolder))
            {
                // if an initial path is provided externally, navigate to it
                await GetFoldersAsync(InitialFolder);
                // expand the navigation treeview to the current path
                NavigationTree.SetInitialPath(InitialFolder);
            }
            WindowTitle = "Devonia - Open Folder";
        }

        /// <summary>
        /// Handles the KeyUp event of the search directories autocomplete box
        /// </summary>
        private async Task SearchDirectoriesKeyUpAsync()
        {
            await NavigateSearchablePathAsync();
        }

        /// <summary>
        /// Handles DropDownClosing event of the search directories autocomplete box
        /// </summary>
        private async Task SearchDirectoriesDropDownClosingAsync()
        {
            await NavigateSearchablePathAsync();
        }

        /// <summary>
        /// Handles MouseDoubleClick event of the folders inside the folders listview
        /// </summary>
        /// <param name="folder">The folder that initiated the double click event</param>
        private async Task FolderMouseDoubleClickAsync(FileSystemEntity folder)
        {
            await GetFoldersAsync(folder.Path);
        }

        /// <summary>
        /// Handles SelectedItemChanged event for the navigation treeview
        /// </summary>
        /// <param name="path">The path of the newly selected item</param>
        private async Task TreeSelectedItemChangedAsync(string path)
        {
            if (Directory.Exists(path))
            { 
                // store the current path in the backwards navigation list, before navigating to the new path
                if (SearchDirectoriesSelectedItem != null)
                    undoStack.Push(SearchDirectoriesSelectedItem.Value.ToString());
                // clear forward navigation list, it is only relevant when backward navigating
                redoStack.Clear();
                // navigate to the new path
                await GetFoldersAsync(path);
            }
        }
        #endregion
    }
}
