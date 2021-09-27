/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Navigation tree view item for links
/// Remark: based on https://www.codeproject.com/Articles/390514/Playing-with-a-MVVM-Tabbed-TreeView-for-a-File-Exp
#region ========================================================================= USING =====================================================================================
using System.IO;
#endregion

namespace Devonia.ViewModels.Common.Controls.NavigationTreeView
{
    public static class FolderPlaneUtils
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Checks whether <paramref name="path"/> is a directory or not
        /// </summary>
        /// <param name="path">The path to the directory to be checked</param>
        /// <returns>True if <paramref name="path"/> is a directory, False otherwise</returns>
        public static bool IsFolder(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Checks if <paramref name="path"/> is a drive or not
        /// </summary>
        /// <param name="path">The path to the drive to be checked</param>
        /// <returns>True if <paramref name="path"/> is a drive, False otherwise</returns>
        public static bool IsDrive(string path)
        {
            bool isDrive = false;
            foreach (string driveName in Directory.GetLogicalDrives())
                if (driveName.Contains(path))
                    isDrive = true; 
            return isDrive;
        }

        /// <summary>
        /// Checks if <paramref name="path"/> is a link or not
        /// </summary>
        /// <param name="path">The path to the link to be checked</param>
        /// <returns>True if <paramref name="path"/> is a link, False otherwise</returns>
        public static bool IsLink(string path)
        {
            return Path.GetExtension(path).ToLower() == ".lnk";
        }

        /// <summary>
        /// Resolves the path to a file that is linked by a link file located at <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path of the link file</param>
        /// <returns>The path to the file that is linked by the link file located at <paramref name="path"/></returns>
        public static string ResolveShortCut(string path)
        {
            // TODO: re-implement!
            //if (FolderPlaneUtils.IsLink(path))
            //{
            //    // First working solution chosen to resolve link
            //    // For using IWshRuntimeLibrary add reference, Com tab, choose Microsoft Shell Controls and Automation
            //    // Question: must we dispose shell, I assume this is a Managed wrapper around com

            //    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            //    IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);

            //    string target = link.TargetPath;
            //    return target;
            //}

            //// If not found, keep original path
            //return path;
            return null;
        }       
        #endregion
    }
}
