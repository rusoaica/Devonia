/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: View Model for the custom file and folder browser navigation treeview 
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.IO;
using System.Collections.Generic;
using Devonia.ViewModels.Common.MVVM;
using System.Collections.ObjectModel;
using Devonia.Infrastructure.Configuration;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public class NavigationTreeViewVM : LightBaseModel
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAppConfig appConfig;
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        private string treeName = string.Empty;
        public string TreeName
        {
            get { return treeName; }
            set { treeName = value; Notify(); }
        }

        private int rootNr;
        public int RootNr
        {
            get { return rootNr; }
            set { rootNr = value; Notify(); }
        }

        private ObservableCollection<INavigationTreeViewItem> rootChildren = new ObservableCollection<INavigationTreeViewItem>();
        public ObservableCollection<INavigationTreeViewItem> RootChildren
        {
            get { return rootChildren; }
            set { rootChildren = value; Notify(); }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="appConfig">The injected application's configuration</param>
        /// <param name="rootNumber">The number of the root to start with</param>
        /// <param name="includeFileChildren">Whether to include file items or not</param>
        /// <param name="filter">Filter for filtering items</param>
        public NavigationTreeViewVM(IAppConfig appConfig, int rootNumber = 0, bool includeFileChildren = false, List<string> filter = null)
        {
            this.appConfig = appConfig;
            // create a new RootItem given rootNumber using convention
            RootNr = rootNumber;
            NavigationTreeViewItem treeRootItem = NavigationTreeViewRootItemUtils.ReturnRootItem(appConfig, rootNumber, includeFileChildren, filter);
            TreeName = treeRootItem.FriendlyName;
            // delete RootChildren and init RootChildren using treeRootItem.Children
            foreach (INavigationTreeViewItem item in RootChildren)
                item.DeleteChildren();
            RootChildren.Clear();
            //foreach (INavigationTreeViewItem item in treeRootItem.Children)
            //    RootChildren.Add(item);
            RootChildren = new ObservableCollection<INavigationTreeViewItem>(treeRootItem.Children);
        }

        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="rootNumber">The number of the root to start with</param>
        /// <param name="appConfig">The injected application's configuration</param>
        public NavigationTreeViewVM(IAppConfig appConfig, int rootNumber) : this(appConfig, rootNumber, false)
        {
            this.appConfig = appConfig;
        }

        /// <summary>
        /// Overload c-tor
        /// </summary>
        /// <param name="appConfig">The injected application's configuration</param>
        public NavigationTreeViewVM(IAppConfig appConfig) : this(appConfig, 0)
        {
            this.appConfig = appConfig;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Rebuilds a tree
        /// </summary>
        /// <param name="rootNumber">The root number to rebuild</param>
        /// <param name="includeFileChildren">Whether to include files or not</param>
        /// <param name="filter">Optional filter for file types</param>
        public void RebuildTree(int rootNumber = -1, bool includeFileChildren = false, List<string> filter = null)
        {
            // first, take snapshot of current expanded items
            List<string> snapshot = NavigationTreeViewUtils.TakeSnapshot(rootChildren);
            // delete all rootChildren
            foreach (INavigationTreeViewItem children in rootChildren)
                children.DeleteChildren();
            rootChildren.Clear();
            // create treeRootItem 
            if (rootNumber != -1)
                RootNr = rootNumber;
            NavigationTreeViewItem treeRootItem = NavigationTreeViewRootItemUtils.ReturnRootItem(appConfig, RootNr, includeFileChildren, filter);
            if (rootNumber != -1)
                TreeName = treeRootItem.FriendlyName;
            // copy children treeRootItem to RootChildren, set up new tree 
            RootChildren = new ObservableCollection<INavigationTreeViewItem>(treeRootItem.Children);
            // expand previous snapshot
            NavigationTreeViewUtils.ExpandSnapShotItems(snapshot, treeRootItem);
        }

        /// <summary>
        /// Sets the initial path in the navigation treeview
        /// </summary>
        /// <param name="path">The initial path to set</param>
        /// <param name="includeFiles">Whether to include files or not</param>
        /// <param name="filter">Optional filter for file types</param>
        public void SetInitialPath(string path, bool includeFiles = false, List<string> filter = null)
        {
            // only set navigation treeview selection when navigation filter is set to drives
            if (RootNr == 0)
            {
                string tempPath = string.Empty;
                List<string> snapshot = new List<string>() { NavigationTreeViewUtils.separator };
                foreach (string directory in path.Split(Path.DirectorySeparatorChar))
                {
                    if (snapshot.Count == 1)
                    {
                        tempPath += directory;
                        snapshot.Add(tempPath);
                    }
                    else
                    {
                        tempPath += Path.DirectorySeparatorChar + directory;
                        snapshot.Add(snapshot[snapshot.Count - 1] + NavigationTreeViewUtils.separator + tempPath);
                    }
                }
                // delete all rootChildren
                foreach (INavigationTreeViewItem children in rootChildren)
                    children.DeleteChildren();
                rootChildren.Clear();
                // create treeRootItem 
                RootNr = 0;
                NavigationTreeViewItem treeRootItem = NavigationTreeViewRootItemUtils.ReturnRootItem(appConfig, RootNr, includeFiles);
                TreeName = treeRootItem.FriendlyName;
                treeRootItem.Filter = filter;
                // copy children treeRootItem to RootChildren, set up new tree 
                RootChildren = new ObservableCollection<INavigationTreeViewItem>(treeRootItem.Children);
                // expand previous snapshot
                NavigationTreeViewUtils.ExpandSnapShotItems(snapshot, treeRootItem);
            }
        }
        #endregion
    }
}
