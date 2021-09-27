/// Written by: Yulia Danilova
/// Creation Date: 11th of November, 2020
/// Purpose: Code behind for the StartupV view
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using Devonia.ViewModels.Startup;
using Devonia.Infrastructure.Configuration;
#endregion

namespace Devonia.Views.Startup
{
    public partial class StartupV : Window, IStartupView
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private bool isWindowLoaded;
        private readonly IAppConfig appConfig;
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public static StartupV Instance { get; private set; }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public StartupV()
        {
        }

        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="appConfig">Injected application's configuration service</param>
        public StartupV(IAppConfig appConfig)
        {
            AvaloniaXamlLoader.Load(this);
            Instance = this;
#if DEBUG
            this.AttachDevTools();
            this.appConfig = appConfig;
#endif
            // set the application's theme from the application's configuration setting
            App.styles.SwitchThemeByIndex(appConfig.Miscellaneous.SelectedTheme == "Dark" ? 0 : 1);
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows the current window as a modal dialog
        /// </summary>
        public async Task<bool?> ShowDialogAsync()
        {
            return await ShowDialog<bool?>(Instance);
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles window's Opened event
        /// </summary>
        private void Window_Opened(object? sender, EventArgs e)
        {
            (DataContext as StartupVM).ShowingView += (s, e) => Show();
            (DataContext as StartupVM).HidingView += (s, e) => Hide();
            if (appConfig.Application.StartupWindowPositionX != null)
                Position = Position.WithX((int)appConfig.Application.StartupWindowPositionX);
            if (appConfig.Application.StartupWindowPositionY != null)
                Position = Position.WithY((int)appConfig.Application.StartupWindowPositionY);
            isWindowLoaded = true;
        }

        /// <summary>
        /// Handles Window's PositionChanged event
        /// </summary>
        private async void Window_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            // do not allow the application's configuration to be updated with the new position
            // unless the window is loaded and the user is the one changing it
            if (isWindowLoaded)
            {
                appConfig.Application.StartupWindowPositionX = Position.X;
                appConfig.Application.StartupWindowPositionY = Position.Y;
                await appConfig.UpdateConfigurationAsync();
            }
        }
        #endregion
    }
}
