/// Written by: Yulia Danilova
/// Creation Date: 18th of November, 2020
/// Purpose: Interface business model for authentication
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Models.Common.Models.Users;
#endregion

namespace Devonia.Models.Core.Security
{
    public interface IAuthentication
    {
        #region ================================================================ PROPERTIES =================================================================================
        UserEntity User { get; set; }
        bool AutoLogin { get; set; }
        bool RememberCredentials { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Verifies the provided credentials against the credentials stored in the storage medium
        /// </summary>
        void Login();

        /// <summary>
        /// Gets the details of an user identified by <paramref name="username"/>
        /// </summary>
        void GetUser(string username);

        /// <summary>
        /// Updates the password of the current account
        /// </summary>
        Task ChangePasswordAsync();

        /// <summary>
        /// Registers a new username in the storage medium
        /// </summary>
        Task RegisterUsernameAsync();

        /// <summary>
        /// Stores the credentials in the application's configuration file, for later retrival
        /// </summary>
        Task RememberLoginCredentialsAsync();

        /// <summary>
        /// Recovers an account by checking the security answer against the one stored in the storage medium
        /// </summary>
        Task RecoverAccountAsync();

        /// <summary>
        /// Logs in a user automatically, if <see cref="RememberCredentials"/> is True
        /// </summary>
        Task AutoLoginAsync();
        #endregion
    }
}
