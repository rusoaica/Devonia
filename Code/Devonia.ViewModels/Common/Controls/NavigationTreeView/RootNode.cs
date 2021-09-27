/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Dummy root node
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.Collections.ObjectModel;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public class RootNode : NavigationTreeViewItem
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Does nothing, used only as dummy object
        /// </summary>
        /// <returns>Null</returns>
        public override string GetMyIcon()
        {
            return icon = null;
        }

        /// <summary>
        /// Does nothing, used only as dummy object
        /// </summary>
        /// <returns>Am empty collection</returns>
        public override ObservableCollection<INavigationTreeViewItem> GetMyChildren()
        {
            return new ObservableCollection<INavigationTreeViewItem>();
        }
        #endregion
    }
}
