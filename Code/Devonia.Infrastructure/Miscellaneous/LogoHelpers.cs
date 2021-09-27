/// Written by: Yulia Danilova
/// Creation Date: 11th of July, 2021
/// Purpose: Helper class for logo image files
#region ========================================================================= USING =====================================================================================
using System.IO;
#endregion

namespace Devonia.Infrastructure.Miscellaneous
{
    public static class LogoHelpers
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Returns the path of an existing image that could have different extensions
        /// </summary>
        /// <param name="path">The path to the image</param>
        /// <returns>The path to the image with the existing extension</returns>
        public static string GetPossibleImagePath(string path)
        {
            if (File.Exists(path + ".jpg"))
                return path + ".jpg";
            else if (File.Exists(path + ".jpeg"))
                return path + ".jpeg";
            else if (File.Exists(path + ".png"))
                return path + ".png";
            else if (File.Exists(path + ".bmp"))
                return path + ".bmp";
            else
                return null;
        }
        #endregion
    }
}
