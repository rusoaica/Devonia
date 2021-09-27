/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Utility class for navigation tree view root items
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Devonia.Infrastructure.Configuration;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public static class NavigationTreeViewRootItemUtils
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        // convention: Root items end with:
        public const string LastPartRootItemName = "RootItem";
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets all the special navigation treeview items 
        /// </summary>
        /// <returns>A list containing the names of the special navigation treeview items</returns>
        public static List<string> ListNavigationTreeViewRootItemsByConvention()
        {
            List<string> List = new List<string> { };
            // convention: all classes that end with "RootItem" form the rootlist 
            IEnumerable<Type> entityTypes = Assembly.GetAssembly(typeof(NavigationTreeViewItem))
                                                    .GetTypes()
                                                    .Where(t => t.IsSubclassOf(typeof(NavigationTreeViewItem)));
            foreach (Type type in entityTypes)
                if (type.Name.EndsWith(LastPartRootItemName))
                    List.Add(type.Name.Replace(LastPartRootItemName, string.Empty));
            return List;
        }

        /// <summary>
        /// Gets the root items specified by <paramref name="rootNr"/>
        /// </summary>
        /// <param name="appConfig">The injected application's configuration</param>
        /// <param name="rootNr">The root number of the root to get</param>
        /// <param name="includeFileChildren">Whether to include files or not</param>
        /// <param name="filter">Optional filter for file types</param>
        /// <returns>A root item identified by <paramref name="rootNr"/></returns>
        public static NavigationTreeViewItem ReturnRootItem(IAppConfig appConfig, int rootNr, bool includeFileChildren = false, List<string> filter = null)
        {
            Type selectedType = typeof(DriveRootItem);
            string selectedName = "Drive";
            // can you find other type given the conventions.. RootItem name and rootNr
            IEnumerable<Type> entityTypes = Assembly.GetAssembly(typeof(NavigationTreeViewItem))
                                                    .GetTypes()
                                                    .Where(t => t.IsSubclassOf(typeof(NavigationTreeViewItem)));
            int i = 0;
            foreach (Type type in entityTypes)
            {
                if (type.Name.EndsWith(LastPartRootItemName))
                {
                    if (i == rootNr)
                    {
                        selectedType = Type.GetType(type.FullName);
                        selectedName = type.Name.Replace(LastPartRootItemName, "");
                        break;
                    }
                    i++;
                }
            }
            // use selectedType to create root       
            NavigationTreeViewItem rootItem;
            if (selectedType == typeof(FavoritesRootItem))
                rootItem = (NavigationTreeViewItem)Activator.CreateInstance(selectedType, appConfig, includeFileChildren, filter);
            else
                rootItem = (NavigationTreeViewItem)Activator.CreateInstance(selectedType);
            rootItem.FriendlyName = selectedName;
            rootItem.IncludeFileChildren = includeFileChildren;
            rootItem.Filter = filter;
            return rootItem;
        }
        #endregion
    }
}
