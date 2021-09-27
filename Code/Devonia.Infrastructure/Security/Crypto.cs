/// Written by: Yulia Danilova
/// Creation Date: 20th of October, 2019
/// Purpose: Handles encryption and decryption of strings using SHA256 
#region ========================================================================= USING =====================================================================================
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
#endregion

namespace Devonia.Infrastructure.Security
{
    public class Crypto
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private const string KEY = "lkirwf897+22#bbtrm8814z5qq=498j5"; // 32 char shared ASCII string (32 * 8 = 256 bit)
        private const string INITIALIZATION_VECTOR = "6#cs!9hjv887mx7@"; // 16 char shared ASCII string (16 * 8 = 128 bit)
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="param">The string to decrypt</param>
        /// <returns>The decrypted string</returns>
        public static string Decrypt(string param)
        {
            RijndaelManaged rijndael = new RijndaelManaged()
            {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 128
            };
            byte[] key = Encoding.ASCII.GetBytes(KEY);
            byte[] IV = Encoding.ASCII.GetBytes(INITIALIZATION_VECTOR);
            ICryptoTransform _decryptor = rijndael.CreateDecryptor(key, IV);
            byte[] sEncrypted = Convert.FromBase64String(param);
            byte[] fromEncrypt = new byte[sEncrypted.Length];
            MemoryStream msDecrypt = new MemoryStream(sEncrypted);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, _decryptor, CryptoStreamMode.Read);
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
            string output = Encoding.ASCII.GetString(fromEncrypt);
            if (output.Contains("\0"))
                output = output.Replace("\0", string.Empty);
            return output;
        }

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="param">The string to exncrypt</param>
        /// <returns>The encrypted string</returns>
        public static string Encrypt(string param)
        {
            RijndaelManaged rijndael = new RijndaelManaged()
            {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 128
            };
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] key = encoding.GetBytes(KEY);
            byte[] IV = encoding.GetBytes(INITIALIZATION_VECTOR);
            ICryptoTransform encryptor = rijndael.CreateEncryptor(key, IV);
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            byte[] toEncrypt = Encoding.ASCII.GetBytes(param);
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();
            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        #endregion
    }
}