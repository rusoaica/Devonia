/// Written by: Yulia Danilova
/// Creation Date: 15th of November, 2020
/// Purpose: Code behind for the Marquee user control
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
#endregion

namespace Devonia.Views.Common.Controls
{
    public partial class Marquee : UserControl, INotifyPropertyChanged
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public new event PropertyChangedEventHandler? PropertyChanged;

        private int counter;
        private int textLength;
        private int currentCharacter;
        private bool isReverseDirection;
        private string internalText = string.Empty;
        private readonly Grid container;
        private readonly TextBlock txtMain;
        private readonly DispatcherTimer animationTimer = new DispatcherTimer();
        #endregion

        #region ============================================================ BINDING PROPERTIES ============================================================================= 
        private string text = string.Empty;
        public string Text
        {
            get { return text; }
            set { text = value; Notify(nameof(Text)); }
        }
        #endregion

        #region ========================================================== DEPENDENCY PROPERTIES ============================================================================
        public static readonly DirectProperty<Marquee, string> SetTextProperty = AvaloniaProperty.RegisterDirect<Marquee, string>(nameof(SetText), e => e.SetText, OnSetTextChanged);

        private string setText;
        public string SetText
        {
            get { return setText; }
            set { SetAndRaise(SetTextProperty, ref setText, value); }
        }
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        private bool IsTextTrimmed => MeasureString(internalText).Width > Bounds.Width - 10;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public Marquee()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            txtMain = this.Find<TextBlock>("txtMain");
            container = this.FindControl<Grid>("container");
            internalText = Text;
            container.Width = Width;
            container.Height = Height;
            PointerEnter += Marquee_PointerEnter;
            PointerLeave += Marquee_PointerLeave;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Interval = TimeSpan.FromSeconds(0.3);
            BoundsProperty.Changed.AddClassHandler<Window>((s, e) => UserControl_SizeChanged());
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Notifies the UI about a binded property's value being changed
        /// </summary>
        /// <param name="propertyName">The property that had the value changed</param>
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Text Changed dependency property handler
        /// </summary>
        private static void OnSetTextChanged(Marquee control, string value)
        {
            control.OnSetTextChanged(value);
        }

        /// <summary>
        /// Internal Text Changed dependency property handler
        /// </summary>
        private void OnSetTextChanged(string e)
        {
            if (e != null)
            {
                Text = e;
                SetTextInternal(e);
            }
        }

        /// <summary>
        /// Sets the internal text of the control
        /// </summary>
        /// <param name="text">The text to set</param>
        private void SetTextInternal(string text)
        {
            internalText = text;
            ArrangeTextLength();
        }

        /// <summary>
        /// Measures the size of <paramref name="candidate"/> with a specific font
        /// </summary>
        /// <param name="candidate">The text to measure</param>
        /// <returns>The size of <paramref name="candidate"/></returns>
        private Size MeasureString(string candidate)
        {
            FormattedText formattedText = new FormattedText(
                candidate, 
                new Typeface(txtMain.FontFamily, txtMain.FontStyle, txtMain.FontWeight), 
                txtMain.FontSize,
                txtMain.TextAlignment,
                txtMain.TextWrapping, Bounds.Size);
            return new Size(formattedText.Bounds.Width, formattedText.Bounds.Height);
        }

        /// <summary>
        /// Trims the visible text to the visible size of the control area and adds trimming dots if needed
        /// </summary>
        private void ArrangeTextLength()
        {
            // check if the text needs trimming
            if (IsTextTrimmed)
            {
                // store a temporary string to be used as the control's text
                string temp = string.Empty;
                // cycle all the characters of the internal text
                for (int i = 0; i < internalText.Length; i++)
                {
                    // if the size of the temporary string plus the trimming dots fits in the visible area of the control, add another character from the internal text
                    if (MeasureString(temp + "...").Width < Bounds.Width - 10)
                        temp += internalText[i];
                    else // no more characters can be added to fit in the visible area of the control, set the control's text to the temporary one plus the trimming dots
                    {
                        Text = temp + "...";
                        break;
                    }
                }
            }
            else // control's text fits, no need for trimming
                Text = internalText;
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles animation timer Tick event
        /// </summary>
        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                // check if there is a control text set
                if (internalText.Length > 0)
                {
                    // check if the text goes from right to left or viceversa
                    if (!isReverseDirection)
                    {
                        // while control's text doesn't reach the end of the internal text, set it to a substring of internal text with an incrementing starting index, else, reverse text scroll direction
                        if (currentCharacter + textLength - 3 < internalText.Length)
                            Text = internalText.Substring(++currentCharacter, textLength - 3) + (internalText.Substring(0, currentCharacter).Length + internalText.Substring(currentCharacter, textLength - 3).Length < internalText.Length ? "..." : string.Empty);
                        else
                            isReverseDirection = true;
                    }
                    else
                    {
                        // when the direction is reversed, increment a counter for 10 ticks, as pause
                        if (counter++ > 10)
                        {
                            // while control's text doesn't reach the start of the internal text, set it to a substring of internal text with an decrementing starting index, else, reverse text scroll direction
                            if (currentCharacter > 0)
                                Text = internalText.Substring(--currentCharacter, textLength - 3) + (internalText.Substring(0, currentCharacter).Length + internalText.Substring(currentCharacter, textLength - 3).Length < internalText.Length ? "..." : string.Empty);
                            else
                            {
                                counter = 0;
                                isReverseDirection = false;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Handles user control MouseEnter event
        /// </summary>
        private void Marquee_PointerEnter(object? sender, PointerEventArgs e)
        {
            // set the index of the current internal text subsctring index back to 0
            currentCharacter = 0;
            isReverseDirection = false;
            // store the actual displayed length of the text
            textLength = Text.Length;
            // start the scrolling timer
            animationTimer.Start();
        }

        /// <summary>
        /// Handles user control MouseLeave event
        /// </summary>
        private void Marquee_PointerLeave(object? sender, PointerEventArgs e)
        {
            // stop the scrolling timer
            animationTimer.Stop();
            // rearenge the text 
            ArrangeTextLength();
        }

        /// <summary>
        /// Handles user control SizeChanged event
        /// </summary>
        private void UserControl_SizeChanged()
        {
            ArrangeTextLength();
        }
        #endregion
    }
}
