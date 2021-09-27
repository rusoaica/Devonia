/// Written by: Yulia Danilova
/// Creation Date: 19th of November, 2019
/// Purpose: Explicit implementation of abstract custom message box service
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.Infrastructure.Notification;
using Devonia.ViewModels.Common.Dispatcher;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.MessageBox
{
    /// <summary>
    /// A service that shows message boxes
    /// </summary>
    public class MessageBoxService : INotificationService
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IDispatcher dispatcher;
        private readonly Func<IMsgBoxVM> msgBoxVM;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use</param>
        /// <param name="msgBoxVM">A Func that will force the creation of a new messagebox viewmodel instance on each call</param>
        /// <remarks>When using simple constructor injection, the messagebox instance is created only once, and can no longer trigger new views after the 
        /// first one is disposed, therefor, the need of a new viewmodel on each call. Method injection could have been an approach too, but that would 
        /// require changing the signatures of the Show() methods in the interface, which is unacceptable. Service Locator pattern is also unacceptable, 
        /// and the viewmodel is not aware of a DI container anyway. Sending the DI container as injected parameter is definitely unacceptable.</remarks>
        public MessageBoxService(IDispatcher dispatcher, Func<IMsgBoxVM> msgBoxVM)
        {
            this.dispatcher = dispatcher;
            this.msgBoxVM = msgBoxVM;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Changes the dialog result of the custom MessageBox
        /// </summary>
        /// <param name="newDialogResult">The new dialog result value</param>
        public void ChangeInjectedNotificationResult(bool? newDialogResult)
        {
            // MessageBoxService dialog results are set from UI interraction!
            throw new NotSupportedException("Dialog results are set from UI only!");
        }

        /// <summary>
        /// Shows a new custom MessageBox
        /// </summary>
        /// <param name="message">The text to display in the custom MessageBox</param>
        /// <returns>A <see cref="NotificationResult"/> value representing the result of the custom MessageBox dialog</returns>
        public async Task<NotificationResult> ShowAsync(string message)
        {
            return await await dispatcher?.DispatchAsync(new Func<Task<NotificationResult>>(async () =>
                 await msgBoxVM.Invoke().ShowAsync(message)
            ));
        }

        /// <summary>
        /// Shows a new custom MessageBox
        /// </summary>
        /// <param name="message">The text to display in the custom MessageBox</param>
        /// <param name="title">The Title of the custom MessageBox dialog</param>
        /// <returns>A <see cref="NotificationResult"/> value representing the result of the custom MessageBox dialog</returns>
        public async Task<NotificationResult> ShowAsync(string message, string title)
        {
            return await await dispatcher?.DispatchAsync(new Func<Task<NotificationResult>>(async () =>
                await msgBoxVM.Invoke().ShowAsync(message, title)
            ));
        }

        /// <summary>
        /// Shows a new custom MessageBox
        /// </summary>
        /// <param name="message">The text to display in the custom MessageBox</param>
        /// <param name="title">The Title of the custom MessageBox dialog</param>
        /// <param name="buttons">The buttons displayed inside the custom MessageBox dialog</param>
        /// <returns>A <see cref="NotificationResult"/> value representing the result of the custom MessageBox dialog</returns>
        public async Task<NotificationResult> ShowAsync(string message, string title, NotificationButton buttons)
        {
            return await await dispatcher?.DispatchAsync(new Func<Task<NotificationResult>>(async () =>
                await msgBoxVM.Invoke().ShowAsync(message, title, buttons)
            ));
        }

        /// <summary>
        /// Shows a new custom MessageBox
        /// </summary>
        /// <param name="message">The text to display in the custom MessageBox</param>
        /// <param name="title">The Title of the custom MessageBox dialog</param>
        /// <param name="buttons">The buttons displayed inside the custom MessageBox dialog</param>
        /// <param name="image">The icon of the custom MessageBox dialog</param>
        /// <returns>A <see cref="NotificationResult"/> value representing the result of the custom MessageBox dialog</returns>
        public async Task<NotificationResult> ShowAsync(string message, string title, NotificationButton buttons, NotificationImage image)
        {
            return await await dispatcher?.DispatchAsync(new Func<Task<NotificationResult>>(async () =>
                 await msgBoxVM.Invoke().ShowAsync(message, title, buttons, image)
            ));
        }
        #endregion
    }
}
