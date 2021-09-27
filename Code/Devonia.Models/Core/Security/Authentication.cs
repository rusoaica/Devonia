/// Written by: Yulia Danilova
/// Creation Date: 18th of November, 2020
/// Purpose: Business model for authentication
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
using Devonia.Infrastructure.Security;
using Devonia.Models.Common.Models.Users;
using Devonia.Infrastructure.Notification;
using Devonia.Infrastructure.Configuration;
using System.Linq;
#endregion

namespace Devonia.Models.Core.Security
{
    public class Authentication : IAuthentication
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAppConfig config;
        private readonly INotificationService notificationService;
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public bool AutoLogin { get; set; }
        public bool RememberCredentials { get; set; }
        public UserEntity User { get; set; } = new UserEntity();
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// <param name="unitOfWork">Injected unit of work for interacting with the data access layer repositories</param>
        /// <param name="config">Injected configuration for itneracting with the application's configuration</param>
        /// <param name="notificationService">The injected service used for displaying notifications</param>
        /// </summary>
        public Authentication(IAppConfig config, INotificationService notificationService)
        {
            this.config = config;
            this.notificationService = notificationService;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Updates the password of the current account
        /// </summary>
        public async Task ChangePasswordAsync()
        {
            // make sure the username for whom the password is changed exists in the storage medium
            if (config.Users.Where(u => u.Username == Crypto.Encrypt(User.Username)).Count() == 1)
            {
                config.Users.Where(u => u.Username == Crypto.Encrypt(User.Username)).First().Password = Crypto.Encrypt(User.Password);
                // update the user in the storage medium
                try
                {
                    await config.UpdateConfigurationAsync();
                }
                catch
                {
                    throw new InvalidOperationException("Error updating the user in the repository!");
                }
            }
            else
                throw new InvalidOperationException("Cannot update a password of an inexistent account!");
        }

        /// <summary>
        /// Verifies the provided credentials against the credentials stored in the storage medium
        /// </summary>
        public void Login()
        {
            // check required members
            if (!string.IsNullOrEmpty(User.Username))
            {
                if (User.Password != null && User.Password.Length > 0)
                {
                    // get the details of the user from the storage medium
                    if (config.Users.FirstOrDefault(e => e.Username == Crypto.Encrypt(User.Username) && e.Password == Crypto.Encrypt(User.Password)) == null)
                        throw new InvalidOperationException("Invalid username or password!");
                }
                else
                    throw new ArgumentException("The password cannot be empty!", "Passowrd");
            }
            else
                throw new ArgumentException("The username cannot be empty!", "Username");
        }

        /// <summary>
        /// Registers a new username in the application's configuration
        /// </summary>
        public async Task RegisterUsernameAsync()
        {
            // check required members
            if (!string.IsNullOrEmpty(User.Username))
            {
                if (User.Password != null && User.Password.Length > 0)
                {
                    if (!string.IsNullOrEmpty(User.SecurityQuestion))
                    {
                        if (User.SecurityAnswer != null && User.SecurityAnswer.Length > 0)
                        {
                            // make sure the username about to be registered does not already exists in the storage medium
                            if (config.Users.Where(e => e.Username == Crypto.Encrypt(User.Username)).Count() == 0)
                            {
                                // insert the user in the application's configuration
                                config.Users.Add(new UserConfigEntity()
                                {
                                    Username = Crypto.Encrypt(User.Username),
                                    Password = Crypto.Encrypt(User.Password),
                                    SecurityAnswer = Crypto.Encrypt(User.SecurityAnswer),
                                    SecurityQuestion = Crypto.Encrypt(User.SecurityQuestion)
                                });
                                try
                                {
                                    await config.UpdateConfigurationAsync();
                                }
                                catch
                                {
                                    throw new InvalidOperationException("Error inserting the user in the storage medium!");
                                }
                            }
                            else
                                throw new InvalidOperationException("Specified username is already taken!");
                        }
                        else
                            throw new ArgumentException("The security answer cannot be empty!", "SecurityAnswer");
                    }
                    else
                        throw new ArgumentException("The security question cannot be empty!", "SecurityQuestion");
                }
                else
                    throw new ArgumentException("The password cannot be empty!", "Passowrd");
            }
            else
                throw new ArgumentException("The username cannot be empty!", "Username");
        }

        /// <summary>
        /// Recovers an account by checking the security answer against the one stored in the application's configuration
        /// </summary>
        public async Task RecoverAccountAsync()
        {
            // check required members
            if (!string.IsNullOrEmpty(User.Username))
            {
                if (User.SecurityAnswerConfirm != null && User.SecurityAnswerConfirm.Length > 0)
                {
                    // get the details of the user from the storage medium
                    if (config.Users.Count(u => u.Username == Crypto.Encrypt(User.Username)) > 0)
                    {
                        // check if the provided security answer coincides with the one from the storage medium
                        if (Crypto.Encrypt(User.SecurityAnswerConfirm) != config.Users.First(u => u.Username == Crypto.Encrypt(User.Username)).SecurityAnswer)
                            throw new InvalidOperationException("Invalid security answer!");
                        else
                        {
                            config.Users.First(u => u.Username == Crypto.Encrypt(User.Username)).Password = Crypto.Encrypt(User.Username);
                            try
                            {
                                await config.UpdateConfigurationAsync();
                                await notificationService.ShowAsync("Your password has been changed to " + User.Username + "!\nChange it to a secure password as soon as you log in!", "LEYA - Success");
                            }
                            catch (Exception)
                            {
                                throw new InvalidOperationException("Error updating the password of the account!");
                            }
                        }
                    }
                    else
                        throw new InvalidOperationException("Invalid username or password!");
                }
                else
                    throw new ArgumentException("The security answer cannot be empty!", "SecurityAnswer");
            }
            else
                throw new ArgumentException("The username cannot be empty!", "Username");
        }

        /// <summary>
        /// Gets the details of an user identified by <paramref name="username"/>
        /// </summary>
        public void GetUser(string username)
        {
            // the id should always be a positive, non-zero value
            if (!string.IsNullOrEmpty(username))
            {
                // get the details of the user from the storage medium
                var result = config.Users.Where(u => u.Username == Crypto.Encrypt(username)).FirstOrDefault();
                if (result != null)
                {
                    User = new UserEntity() 
                    { 
                        Username = Crypto.Decrypt(result.Username), 
                        Password = Crypto.Decrypt(result.Password),
                        SecurityAnswer = Crypto.Decrypt(result.SecurityAnswer),
                        SecurityQuestion = Crypto.Decrypt(result.SecurityQuestion)
                    };
                }
                else
                    throw new InvalidOperationException("Specified username does not exist!");
            }
            else
                throw new InvalidOperationException("Cannot get a user without a username specified!");
        }

        /// <summary>
        /// Stores the credentials in the application's configuration file, for later retrieval
        /// </summary>
        public async Task RememberLoginCredentialsAsync()
        {
            if (RememberCredentials)
            {
                // make sure both username and password are set, before accepting "remember credentials" options to be enabled
                if (string.IsNullOrEmpty(User.Username))
                {
                    RememberCredentials = false;
                    AutoLogin = false;
                    throw new InvalidOperationException("For automatic login, enter the username!");
                }
                else if (string.IsNullOrEmpty(User.Password))
                {
                    RememberCredentials = false;
                    AutoLogin = false;
                    throw new InvalidOperationException("For automatic login, enter the password!");
                }
                else
                {
                    config.Authentication.RememberCredentials = true;
                    config.Authentication.Username = Crypto.Encrypt(User.Username);
                    config.Authentication.Password = Crypto.Encrypt(User.Password);
                    await config.UpdateConfigurationAsync();
                }
            }
            else
            {
                // if "Remember Credentials" is not enabled, automatic login is not permited
                AutoLogin = false;
                config.Authentication.Autologin = false;
                config.Authentication.RememberCredentials = false;
                config.Authentication.Username = string.Empty;
                config.Authentication.Password = string.Empty;
                await config.UpdateConfigurationAsync();
            }
        }

        /// <summary>
        /// Logs in a user automatically, if <see cref="RememberCredentials"/> is True
        /// </summary>
        public async Task AutoLoginAsync()
        {
            if (AutoLogin)
            {
                // we need both a username and a password for automatic login
                if (string.IsNullOrEmpty(User.Username) || User.Password.Length == 0 || !RememberCredentials)
                {
                    RememberCredentials = false;
                    AutoLogin = false;
                    throw new InvalidOperationException("Remember Credentials must be enabled!");
                }
                // save the automatic login state in the application's configuration
                config.Authentication.Autologin = true;
                await config.UpdateConfigurationAsync();
                // credentials remembering must be enabled for automatic login to work
                RememberCredentials = true;
                // wait 3 seconds for the user to be able to disable autologin, if so desired
                await Task.Delay(3000);
                Login();
            }
            else
            {
                config.Authentication.Autologin = false;
                await config.UpdateConfigurationAsync();
            }
        }
        #endregion
    }
}