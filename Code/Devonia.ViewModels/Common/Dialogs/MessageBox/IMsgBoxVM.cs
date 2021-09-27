/// Written by: Yulia Danilova
/// Creation Date: 10th of November, 2020
/// Purpose: Interface for the view model for the message box view
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.ViewModels.Common.MVVM;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.MessageBox
{
    public interface IMsgBoxVM : IBaseModel
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        Task<NotificationResult> ShowAsync(string text);

        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <param name="caption">The text displayed on the title bar of the MessageBox</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        Task<NotificationResult> ShowAsync(string text, string caption);

        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <param name="caption">The text displayed on the title bar of the MessageBox</param>
        /// <param name="messageType">The type of the MessageBox, which determines what buttons are visibile and their captions</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        Task<NotificationResult> ShowAsync(string text, string caption, NotificationButton messageType);

        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <param name="caption">The text displayed on the title bar of the MessageBox</param>
        /// <param name="messageType">The type of the MessageBox, which determines what buttons are visibile and their captions</param>
        /// <param name="image">The icon image of the MessageBox</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        Task<NotificationResult> ShowAsync(string text, string caption, NotificationButton messageType, NotificationImage image);
        #endregion
    }
}
