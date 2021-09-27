/// Written by: Yulia Danilova
/// Creation Date: 18th of November, 2020
/// Purpose: View Model for the Register view
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
    public class RegisterVM : BaseModel, IRegisterVM
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAuthentication authentication;
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public ISyncCommand ViewOpened_Command { get; private set; }
        public IAsyncCommand RegisterAccount_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES =============================================================================
        private string password;
        public string Password
        {
            private get { return password; }
            set { password = value; Notify(); RegisterAccount_Command?.RaiseCanExecuteChanged(); authentication.User.Password = value; }
        }

        private string securityAnswer;
        public string SecurityAnswer
        {
            private get { return securityAnswer; }
            set { securityAnswer = value; Notify(); RegisterAccount_Command?.RaiseCanExecuteChanged(); authentication.User.SecurityAnswer = value; }
        }
        
        private string username = string.Empty;
        public string Username
        {
            get { return username; }
            set { username = value; Notify(); RegisterAccount_Command.RaiseCanExecuteChanged(); authentication.User.Username = value; }
        }

        private string securityQuestion = string.Empty;
        public string SecurityQuestion
        {
            get { return securityQuestion; }
            set { securityQuestion = value; Notify(); RegisterAccount_Command.RaiseCanExecuteChanged(); authentication.User.SecurityQuestion = value; }
        }
        
        private string confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            private get { return confirmPassword; }
            set { confirmPassword = value; Notify(); RegisterAccount_Command?.RaiseCanExecuteChanged(); authentication.User.PasswordConfirm = value; }
        }

        private string confirmSecurityAnswer = string.Empty;
        public string ConfirmSecurityAnswer
        {
            private get { return confirmSecurityAnswer; }
            set { confirmSecurityAnswer = value; Notify(); RegisterAccount_Command?.RaiseCanExecuteChanged(); authentication.User.SecurityAnswerConfirm = value; }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="authentication">The injected authentication domain model</param>
        /// <param name="notificationService">Injected notification service</param>
        public RegisterVM(IAuthentication authentication, INotificationService notificationService)
        {
            ViewOpened_Command = new SyncCommand(ViewOpened);
            RegisterAccount_Command = new AsyncCommand(RegisterUsername, ValidateRegister);
            this.authentication = authentication;
            this.notificationService = notificationService;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Registers a new user account
        /// </summary>
        public async Task RegisterUsername()
        {
            ShowProgressBar();
            try
            {
                await authentication.RegisterUsernameAsync();
                await notificationService.ShowAsync("Account created!", "Devonia - Success");
                CloseView();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                await notificationService.ShowAsync(ex.Message, "Devonia - Error", NotificationButton.OK, NotificationImage.Error);
            }
            HideProgressBar();
        }

        /// <summary>
        /// Validates the required information for registering a username
        /// </summary>
        /// <returns>True if required information is fine, False otherwise</returns>
        public bool ValidateRegister()
        {
            bool isValid = !string.IsNullOrEmpty(Username) && Password != null && Password.Length > 0 && ConfirmPassword != null && ConfirmPassword.Length > 0 && SecurityQuestion != null && 
                SecurityQuestion.Length > 0 && SecurityAnswer != null && SecurityAnswer.Length > 0 && Password == ConfirmPassword && ConfirmSecurityAnswer != null && 
                ConfirmSecurityAnswer.Length > 0 && SecurityAnswer == ConfirmSecurityAnswer;
            if (!isValid)
            {
                ShowHelpButton();
                WindowHelp = "\n";
                if (string.IsNullOrWhiteSpace(Username))
                    WindowHelp += "Username cannot be empty!\n";
                if (Password == null || Password.Length == 0)
                    WindowHelp += "Password cannot be empty!\n";
                if (ConfirmPassword == null || ConfirmPassword.Length == 0)
                    WindowHelp += "Password Confirm cannot be empty!\n";
                if (Password != null && ConfirmPassword != null && Password != ConfirmPassword)
                    WindowHelp += "Password and Password Confirm do not match!\n";
                if (string.IsNullOrWhiteSpace(SecurityQuestion))
                    WindowHelp += "Security Question cannot be empty!\n";
                if (SecurityAnswer == null || SecurityAnswer.Length == 0)
                    WindowHelp += "Security Answer cannot be empty!\n";
                if (ConfirmSecurityAnswer == null || ConfirmSecurityAnswer.Length == 0)
                    WindowHelp += "Security Answer Confirm cannot be empty!\n";
                if (SecurityAnswer != null && ConfirmSecurityAnswer != null && SecurityAnswer != ConfirmSecurityAnswer)
                    WindowHelp += "Security Answer and Security Answer Confirm do not match!\n";
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
        private void ViewOpened()
        {
            WindowTitle = "Devonia - Register new account";
        }
        #endregion
    }
}
