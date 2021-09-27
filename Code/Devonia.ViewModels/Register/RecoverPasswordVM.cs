/// Written by: Yulia Danilova
/// Creation Date: 25th of June, 2021
/// Purpose: View Model for the Recover Password view
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
using Devonia.Models.Core.Security;
using Devonia.Infrastructure.Enums;
using Devonia.ViewModels.Common.MVVM;
using Devonia.Infrastructure.Notification;
#endregion

namespace Devonia.ViewModels.Register
{
    public class RecoverPasswordVM : BaseModel, IRecoverPasswordVM
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAuthentication authentication;
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public IAsyncCommand ViewOpenedAsync_Command { get; private set; }
        public IAsyncCommand RecoverAccountAsync_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES =============================================================================
        private string password;
        public string Password
        {
            private get { return password; }
            set { password = value; authentication.User.Password = value; }
        }

        private string securityAnswer;
        public string SecurityAnswer
        {
            private get { return securityAnswer; }
            // uses SecurityAnswerConfirm because SecurityAnswer is already populated with the data from the database, against which it needs to be compared
            set { securityAnswer = value; Notify(); RecoverAccountAsync_Command?.RaiseCanExecuteChanged(); authentication.User.SecurityAnswerConfirm = value; } 
        }

        private string username = string.Empty;
        public string Username
        {
            get { return username; }
            set { username = value; Notify(); authentication.User.Username = value; }
        }

        private string securityQuestion = string.Empty;
        public string SecurityQuestion
        {
            get { return securityQuestion; }
            set { securityQuestion = value; Notify(); authentication.User.SecurityQuestion = value; }
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
        public RecoverPasswordVM(IAuthentication authentication, INotificationService notificationService)
        {
            ViewOpenedAsync_Command = new AsyncCommand(ViewOpenedAsync);
            RecoverAccountAsync_Command = new AsyncCommand(RecoverAccountAsync, ValidateRecover);
            this.authentication = authentication;
            this.notificationService = notificationService;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Recovers an existing user account
        /// </summary>
        public async Task RecoverAccountAsync()
        {
            ShowProgressBar();
            try
            {
                await authentication.RecoverAccountAsync();
                CloseView();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
            }
            HideProgressBar();
        }

        /// <summary>
        /// Validates the required information for recovering a password
        /// </summary>
        /// <returns>True if required information is fine, False otherwise</returns>
        public bool ValidateRecover()
        {
            bool isValid = !string.IsNullOrEmpty(Username) && SecurityQuestion != null && SecurityQuestion.Length > 0 && SecurityAnswer != null && SecurityAnswer.Length > 0;
            if (!isValid)
            {
                ShowHelpButton();
                WindowHelp = "\n";
                if (string.IsNullOrWhiteSpace(Username))
                    WindowHelp += "Username cannot be empty!\n";
                if (string.IsNullOrWhiteSpace(SecurityQuestion))
                    WindowHelp += "Security Question cannot be empty!\n";
                if (SecurityAnswer == null || SecurityAnswer.Length == 0)
                    WindowHelp += "Security Answer cannot be empty!\n";
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
            WindowTitle = "Devonia - Recover password";
            try
            {
                ShowProgressBar();
                // get the account details based on the provided username
                authentication.GetUser(username);
                SecurityQuestion = authentication.User.SecurityQuestion;
                RecoverAccountAsync_Command?.RaiseCanExecuteChanged();
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
