/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Abstract flass for navigation tree view items
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public abstract class NavigationTreeViewItem : INavigationTreeViewItem
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region =============================================================== PROPERTIES ==================================================================================
        public string FriendlyName { get; set; }
        public string FullPathName { get; set; }
        public bool IncludeFileChildren { get; set; }
        public List<string> Filter { get; set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        protected string icon;
        public string Icon
        {
            get { return icon ?? (icon = GetMyIcon()); }
            set { icon = value; Notify(); }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; Notify(); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; Notify(); }
        }

        protected ObservableCollection<INavigationTreeViewItem> children;
        public ObservableCollection<INavigationTreeViewItem> Children
        {
            get { return children ?? (children = GetMyChildren()); }
            set { children = value; Notify(); }
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Abstract method for getting the icon of the node represented by derived classes
        /// </summary>
        /// <returns>A string representing the file name of the icon</returns>
        public abstract string GetMyIcon();

        /// <summary>
        /// Abstract method for getting the children nodes of the node represented by derived classes
        /// </summary>
        /// <returns>A collection of nodes representing the child nodes of the node represented by derived classes</returns>
        public abstract ObservableCollection<INavigationTreeViewItem> GetMyChildren();

        /// <summary>
        /// Notifies the UI about a binded property's value being changed
        /// </summary>
        /// <param name="propertyName">The property that had the value changed</param>
        public void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Removes old trees or sets <see cref="children"/> to null, so a new tree is built
        /// </summary>
        public void DeleteChildren()
        {
            if (children != null)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    children[i].DeleteChildren();
                    //children[i] = null;
                    children.RemoveAt(i);
                }
                children = null;
            }
        }
        #endregion
    }
}
