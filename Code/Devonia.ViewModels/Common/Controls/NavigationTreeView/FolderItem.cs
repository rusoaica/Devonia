/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Navigation tree view item for folders
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public class FolderItem : NavigationTreeViewItem
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the icon of the node represented by this class
        /// </summary>
        /// <returns>A string representing the file name of the icon</returns>
        public override string GetMyIcon()
        {
            // TODO: maybe add special icons for special folders? (Desktop, Downloads, etc)
            return "folder-empty.png";
        }

        /// <summary>
        /// Gets the children nodes of the node represented by this class
        /// </summary>
        /// <returns>A collection of nodes representing the child nodes of the node represented by this class</returns>
        public override ObservableCollection<INavigationTreeViewItem> GetMyChildren()
        {
            ObservableCollection<INavigationTreeViewItem> childrenList = new ObservableCollection<INavigationTreeViewItem>() { };
            INavigationTreeViewItem child;
            try
            {
                DirectoryInfo di = new DirectoryInfo(FullPathName); // access may not be allowed!
                if (!di.Exists) return childrenList;
                // get the subdirectories of the drive and add them as childred
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    child = new FolderItem();
                    child.FullPathName = FullPathName + "\\" + dir.Name;
                    child.FriendlyName = dir.Name;
                    child.IncludeFileChildren = this.IncludeFileChildren;
                    child.Filter = Filter;
                    childrenList.Add(child);
                }
                // if files are to be included and the drive root directory contains files too, include them
                if (IncludeFileChildren)
                {
                    foreach (FileInfo file in di.GetFiles())
                    {
                        string extension = Path.GetExtension(file.Name).ToLower();
                        // if a filter was provided, only add files whose extenstions exist in the filter list, otherwise, add all files
                        if ((Filter != null && !string.IsNullOrEmpty(extension) && Filter.Where(e => e.ToLower() == extension).Count() > 0) || Filter == null)
                        {
                            child = new FileItem();
                            child.FullPathName = FullPathName + "\\" + file.Name;
                            child.FriendlyName = file.Name;
                            childrenList.Add(child);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return childrenList;
        }
        #endregion
    }
}
