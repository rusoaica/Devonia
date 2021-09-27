/// Written by: Yulia Danilova
/// Creation Date: 09th of November, 2020
/// Purpose: Code behind for the MsgBoxV window
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Controls;
using Devonia.Views.Startup;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Devonia.Views.Common.Dialogs.MessageBox;
using Devonia.ViewModels.Common.Dialogs.MessageBox;
#endregion

namespace Devonia.Views.Common.Dialogs
{
    public partial class MsgBoxV : Window, IMsgBoxView
    {
        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public MsgBoxV()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Opens a view and returns only when the newly opened view is closed.
        /// </summary>
        /// <returns>A nullable bool that specifies whether the activity was accepted (true) or canceled (false). 
        /// The return value is the value of the DialogResult property before a window closes</returns>
        public async Task<bool?> ShowDialogAsync()
        {
            return await ShowDialog<bool?>(StartupV.Instance);
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles the Cancel button's Click event
        /// </summary>
        private void Window_Close(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the Window's Opened event
        /// </summary>
        private void Window_Opened(object? sender, EventArgs e)
        {
            // allow closing View from ViewModel without breaking MVVM
            (DataContext as MsgBoxVM).ClosingView += (sender, e) => Close();
        }
        #endregion
    }
}
