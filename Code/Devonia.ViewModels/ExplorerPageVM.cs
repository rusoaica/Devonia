/// Written by: Yulia Danilova, Sameh Salem
/// Creation Date: 27th of October, 2021
/// Purpose: View Model for the main application Window
#region ========================================================================= USING =====================================================================================
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Devonia.Infrastructure.Enums;
using System.Collections.ObjectModel;
using Devonia.ViewModels.Common.MVVM;
using Devonia.ViewModels.Common.Models;
#endregion

namespace Devonia.ViewModels
{

    public class ExplorerPageVM : LightBaseModel
    {

        #region ============================================================== FIELD MEMBERS ================================================================================
        public event Action<int> ExplorerPageClosed;
        public event Action<string> FolderBrowsed;
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public int Id { get; set; }

        //public ObservableCollection<FileSystemEntity> SourceItems { get; }
        public List<string> SourceHistory { get; set; } = new List<string>();
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        private string title;
        public string Title
        {
            get { return title; }
            private set { title = value; Notify(); }
        }

        private FileSystemExplorerLayouts layout = FileSystemExplorerLayouts.List;
        public FileSystemExplorerLayouts Layout
        {
            get { return layout; }
            set { layout = value; Notify(); }
        }

        private bool alternatesBackgroundColor = true;
        public bool AlternatesBackgroundColor
        {
            get { return alternatesBackgroundColor; }
            set { alternatesBackgroundColor = value; Notify(); }
        }

        private bool isCtrlPressed;
        public bool IsCtrlPressed
        {
            get { return isCtrlPressed; }
            set { isCtrlPressed = value; Notify(); }
        }

        private bool isShiftPressed;
        public bool IsShiftPressed
        {
            get { return isShiftPressed; }
            set { isShiftPressed = value; Notify(); }
        }

        private string currentPath; 
        public string CurrentPath
        {
            get { return currentPath; }
            set
            {
                currentPath = value;
                SetPageTitle();
                Notify();
            }
        }
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public IAsyncCommand NavigateUp_Command { get; private set; }
        public IAsyncCommand NavigateBack_Command { get; private set; }
        public IAsyncCommand NavigateForward_Command { get; private set; }
        public ISyncCommand CloseExplorerPage_Command { get; private set; }
        public ISyncCommand<string> FolderBrowsed_Command { get; private set; }
        public ISyncCommand<string> LocationChanged_Command { get; private set; }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public ExplorerPageVM()
        {
            CloseExplorerPage_Command = new SyncCommand(CloseExplorerPage);
            FolderBrowsed_Command = new SyncCommand<string>(BrowseFolder);
            LocationChanged_Command = new SyncCommand<string>(LocationChanged);
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Triggers the event that signals that the current file system explorer page can be removed 
        /// </summary>
        private void CloseExplorerPage()
        {
            ExplorerPageClosed?.Invoke(Id);
        }

        /// <summary>
        /// Updates current path and title of file system explorer page when a folder is browsed
        /// </summary>
        /// <param name="path">The new path of the file system explorer page</param>
        private void BrowseFolder(string path)
        {
            // TODO: update navigator location too
            CurrentPath = path;
            FolderBrowsed?.Invoke(path);
        }

        /// <summary>
        /// Updates current path and title of file system explorer page when navigation is done externally
        /// </summary>
        /// <param name="path">The new path of the file system explorer page</param>
        private void LocationChanged(string path)
        {
            currentPath = path;
            SetPageTitle();
        }

        /// <summary>
        /// Sets the title of the file system explorer page
        /// </summary>
        private void SetPageTitle()
        {
            string temp = currentPath;
            if (!string.IsNullOrEmpty(temp))
            {
                // get the last folder of the current path
                if (temp.Contains(Path.DirectorySeparatorChar))
                {
                    if (temp.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        temp = currentPath.Substring(currentPath.LastIndexOf(Path.DirectorySeparatorChar, currentPath.Length - 2, currentPath.Length - 1) + 1);
                        temp = temp.Substring(0, temp.Length - 1);
                    }
                    else
                        temp = currentPath.Substring(currentPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                }
                // check if the current location is a root or drive
                if (Path.IsPathRooted(temp))
                {

                }
            }
            Title = temp;
        }
        #endregion
    }
}