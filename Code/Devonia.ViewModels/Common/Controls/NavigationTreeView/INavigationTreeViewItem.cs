/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Interface for navigation tree view items
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public interface INavigationTreeViewItem : INotifyPropertyChanged
    {
        #region =============================================================== PROPERTIES ==================================================================================
        string Icon { get; set; }
        string FriendlyName { get; set; }
        string FullPathName { get; set; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        bool IncludeFileChildren { get; set; }
        List<string> Filter { get; set; }
        ObservableCollection<INavigationTreeViewItem> Children { get; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Deletes the children nodes of a node
        /// </summary>
        void DeleteChildren();

        /// <summary>
        /// Notifies the UI about a binded property's value being changed
        /// </summary>
        /// <param name="propertyName">The property that had the value changed</param>
        void Notify([CallerMemberName] string propertyName = null);
        #endregion
    }
}
