/// Written by: Yulia Danilova
/// Creation Date: 05th of July, 2021
/// Purpose: View Model for the Change Password view
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.Models.Core.Security;
using Devonia.ViewModels.Common.MVVM;
using Devonia.Infrastructure.Notification;
#endregion

namespace Devonia.ViewModels.Register
{
    public class ChangePasswordVM : BaseModel, IChangePasswordVM
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAuthentication authentication;
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public IAsyncCommand ViewOpenedAsync_Command { get; private set; }
        public IAsyncCommand ChangePasswordAsync_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES =============================================================================
        private string oldPassword;
        public string OldPassword
        {
            private get { return oldPassword; }
            set { oldPassword = value; authentication.User.Password = value; Notify(); ChangePasswordAsync_Command.RaiseCanExecuteChanged(); }
        }

        private string newPassword;
        public string NewPassword
        {
            private get { return newPassword; }
            set { newPassword = value; Notify(); ChangePasswordAsync_Command.RaiseCanExecuteChanged(); }
        }

        private string newPasswordConfirm;
        public string NewPasswordConfirm
        {
            private get { return newPasswordConfirm; }
            set { newPasswordConfirm = value; Notify(); ChangePasswordAsync_Command.RaiseCanExecuteChanged(); }
        }

        public string Username
        {
            get { return authentication.User.Username; }
            set { authentication.User.Username = value; Notify(); }
        }
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public string Id { set { Username = value; } }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="authentication">The injected authentication domain model</param>
        /// <param name="notificationService">Injected notification service</param>
        public ChangePasswordVM(IAuthentication authentication, INotificationService notificationService)
        {
            ViewOpenedAsync_Command = new AsyncCommand(ViewOpenedAsync);
            ChangePasswordAsync_Command = new AsyncCommand(ChangePasswordAsync, ValidateChangePassword);
            this.authentication = authentication;
            this.notificationService = notificationService;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Changes the password of a user account
        /// </summary>
        public async Task ChangePasswordAsync()
        {
            ShowProgressBar();
            try
            {
                // test if authentication succeeds (password is correct)
                authentication.Login();
                // assign the new password
                authentication.User.Password = NewPassword;
                await authentication.ChangePasswordAsync();
                await notificationService.ShowAsync("The password has been changed!", "Devonia - Success");
                await authentication.RememberLoginCredentialsAsync();
                CloseView();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
            }
            HideProgressBar();
        }

        /// <summary>
        /// Validates the required information for changing a password
        /// </summary>
        /// <returns>True if required information is fine, False otherwise</returns>
        public bool ValidateChangePassword()
        {
            bool isValid = !string.IsNullOrEmpty(Username) && 
                OldPassword != null && OldPassword.Length > 0 && 
                NewPassword != null && NewPassword.Length > 0 && 
                NewPasswordConfirm != null && NewPasswordConfirm.Length > 0 &&
                NewPassword == NewPasswordConfirm;
            if (!isValid)
            {
                ShowHelpButton();
                WindowHelp = "\n";
                if (string.IsNullOrWhiteSpace(Username))
                    WindowHelp += "Username cannot be empty!\n";
                if (OldPassword == null || OldPassword.Length == 0)
                    WindowHelp += "Current password cannot be empty!\n";
                if (NewPassword == null || NewPassword.Length == 0)
                    WindowHelp += "Password cannot be empty!\n";
                if (NewPasswordConfirm == null || NewPasswordConfirm.Length == 0)
                    WindowHelp += "Password Confirm cannot be empty!\n";
                if (NewPassword != null && NewPasswordConfirm != null && NewPassword != NewPasswordConfirm)
                    WindowHelp += "Password and Password Confirm do not match!\n";
            }
            else
                HideHelpButton();
            return isValid;
        }

        /// <summary>
        /// Displays the help for the current window
        /// </summary>
        public override async Task ShowHelpAsync()
        {
            await notificationService.ShowAsync(WindowHelp, "Devonia - Help");
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles the Opened event of the view
        /// </summary>
        private async Task ViewOpenedAsync()
        {
            WindowTitle = "Devonia - Change password";
            try
            {
                ShowProgressBar();
                // get the account details based on the provided username
                authentication.GetUser(Username);
                HideProgressBar();
            }
            catch (Exception ex) when (ex is InvalidOperationException)
            {
                await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
            }
        }
        #endregion
    }
}
