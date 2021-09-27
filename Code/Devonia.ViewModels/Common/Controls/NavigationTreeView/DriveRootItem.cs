/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Navigation tree view item for root drives with their contents
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.IO;
using System.Collections.ObjectModel;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public class DriveRootItem : NavigationTreeViewItem
    {
        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default c-tor
        /// </summary>
        public DriveRootItem()
        {
            FriendlyName = "DriveRoot";
            IsExpanded = true;
            FullPathName = "$xxDriveRoot$";
        }
        #endregion

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
            ObservableCollection<INavigationTreeViewItem> childrenList = new ObservableCollection<INavigationTreeViewItem>() { };
            INavigationTreeViewItem child;
            string friendlyName = string.Empty;
            // get the properties of all the available drives and if they are accessible, add them to the child list
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    child = new DriveItem();
                    // some processing for the FriendlyName
                    friendlyName = drive.Name.Replace(@"\", "");
                    child.FullPathName = friendlyName;
                    if (drive.VolumeLabel == string.Empty)
                        friendlyName = drive.DriveType.ToString() + " (" + friendlyName + ")";
                    else if (drive.DriveType == DriveType.CDRom)
                        friendlyName = drive.DriveType.ToString() + " " + drive.VolumeLabel + " (" + friendlyName + ")";
                    else
                        friendlyName = drive.VolumeLabel + " (" + friendlyName + ")";
                    child.FriendlyName = friendlyName;
                    child.IncludeFileChildren = this.IncludeFileChildren;
                    child.Filter = Filter;
                    childrenList.Add(child);
                }
            }
            return childrenList;
        }
        #endregion
    }
}
