/// Written by: Yulia Danilova
/// Creation Date: 19th of November, 2019
/// Purpose: Interface for custom notifications
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
#endregion

namespace Devonia.Infrastructure.Notification
{
    public interface INotificationService
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows a new notification
        /// </summary>
        /// <param name="text">The text to be displayed inside the notification</param>
        /// <returns>A <see cref="System.Nullable{System.Boolean}"/> representing the result of the notification</returns>
        Task<NotificationResult> ShowAsync(string text);

        /// <summary>
        /// Shows a new notification
        /// </summary>
        /// <param name="text">The text to be displayed inside the notification</param>
        /// <param name="caption">The text displayed on the title of the notification</param>
        /// <returns>A <see cref="System.Nullable{System.Boolean}"/> representing the result of the notification</returns>
        Task<NotificationResult> ShowAsync(string text, string caption);

        /// <summary>
        /// Shows a new notification
        /// </summary>
        /// <param name="text">The text to be displayed inside the notification</param>
        /// <param name="caption">The text displayed on the title of the notification</param>
        /// <param name="notificationType">The type of the notification, which determines what buttons are visibile and their captions</param>
        /// <returns>A <see cref="System.Nullable{System.Boolean}"/> representing the result of the notification</returns>
        Task<NotificationResult> ShowAsync(string text, string caption, NotificationButton notificationType);

        /// <summary>
        /// Shows a new notification
        /// </summary>
        /// <param name="text">The text to be displayed inside the notification</param>
        /// <param name="caption">The text displayed on the title of the notification</param>
        /// <param name="notificationType">The type of the notification, which determines what buttons are visibile and their captions</param>
        /// <param name="image">The icon image of the notification</param>
        /// <returns>A <see cref="System.Nullable{System.Boolean}"/> representing the result of the notification</returns>
        Task<NotificationResult> ShowAsync(string text, string caption, NotificationButton notificationType, NotificationImage image);

        /// <summary>
        /// Changes the result of the custom notification
        /// </summary>
        /// <param name="newResult">The new result value</param>
        void ChangeInjectedNotificationResult(bool? newResult);
        #endregion
    }
}
