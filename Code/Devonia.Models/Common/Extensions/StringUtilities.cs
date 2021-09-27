/// Written by: Yulia Danilova
/// Creation Date: 25th of June, 2021
/// Purpose: String extension methods
#region ========================================================================= USING =====================================================================================
using System;
using System.Security;
#endregion

namespace Devonia.Models.Common.Extensions
{
    public static class StringUtilities
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Converts a <see cref="string"/> into a <see cref="SecureString"/> 
        /// </summary>
        /// <param name="data">The <see cref="string"/> to be converted</param>
        /// <returns>A <see cref="SecureString"/> containing the encrypted <see cref="string"/> data.</returns>
        public static SecureString ToSecureString(this string data)
        {
            SecureString secureString = new SecureString();
            char[] chars = data.ToCharArray();
            foreach (char c in chars)
                secureString.AppendChar(c);
            return secureString;
        }

        /// <summary>
        /// Converts a <see cref="string"/> into a <see cref="{T}"/> 
        /// </summary>
        /// <typeparam name="T">The type to convert into</typeparam>
        /// <param name="valueAsString">The <see cref="string"/> to be converted</param>
        /// <returns>A <see cref="{T}"/> representing the converted value of <param name="valueAsString"></returns>
        public static T? GetValueOrNull<T>(this string valueAsString) where T : struct
        {
            if (string.IsNullOrEmpty(valueAsString) || valueAsString.ToLower() == "null")
                return null;
            return (T)Convert.ChangeType(valueAsString, typeof(T));
        }
        #endregion
    }
}
