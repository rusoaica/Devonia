/// Written by: Yulia Danilova
/// Creation Date: 12th of November, 2020
/// Purpose: View Model for the startup login view
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
using Devonia.ViewModels.Register;
using Devonia.Infrastructure.Enums;
using Devonia.Models.Core.Security;
using Devonia.ViewModels.Common.MVVM;
using Devonia.Infrastructure.Security;
using Devonia.Infrastructure.Notification;
using Devonia.Infrastructure.Configuration;
using Devonia.ViewModels.Common.ViewFactory;
using Devonia.ViewModels.Main;
#endregion

namespace Devonia.ViewModels.Startup
{
    public class StartupVM : BaseModel, IStartupVM
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAppConfig config;
        private readonly IViewFactory viewFactory;
        private readonly IAuthentication authentication;
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public ISyncCommand ViewOpened_Command { get; private set; }
        public IAsyncCommand LoginAsync_Command { get; private set; }
        public IAsyncCommand AutoLoginAsync_Command { get; private set; }
        public IAsyncCommand ChangePasswordAsync_Command { get; private set; }
        public IAsyncCommand RecoverPasswordAsync_Command { get; private set; }
        public IAsyncCommand RegisterUsernameAsync_Command { get; private set; }
        public IAsyncCommand RememberCredentialsAsync_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES =============================================================================
        public string Password
        {
            private get { return authentication.User.Password; }
            set 
            {
                authentication.User.Password = value;
                Notify();
                if (string.IsNullOrEmpty(value))
                {
                    RememberCredentials = false;
                    AutoLogin = false;
                }
                LoginAsync_Command?.RaiseCanExecuteChanged(); 
            }
        }

        public string Username
        {
            get { return authentication.User.Username; }
            set 
            {
                authentication.User.Username = value;
                Notify();
                if (string.IsNullOrEmpty(value))
                {
                    RememberCredentials = false;
                    AutoLogin = false;
                }
                LoginAsync_Command.RaiseCanExecuteChanged();
                RecoverPasswordAsync_Command.RaiseCanExecuteChanged();
                ChangePasswordAsync_Command.RaiseCanExecuteChanged();
            }
        }

        public bool AutoLogin
        {
            get { return authentication.AutoLogin; }
            set { authentication.AutoLogin = value;  Notify(); }
        }

        public bool RememberCredentials
        {
            get { return authentication.RememberCredentials; }
            set { authentication.RememberCredentials = value; Notify(); }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// <param name="authentication">The injected authentication domain model</param>
        /// <param name="notificationService">Injected notification service</param>
        /// <param name="viewFactory">The injected abstract factory for creating views</param>
        /// <param name="config">The injected application's configuration</param>
        /// </summary>
        public StartupVM(IAuthentication authentication, INotificationService notificationService, IViewFactory viewFactory, IAppConfig config)
        {
            this.config = config;
            this.viewFactory = viewFactory;
            this.authentication = authentication;
            this.notificationService = notificationService;
            ViewOpened_Command = new SyncCommand(ViewOpened);
            AutoLoginAsync_Command = new AsyncCommand(AutologinAsync);
            LoginAsync_Command = new AsyncCommand(LoginAsync, ValidateLogin);
            RegisterUsernameAsync_Command = new AsyncCommand(RegisterUsernameAsync);
            RememberCredentialsAsync_Command = new AsyncCommand(UpdateRememberCredentialsAsync);
            ChangePasswordAsync_Command = new AsyncCommand(ChangePasswordAsync, ValidateRecoverPassword);
            RecoverPasswordAsync_Command = new AsyncCommand(RecoverPasswordAsync, ValidateRecoverPassword);
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Stores the credentials in the application's configuration file, for later retrival
        /// </summary>
        private async Task UpdateRememberCredentialsAsync()
        {
            try
            {
                await authentication.RememberLoginCredentialsAsync();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
            }
            if (string.IsNullOrEmpty(Username) || Password.Length == 0)
            {
                RememberCredentials = false;
                AutoLogin = false;
            }
        }

        /// <summary>
        /// Logs in a user automatically, if <see cref="RememberCredentials"/> is True
        /// </summary>
        private async Task AutologinAsync()
        {
            await authentication.AutoLoginAsync();
            if (string.IsNullOrEmpty(Username) || Password.Length == 0)
            {
                RememberCredentials = false;
                AutoLogin = false;
            }
        }

        /// <summary>
        /// Performs the login
        /// </summary>
        public async Task LoginAsync()
        {
            ShowProgressBar();
            try
            {
                // authenticate the user
                authentication.Login();
                // if login was successful, hide this view and display the main view as modal
                HideView();
                await viewFactory.CreateView<IMainWindowView>().ShowDialogAsync();
                ShowView();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
            }
            HideProgressBar();
        }

        /// <summary>
        /// Validates the required information for Login
        /// </summary>
        /// <returns>True if required information is fine, False otherwise</returns>
        private bool ValidateLogin()
        {
            bool isValid = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
            if (!isValid)
            {
                ShowHelpButton();
                WindowHelp = "\n";
                if (string.IsNullOrEmpty(Username))
                    WindowHelp += "Username cannot be empty!\n";
                if (string.IsNullOrEmpty(Password))
                    WindowHelp += "Password cannot be empty!\n";
            }
            else
                HideHelpButton();
            return isValid;
        }

        /// <summary>
        /// Validates the required information for recovering the password of an account
        /// </summary>
        /// <returns>True if required information is fine, False otherwise</returns>
        private bool ValidateRecoverPassword()
        {
            return !string.IsNullOrEmpty(Username);
        }

        /// <summary>
        /// Opens up the Register View
        /// </summary>
        private async Task RegisterUsernameAsync()
        {
            HideView();
            await viewFactory.CreateView<IRegisterView>().ShowDialogAsync();
            ShowView();
        }

        /// <summary>
        /// Opens up the Recover Password view
        /// </summary>
        private async Task RecoverPasswordAsync()
        {
            HideView();
            await viewFactory.CreateView<IRecoverPasswordView>(Username).ShowDialogAsync();
            ShowView();
        }

        /// <summary>
        /// Opens up the Change Password view
        /// </summary>
        private async Task ChangePasswordAsync()
        {
            HideView();
            await viewFactory.CreateView<IChangePasswordView>(Username).ShowDialogAsync();
            ShowView();
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles the ViewOpened event of the view
        /// </summary>
        private void ViewOpened()
        {
            if (config.Authentication.RememberCredentials)
            {
                if (!string.IsNullOrEmpty(config.Authentication.Username))
                    Username = Crypto.Decrypt(config.Authentication.Username);
                if (!string.IsNullOrEmpty(config.Authentication.Password))
                    Password = Crypto.Decrypt(config.Authentication.Password);
                RememberCredentials = true;
                if (config.Authentication.Autologin)
                    AutoLogin = true;
            }
            WindowTitle = "Devonia - Authentication";
        }
        #endregion
    }
}
