/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Utility class for navigation tree view items
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public static class NavigationTreeViewUtils
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public static readonly string separator = "[+]";
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Visits <paramref name="selectedNode"/> and takes a snapshot
        /// </summary>
        /// <param name="selectedNode">The node to visit</param>
        /// <param name="currentName">The path of the node to visit</param>
        /// <param name="snapShot">The snapshot to take for <paramref name="selectedNode"/></param>
        private static void VisitExpandedChildrenAndTakeSnapshot(INavigationTreeViewItem selectedNode, string currentName, ref List<string> snapShot)
        {
            // If node not expanded we do not refresh/repaint rest of the nodes
            if (selectedNode.IsExpanded)
            {
                string newCurrentName = (currentName == separator) ? selectedNode.FullPathName : currentName + separator + selectedNode.FullPathName;
                snapShot.Add(newCurrentName);
                for (int i = 0; i < selectedNode.Children.Count; i++)
                    VisitExpandedChildrenAndTakeSnapshot(selectedNode.Children[i], newCurrentName, ref snapShot);
            }
        }

        /// <summary>
        /// Creates a snapshot of <paramref name="rootChildren"/>
        /// </summary>
        /// <param name="rootChildren">The nodes for which to take a snapshot</param>
        /// <returns>A list containing the snapshot</returns>
        public static List<string> TakeSnapshot(ObservableCollection<INavigationTreeViewItem> rootChildren)
        {
            List<string> snapShot = new List<string>();
            // use a dummy rootnode, is easier to work with 
            RootNode rootNode = new RootNode();
            foreach (INavigationTreeViewItem item in rootChildren) 
                rootNode.Children.Add(item);
            // take snapshot of all expanded nodes
            // new: for handling all kinds of namespaces we take as snapshot concatenation of consecutive [Fullnames+separator]
            // for a hierachical namespace currentName+separator not needed
            rootNode.IsExpanded = true;
            VisitExpandedChildrenAndTakeSnapshot(rootNode, string.Empty, ref snapShot);
            return snapShot;
        }

        /// <summary>
        /// Gets a node from <paramref name="pathArray"/>
        /// </summary>
        /// <param name="item">The item to get</param>
        /// <param name="pathArray">The path array from which to get <paramref name="item"/></param>
        /// <param name="level">The index at which <paramref name="item"/> is located at inside <paramref name="pathArray"/></param>
        private static void GetNodeFromNameLocal(ref INavigationTreeViewItem item, string[] pathArray, int level)
        {
            string name = pathArray[level];
            INavigationTreeViewItem child;
            INavigationTreeViewItem selected = null;
            for (int i = 0; (i <= item.Children.Count() - 1) && (selected == null); i++)
            {
                child = item.Children[i];
                if (name == child.FullPathName) 
                    selected = child;
            }
            item = selected;
            // if we have a hit, step deeper
            level++;
            if ((level <= pathArray.Length - 1) && (item != null)) 
                GetNodeFromNameLocal(ref item, pathArray, level);
        }

        /// <summary>
        /// Gets a node from <paramref name="rootNode"/>
        /// </summary>
        /// <param name="rootNode">The root node from which to take the node</param>
        /// <param name="fullPathNames">The full path of the node</param>
        /// <param name="selectedNode">The node to get from <paramref name="rootNode"/></param>
        private static void GetNodeFromName(INavigationTreeViewItem rootNode, string fullPathNames, ref INavigationTreeViewItem selectedNode)
        {
            // setup a call to GetNodeFromNameLocal to do the work
            // note: to copy or not to copy (pointer, content), all seems ok
            selectedNode = null;
            if (string.IsNullOrEmpty(fullPathNames))
                return;           
            // make a pathArray 
            // note: now it is not anymore [(drive) (folder)], but [(drive) [(drive) (folder)]]   
            string[] separator = new string[] { NavigationTreeViewUtils.separator };
            string[] pathArray = fullPathNames.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (pathArray.Length == 0)  
                return; 
            // get the node holding the Items
            selectedNode = rootNode;
            GetNodeFromNameLocal(ref selectedNode, pathArray, 0);
        }

        /// <summary>
        /// Expands <paramref name="treeRootItem"/> inside <paramref name="snapShot"/>
        /// </summary>
        /// <param name="snapShot">The snapshot containing the item to expand</param>
        /// <param name="treeRootItem">The item to be expanded</param>
        public static void ExpandSnapShotItems(List<string> snapShot, INavigationTreeViewItem treeRootItem)
        {
            // try to open all old snapshot nodes
            INavigationTreeViewItem selected = null;
            for (int i = 0; i < snapShot.Count; i++)
            {
                GetNodeFromName(treeRootItem, snapShot[i], ref selected);
                if (selected != null)
                {
                    selected.IsExpanded = true;
                    selected.IsSelected = true;
                }
            }
        }
        #endregion
    }
}
