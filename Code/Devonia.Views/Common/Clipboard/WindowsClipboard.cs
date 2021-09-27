/// Written by: Yulia Danilova
/// Creation Date: 09th of November, 2020
/// Purpose: Explicit implementation of abstract IClipboard interface, used in UI environments
#region ========================================================================= USING =====================================================================================
using Devonia.ViewModels.Common.Clipboard;
#endregion

namespace Devonia.Views.Common.Clipboard
{
    public class WindowsClipboard : IClipboard
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Sets the <paramref name="text"/> string into Windows clipboard memory
        /// </summary>
        /// <param name="text">The string to be set into Windows clipboard</param>
        public void SetText(string text)
        {
            // TODO: add platform conditional compilation
            //System.Windows.Clipboard.SetText(text);
        }
        #endregion
    }
}
