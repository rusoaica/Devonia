/// Written by: Yulia Danilova
/// Creation Date: 04th of July, 2021
/// Purpose: Helper class for determining types of files
#region ========================================================================= USING =====================================================================================
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
#endregion

namespace Devonia.Infrastructure.Miscellaneous
{
    public static class FileHelpers
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Tests if a file is a pdf file
        /// </summary>
        /// <param name="path">The path of the file to test</param>
        /// <returns>True if the file is a pdf file, False otherwise</returns>
        public static bool IsPdfFile(string path)
        {
            string pdfString = "%PDF-";
            byte[] pdfBytes = Encoding.ASCII.GetBytes(pdfString);
            int length = pdfBytes.Length;
            byte[] buffer = new byte[length];
            int remaining = length;
            int position = 0;
            using (FileStream stream = File.OpenRead(path))
            {
                while (remaining > 0)
                {
                    int amountRead = stream.Read(buffer, position, remaining);
                    if (amountRead == 0) 
                        return false;
                    remaining -= amountRead;
                    position += amountRead;
                }
            }
            return pdfBytes.SequenceEqual(buffer);
        }

        /// <summary>
        /// Tests if a file is a binary file or a text one
        /// </summary>
        /// <param name="path">The path of the file to test</param>
        /// <returns>True if the file is a binary file, False otherwise</returns>
        public static bool IsBinaryFile(string path)
        {
            long length = new FileInfo(path).Length;
            if (length == 0) 
                return false;
            using (StreamReader stream = new StreamReader(path))
            {
                int character;
                while ((character = stream.Read()) != -1)
                    if (IsControlChar(character))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if <paramref name="fileBytes"/> headers belong to known image headers
        /// </summary>
        /// <param name="fileBytes">The bytes containing the headers to check</param>
        /// <returns>True if any known image header matches, False otherwise</returns>
        public static bool IsImage(byte[] fileBytes)
        {
            List<byte[]> headers = new List<byte[]>
            {
                Encoding.ASCII.GetBytes("BM"),      // BMP
                Encoding.ASCII.GetBytes("GIF"),     // GIF
                new byte[] { 137, 80, 78, 71 },     // PNG
                new byte[] { 73, 73, 42 },          // TIFF
                new byte[] { 77, 77, 42 },          // TIFF
                new byte[] { 255, 216, 255, 224 },  // JPEG
                new byte[] { 255, 216, 255, 225 },  // JPEG CANON
                new byte[] { 255, 216, 255, 226 }   // JPEG UNKNOWN
            };
            return headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length)));
        }

        /// <summary>
        /// Tests if <paramref name="character"/> is a control char or not
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns>True if <paramref name="character"/> is a control char, False otherwise</returns>
        private static bool IsControlChar(int character)
        {
            return (character > Chars.NUL && character < Chars.BS) || (character > Chars.CR && character < Chars.SUB);
        }

        private static class Chars
        {
            public static char NUL = (char)0; // Null char
            public static char BS = (char)8; // Back Space
            public static char CR = (char)13; // Carriage Return
            public static char SUB = (char)26; // Substitute
        }
        #endregion
    }
}
