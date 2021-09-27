/// Written by: Yulia Danilova
/// Creation Date: 11th of October, 2019
/// Purpose: Base class for view models, containing additional functionality for UI
#region ========================================================================= USING =====================================================================================
using Devonia.Infrastructure.Notification;
using Devonia.ViewModels.Common.Dispatcher;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
#endregion

namespace Devonia.ViewModels.Common.MVVM
{
    public class BaseModel : IBaseModel
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event EventHandler ClosingView ;
        public event EventHandler HidingView ;
        public event EventHandler ShowingView ;
        public event PropertyChangedEventHandler PropertyChanged;

        protected IDispatcher dispatcher;
        protected INotificationService notificationService;
        protected bool isMediaPlaying;
        private readonly Timer mediaPlayingIndicatorDispatcher = new Timer(500);
        #endregion

        #region ============================================================ BINDING PROPERTIES =============================================================================
        private string windowTitle = string.Empty;
        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; Notify(); }
        }

        private double titleFontSize = 20;
        public double TitleFontSize
        {
            get { return titleFontSize; }
            set { titleFontSize = value; Notify(); }
        }

        private bool isMediaPlayingIndicatorVisible;
        public bool IsMediaPlayingIndicatorVisible
        {
            get { return isMediaPlayingIndicatorVisible; }
            set 
            {
                if (!isMediaPlayingIndicatorSocketVisible)
                    isMediaPlayingIndicatorVisible = false;
                else
                    isMediaPlayingIndicatorVisible = value;
                Notify(); 
            }
        }

        private bool isMediaPlayingIndicatorSocketVisible;
        public bool IsMediaPlayingIndicatorSocketVisible
        {
            get { return isMediaPlayingIndicatorSocketVisible; }
            set { isMediaPlayingIndicatorSocketVisible = value; Notify(); }
        }

        private bool isProgressBarVisible;
        public bool IsProgressBarVisible
        {
            get { return isProgressBarVisible; }
            set { isProgressBarVisible = value; Notify(); }
        }

        private bool isHelpButtonVisible;
        public bool IsHelpButtonVisible
        {
            get { return isHelpButtonVisible; }
            set { isHelpButtonVisible = value; Notify(); }
        }

        private string mediaPlayingImage;
        public string MediaPlayingImage
        {
            get { return mediaPlayingImage; }
            set { mediaPlayingImage = value; Notify(); }
        }

        private DateTime currentTime = DateTime.Now;
        public DateTime CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; Notify(); }
        }
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public string WindowHelp { get; set; } = string.Empty;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public BaseModel()
        {
            mediaPlayingImage = "green.png";
            mediaPlayingIndicatorDispatcher.Elapsed += MediaPlayingIndicatorDispatcher_Tick;
            mediaPlayingIndicatorDispatcher.Start();
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Notifies the UI about a binded property's value being changed
        /// </summary>
        /// <param name="propertyName">The property that had the value changed</param>
        public void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Event handler for closing Views
        /// </summary>
        protected void CloseView()
        {
            ClosingView?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event handler for hiding views
        /// </summary>
        protected void HideView()
        {
            HidingView?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event handler for showing views
        /// </summary>
        protected void ShowView()
        {
            ShowingView?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Executes the code that needs to be ran when the help button of a view is clicked
        /// </summary>
        public virtual async Task ShowHelpAsync()
        {
            await notificationService?.ShowAsync(WindowHelp, "Devonia - Help");
        }

        /// <summary>
        /// Shows the progress bar that indicates busy operation
        /// </summary>
        protected void ShowProgressBar()
        {
            IsProgressBarVisible = true;
        }

        /// <summary>
        /// Hides the progress bar that indicates busy operation
        /// </summary>
        protected void HideProgressBar()
        {
            IsProgressBarVisible = false;
        }

        /// <summary>
        /// Shows the help button
        /// </summary>
        protected void ShowHelpButton()
        {
            IsHelpButtonVisible = true;
        }

        /// <summary>
        /// Hides the help button
        /// </summary>
        protected void HideHelpButton()
        {
            IsHelpButtonVisible = false;
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles the blinking of the image that indicates whether a media file is playing or not
        /// </summary>
        private void MediaPlayingIndicatorDispatcher_Tick(object sender, ElapsedEventArgs e)
        {
            MediaPlayingImage = (isMediaPlaying ? "red" : "green") + ".png";
            IsMediaPlayingIndicatorVisible = !IsMediaPlayingIndicatorVisible;
            CurrentTime = DateTime.Now;
        }
        #endregion
    }
}
