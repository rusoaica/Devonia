/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Child navigation tree view item for root drives with their contents
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public class DriveItem : NavigationTreeViewItem
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the icon of the node represented by this class
        /// </summary>
        /// <returns>A string representing the file name of the icon</returns>
        public override string GetMyIcon()
        {
            return "drive.png";
        }

        /// <summary>
        /// Gets the children nodes of the node represented by this class
        /// </summary>
        /// <returns>A collection of nodes representing the child nodes of the node represented by this class</returns>
        public override ObservableCollection<INavigationTreeViewItem> GetMyChildren()
        {
            ObservableCollection<INavigationTreeViewItem> childrenList = new ObservableCollection<INavigationTreeViewItem>();
            INavigationTreeViewItem child;
            // get the properties of the drive represented by this class
            DriveInfo drive = new DriveInfo(FullPathName);
            // if drive is inaccessible, return empty child list
            if (!drive.IsReady) 
                return childrenList;
            // check if the drive has a root directory, and if it doesn't return an empty child list
            DirectoryInfo di = new DirectoryInfo(drive.RootDirectory.Name);
            if (!di.Exists) 
                return childrenList;
            // get the subdirectories of the drive and add them as childred
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                child = new FolderItem();
                child.FullPathName = FullPathName + "\\" + dir.Name;
                child.FriendlyName = dir.Name;
                child.IncludeFileChildren = IncludeFileChildren;
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
                        child.Filter = Filter;
                        childrenList.Add(child);
                    }
                }
            }
            return childrenList;
        }
        #endregion
    }
}
