/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Navigation tree view item for files
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.Collections.ObjectModel;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public class FileItem : NavigationTreeViewItem
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the icon of the node represented by this class
        /// </summary>
        /// <returns>A string representing the file name of the icon</returns>
        public override string GetMyIcon()
        {
            // TODO: add different icons for different file types
            return "file.png";
        }

        /// <summary>
        /// Does nothing, files have no children
        /// </summary>
        /// <returns>An empty list</returns>
        public override ObservableCollection<INavigationTreeViewItem> GetMyChildren()
        {
            return new ObservableCollection<INavigationTreeViewItem>();
        }
        #endregion
    }
}
