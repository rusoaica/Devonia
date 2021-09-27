/// Written by: Yulia Danilova
/// Creation Date: 26th of November, 2020
/// Purpose: Interface business model for user interface options
#region ========================================================================= USING =====================================================================================
using System;
using System.IO;
using System.Threading.Tasks;
using Devonia.Models.Common.Broadcasting;
using Devonia.Infrastructure.Notification;
using Devonia.Infrastructure.Configuration;
#endregion

namespace Devonia.Models.Core.Options
{
    public class OptionsInterface : NotifyPropertyChanged, IOptionsInterface
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public static event Action<int> ThemeChanged;
        public static event Action<string> BackgroundChanged;
        
        private readonly IAppConfig appConfig;
        private readonly INotificationService notificationService;
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        private string backgroundImagePath = "background.jpg"; 
        public string BackgroundImagePath
        {
            get { return backgroundImagePath; }
            set { backgroundImagePath = value; Notify(); }
        }

        private string weatherUrl = string.Empty;
        public string WeatherUrl
        {
            get { return weatherUrl; }
            set { weatherUrl = value; Notify(); }
        }

        private int selectedThemeIndex;
        public int SelectedThemeIndex
        {
            get { return selectedThemeIndex; }
            set { selectedThemeIndex = value; Notify(); }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="appConfig">The injected application's configuration</param>
        /// <param name="notificationService">The injected service used for displaying notifications</param>
        public OptionsInterface(IAppConfig appConfig, INotificationService notificationService)
        {
            this.appConfig = appConfig;
            this.notificationService = notificationService;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the user interface options from the application's configuration
        /// </summary>
        public void GetUserInterfaceOptions()
        {
            //if (!string.IsNullOrEmpty(appConfig.Settings.PlayerPath))
            //{
            //    SelectedThemeIndex = appConfig.Settings.SelectedTheme == "Dark" ? 0 : 1;
            //    WeatherUrl = appConfig.Settings.WeatherUrl;
            //    if (!string.IsNullOrEmpty(appConfig.Settings.BackgroundImagePath) && File.Exists(appConfig.Settings.BackgroundImagePath))
            //        BackgroundImagePath = appConfig.Settings.BackgroundImagePath;
            //    else
            //        BackgroundImagePath = "background.jpg";
            //}
        }

        /// <summary>
        /// Updates the application's configurations for the user interface
        /// </summary>
        public async Task UpdateUserInterfaceOptionsAsync()
        {
            //appConfig.Settings.SelectedTheme = selectedThemeIndex == 0 ? "Dark" : "Light";
            //appConfig.Settings.WeatherUrl = weatherUrl;
            //appConfig.Settings.BackgroundImagePath = backgroundImagePath;
            //await appConfig.UpdateConfigurationAsync();
            //await notificationService.ShowAsync("User interface settings have been updated!", "Devonia - Success");
            //ThemeChanged?.Invoke(selectedThemeIndex);
            //BackgroundChanged?.Invoke(backgroundImagePath);
        }
        #endregion
    }
}
