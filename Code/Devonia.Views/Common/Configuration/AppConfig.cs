/// Written by: Yulia Danilova
/// Creation Date: 14th of May, 2021
/// Purpose: Model for strongly typed application configuration values
#region ========================================================================= USING =====================================================================================
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.Infrastructure.Notification;
using Devonia.Infrastructure.Configuration;
using System.Collections.Generic;
#endregion

namespace Devonia.Views.Common.Configuration
{
    internal sealed class AppConfig : IAppConfig
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly INotificationService notificationService;
        #endregion

        #region =============================================================== PROPERTIES ==================================================================================
        public List<UserConfigEntity> Users { get; set; } = new List<UserConfigEntity>();
        public DialogsConfigEntity Dialogs { get; set; } = new DialogsConfigEntity(); 
        public ApplicationConfigEntity Application { get; set; } = new ApplicationConfigEntity();
        public MiscellaneousConfigEntity Miscellaneous { get; set; } = new MiscellaneousConfigEntity();
        public AuthenticationConfigEntity Authentication { get; set; } = new AuthenticationConfigEntity();
        [JsonIgnore]
        public string? ConfigurationFilePath { get; internal set; }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="notificationService">The injected notification service to use</param>
        public AppConfig(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Saves the application's configuration settings
        /// </summary>
        public async Task UpdateConfigurationAsync()
        {
            if (!string.IsNullOrEmpty(ConfigurationFilePath) && File.Exists(ConfigurationFilePath))
                File.WriteAllText(ConfigurationFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
            else
                await notificationService.ShowAsync("Application's configuration file was not found!", "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
        }
        #endregion
    }
}
