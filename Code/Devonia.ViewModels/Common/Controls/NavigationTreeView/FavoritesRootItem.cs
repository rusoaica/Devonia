/// Written by: Yulia Danilova
/// Creation Date: 03rd of July, 2021
/// Purpose: Navigation tree view item for favorite folders
#region ========================================================================= USING =====================================================================================
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Devonia.Infrastructure.Configuration;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public class FavoritesRootItem : NavigationTreeViewItem
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAppConfig appConfig;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload c-tor
        /// <param name="appConfig">The injected application's configuration</param>
        /// <param name="includeFileChildren">Whether to include file children items or not</param>
        /// <param name="filter">Filter for filtering root item types</param>
        /// </summary>
        public FavoritesRootItem(IAppConfig appConfig, bool includeFileChildren = false, List<string> filter = null)
        {
            FriendlyName = "FavoritesRoot";
            FullPathName = "$xxFavoritesRoot$";
            this.appConfig = appConfig;
            IncludeFileChildren = includeFileChildren;
            Filter = filter;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the icon of the node represented by this class
        /// </summary>
        /// <returns>A string representing the file name of the icon</returns>
        public override string GetMyIcon()
        {
            // TODO: add special folder icons
            return "folder-empty.png";
        }

        /// <summary>
        /// Gets the children nodes of the node represented by this class
        /// </summary>
        /// <returns>A collection of nodes representing the child nodes of the node represented by this class</returns>
        public override ObservableCollection<INavigationTreeViewItem> GetMyChildren()
        {
            ObservableCollection<INavigationTreeViewItem> childrenList = new ObservableCollection<INavigationTreeViewItem>();
            INavigationTreeViewItem child;
            foreach (string specialFolder in appConfig.Explorer.FavoritePaths)
            {
                    child = new FolderItem();
                    child.FullPathName = specialFolder;
                    child.FriendlyName = specialFolder;
                    child.IncludeFileChildren = true;
                    child.Filter = Filter;
                    childrenList.Add(child);
            }
            return childrenList;
        }
        #endregion
    }
}
