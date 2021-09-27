/// Written by: wim4you, Yulia Danilova
/// Creation Date: 23rd of May, 2012
/// Purpose: Code behind for the FileSystemTreeView user control
#region ========================================================================= USING =====================================================================================
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endregion

namespace Devonia.Views.Common.Controls
{
    public partial class FileSystemTreeView : UserControl
    {
        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public FileSystemTreeView()
        {
            AvaloniaXamlLoader.Load(this);
        }
        #endregion
    }
}
