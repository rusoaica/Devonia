/// Written by: Yulia Danilova
/// Creation Date: 05th of July, 2021
/// Purpose: Code behind for the ChangePasswordV view
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Devonia.Views.Startup;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using Devonia.ViewModels.Register;
#endregion

namespace Devonia.Views.Register
{
    public partial class ChangePasswordV : Window, IChangePasswordView
    {
        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public ChangePasswordV()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows the current window as a modal dialog
        /// </summary>
        public async Task<bool?> ShowDialogAsync()
        {
            return await ShowDialog<bool?>(StartupV.Instance);
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles window KeyUp event
        /// </summary>
        private void Window_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        /// <summary>
        /// Handles the Window's Opened event
        /// </summary>
        private void Window_Opened(object? sender, EventArgs e)
        {
            // allow closing View from ViewModel without breaking MVVM
            (DataContext as ChangePasswordVM).ClosingView += (sender, e) => Close();
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        private void CloseWindow_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
