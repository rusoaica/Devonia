/// Written by: Yulia Danilova
/// Creation Date: 16th of June, 2020
/// Purpose: User data transfer object
#region ========================================================================= USING =====================================================================================
using System;
#endregion

namespace Devonia.Models.Common.Models.Users
{
    public class UserEntity
    {
        #region ================================================================ PROPERTIES =================================================================================
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecurityAnswer { get; set; }
        public string PasswordConfirm { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswerConfirm { get; set; }
        public DateTime Created { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Customized ToString() method
        /// </summary>
        /// <returns>Custom string value showing relevant data for current class</returns>
        public override string ToString()
        {
            return Id + " :: " + Username;
        }
        #endregion
    }
}
