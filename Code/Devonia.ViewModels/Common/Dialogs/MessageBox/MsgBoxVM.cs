/// Written by: Yulia Danilova
/// Creation Date: 08th of November, 2019
/// Purpose: View Model for the custom MessageBox dialog
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using Devonia.Infrastructure.Enums;
using Devonia.ViewModels.Common.MVVM;
using Devonia.ViewModels.Common.Clipboard;
using Devonia.ViewModels.Common.ViewFactory;
using Devonia.Views.Common.Dialogs.MessageBox;
#endregion

namespace Devonia.ViewModels.Common.Dialogs.MessageBox
{
    public class MsgBoxVM : BaseModel, IMsgBoxVM
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IClipboard clipboard;
        private readonly IViewFactory viewFactory;
        #endregion

        #region ============================================================= BINDING COMMANDS ==============================================================================
        public SyncCommand No_Command { get; private set; }
        public SyncCommand Yes_Command { get; private set; }
        public SyncCommand Copy_Command { get; private set; }
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        private bool? dialogResult = null;
        public bool? DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; Notify(); }
        }

        private string yesLabel = string.Empty;
        public string YesLabel
        {
            get { return yesLabel; }
            set { yesLabel = value; Notify(); }
        }

        private string noLabel = string.Empty;
        public string NoLabel
        {
            get { return noLabel; }
            set { noLabel = value; Notify(); }
        }

        private string cancelLabel = string.Empty;
        public string CancelLabel
        {
            get { return cancelLabel; }
            set { cancelLabel = value; Notify(); }
        }

        private string prompt = string.Empty;
        public string Prompt
        {
            get { return prompt; }
            set { prompt = value; Notify(); }
        }

        private string messageBoxIcon = "mb_iconasterisk.png";
        public string MessageBoxIcon
        {
            get { return messageBoxIcon; }
            set { messageBoxIcon = value; Notify(); }
        }

        private bool isNoVisible;
        public bool IsNoVisible
        {
            get { return isNoVisible; }
            set { isNoVisible = value; Notify(); }
        }

        private bool isCancelVisible;      
        public bool IsCancelVisible
        {
            get { return isCancelVisible; }
            set { isCancelVisible = value; Notify(); }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="viewFactory">The injected abstract factory for creating views</param>
        /// <param name="clipboard">The injected clipboard to use</param>
        public MsgBoxVM(IViewFactory viewFactory, IClipboard clipboard)
        {
            No_Command = new SyncCommand(No);
            Yes_Command = new SyncCommand(Yes);
            Copy_Command = new SyncCommand(Copy);
            this.viewFactory = viewFactory;
            this.clipboard = clipboard;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Sets the DialogResult value of the MessageBox to True
        /// </summary>
        private void Yes()
        {
            DialogResult = true;
            CloseView();
        }

        /// <summary>
        /// Sets the DialogResult value of the MessageBox to False
        /// </summary>
        private void No()
        {
            DialogResult = false;
            CloseView();
        }

        /// <summary>
        /// Copies the prompt of the MessageBox dialog into the Cache memory
        /// </summary>
        private void Copy()
        {
            clipboard?.SetText(Prompt);
        }

        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        public async Task<NotificationResult> ShowAsync(string text)
        {
            Prompt = text;
            YesLabel = "OK";
            IsNoVisible = false;
            IsCancelVisible = false;
            // display the message box view
            await viewFactory.CreateView<IMsgBoxView, IMsgBoxVM>(this).ShowDialogAsync();
            return DialogResult == true ? NotificationResult.OK : NotificationResult.None;
        }

        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <param name="caption">The text displayed on the title bar of the MessageBox</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        public async Task<NotificationResult> ShowAsync(string text, string caption)
        {
            Prompt = text;
            YesLabel = "OK";
            IsNoVisible = false;
            IsCancelVisible = false;
            WindowTitle = caption;
            // display the message box view
            await viewFactory.CreateView<IMsgBoxView, IMsgBoxVM>(this).ShowDialogAsync();
            return DialogResult == true ? NotificationResult.OK : NotificationResult.None;
        }

        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <param name="caption">The text displayed on the title bar of the MessageBox</param>
        /// <param name="messageType">The type of the MessageBox, which determines what buttons are visibile and their captions</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        public async Task<NotificationResult> ShowAsync(string text, string caption, NotificationButton messageType)
        {
            switch (messageType)
            {
                case NotificationButton.OK:
                    YesLabel = "OK";
                    IsNoVisible = false;
                    IsCancelVisible = false;
                    break;
                case NotificationButton.YesNo:
                    YesLabel = "Yes";
                    NoLabel = "No";
                    IsNoVisible = true;
                    IsCancelVisible = false;
                    break;
                case NotificationButton.YesNoCancel:
                    YesLabel = "Yes";
                    NoLabel = "No";
                    CancelLabel = "Cancel";
                    IsNoVisible = true;
                    IsCancelVisible = true;
                    break;
                case NotificationButton.OKCancel:
                    YesLabel = "OK";
                    NoLabel = "Cancel";
                    IsNoVisible = true;
                    IsCancelVisible = false;
                    break;
                default:
                    YesLabel = "OK";
                    IsNoVisible = false;
                    IsCancelVisible = false;
                    break;
            }
            Prompt = text;
            WindowTitle = caption;
            // display the message box view
            await viewFactory.CreateView<IMsgBoxView, IMsgBoxVM>(this).ShowDialogAsync();
            switch (messageType)
            {
                case NotificationButton.OK:
                    return DialogResult == true ? NotificationResult.OK : NotificationResult.None;
                case NotificationButton.YesNo:
                    return DialogResult == true ? NotificationResult.Yes : NotificationResult.No;
                case NotificationButton.YesNoCancel:
                    return DialogResult == true ? NotificationResult.Yes : (DialogResult == false ? NotificationResult.No : NotificationResult.Cancel);
                case NotificationButton.OKCancel:
                    return DialogResult == true ? NotificationResult.OK : NotificationResult.Cancel;
                default:
                    return NotificationResult.None;
            }
        }

        /// <summary>
        /// Shows a new instance of the MessageBox dialog
        /// </summary>
        /// <param name="text">The text to be displayed inside the MessageBox</param>
        /// <param name="caption">The text displayed on the title bar of the MessageBox</param>
        /// <param name="messageType">The type of the MessageBox, which determines what buttons are visibile and their captions</param>
        /// <param name="image">The icon image of the MessageBox</param>
        /// <returns>A <see cref="NotificationResult"/> representing the DialogResult of the MessageBox window</returns>
        public async Task<NotificationResult> ShowAsync(string text, string caption, NotificationButton messageType, NotificationImage image)
        {
            switch (messageType)
            {
                case NotificationButton.OK:
                    YesLabel = "OK";
                    IsNoVisible = false;
                    IsCancelVisible = false;
                    break;
                case NotificationButton.YesNo:
                    YesLabel = "Yes";
                    NoLabel = "No";
                    IsNoVisible = true;
                    IsCancelVisible = false;
                    break;
                case NotificationButton.YesNoCancel:
                    YesLabel = "Yes";
                    NoLabel = "No";
                    CancelLabel = "Cancel";
                    IsNoVisible = true;
                    IsCancelVisible = true;
                    break;
                case NotificationButton.OKCancel:
                    YesLabel = "OK";
                    NoLabel = "Cancel";
                    IsNoVisible = true;
                    IsCancelVisible = false;
                    break;
                default:
                    YesLabel = "OK";
                    IsNoVisible = false;
                    IsCancelVisible = false;
                    break;
            }
            switch (image)
            {
                case NotificationImage.Asterisk:
                    MessageBoxIcon = "mb_iconasterisk.png";
                    break;
                case NotificationImage.Exclamation:
                    MessageBoxIcon = "mb_iconexclamation.png";
                    break;
                case NotificationImage.Hand:
                    MessageBoxIcon = "mb_iconhand.png";
                    break;
                case NotificationImage.Question:
                    MessageBoxIcon = "mb_iconquestion.png";
                    break;
                case NotificationImage.None:
                default:
                    MessageBoxIcon = "mb_iconasterisk.png";
                    break;
            }
            Prompt = text;
            WindowTitle = caption;
            // display the message box view
            await viewFactory.CreateView<IMsgBoxView, IMsgBoxVM>(this).ShowDialogAsync();
            switch (messageType)
            {
                case NotificationButton.OK:
                    return DialogResult == true ? NotificationResult.OK : NotificationResult.None;
                case NotificationButton.YesNo:
                    return DialogResult == true ? NotificationResult.Yes : NotificationResult.No;
                case NotificationButton.YesNoCancel:
                    return DialogResult == true ? NotificationResult.Yes : (DialogResult == false ? NotificationResult.No : NotificationResult.Cancel);
                case NotificationButton.OKCancel:
                    return DialogResult == true ? NotificationResult.OK : NotificationResult.Cancel;
                default:
                    return NotificationResult.None;
            }
        }
        #endregion
    }
}
