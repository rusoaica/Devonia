/// Written by: Yulia Danilova
/// Creation Date: 22nd of March, 2020
/// Purpose: Code behind for FluidProgressBar control
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
#endregion

namespace Devonia.Views.Common.Controls
{
    public class FluidProgressBar : UserControl
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public static readonly StyledProperty<int> DotWidthProperty = AvaloniaProperty.Register<FluidProgressBar, int>(nameof(DotWidth), 4);
        public static readonly StyledProperty<int> DotHeightProperty = AvaloniaProperty.Register<FluidProgressBar, int>(nameof(DotHeight), 4);
        public static readonly StyledProperty<bool> OscillateProperty = AvaloniaProperty.Register<FluidProgressBar, bool>(nameof(Oscillate), true);
        public static readonly StyledProperty<int> DelayBetweenCyclesProperty = AvaloniaProperty.Register<FluidProgressBar, int>(nameof(DelayBetweenCycles), 50);
        public static readonly StyledProperty<CornerRadius> DotRadiusProperty = AvaloniaProperty.Register<FluidProgressBar, CornerRadius>(nameof(DotRadius), new CornerRadius(10));
        public static readonly StyledProperty<ISolidColorBrush> DotColorProperty = AvaloniaProperty.Register<FluidProgressBar, ISolidColorBrush>(nameof(DotColor), Brushes.DodgerBlue);

        private int speed = 50;
        private int delayCounter;
        private readonly int dotsMovementOffsetDelay = 200;
        private bool isMovingRight = true;
        private bool isOutsideVisibleArea;
        private readonly Border[] dots = new Border[5];
        private readonly DispatcherTimer animationTimer = new DispatcherTimer();
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        /// <summary>
        /// Gets or sets the radius of the dots
        /// </summary>
        public CornerRadius DotRadius
        {
            get { return GetValue(DotRadiusProperty); }
            set { SetValue(DotRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the dots should reverse their movement each cycle, or not
        /// </summary>
        public bool Oscillate
        {
            get { return GetValue(OscillateProperty); }
            set { SetValue(OscillateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the dots
        /// </summary>
        public int DotWidth
        {
            get { return GetValue(DotWidthProperty); }
            set { SetValue(DotWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the dots 
        /// </summary>
        public int DotHeight
        {
            get { return GetValue(DotHeightProperty); }
            set { SetValue(DotHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the delay between each cycle
        /// </summary>
        public int DelayBetweenCycles
        {
            get { return GetValue(DelayBetweenCyclesProperty); }
            set { SetValue(DelayBetweenCyclesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dots color
        /// </summary>
        public ISolidColorBrush DotColor
        {
            get { return GetValue(DotColorProperty); }
            set { SetValue(DotColorProperty, value); }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public FluidProgressBar()
        {
            AvaloniaXamlLoader.Load(this);
            Background = Brushes.Transparent;
            for (int i = 0; i < dots.Length; i++)
                dots[i] = this.Find<Border>("Dot" + (i + 1));
            AlignDotsAtStart();
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Interval = TimeSpan.FromMilliseconds(50);
            animationTimer.Start();
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the movement offset for the acceleration and deceleration of dots
        /// </summary>
        /// <param name="distanceToCenter">The distance of the dot to the center of the control</param>
        /// <returns>The movement offset in relation to center</returns>
        private double GetLinearMovementOffset(double distanceToCenter)
        {
            return Normalize(distanceToCenter, Bounds.Width) * speed;
        }

        /// <summary>
        /// Aligns all dots to their initial position
        /// </summary>
        private void AlignDotsAtStart()
        {
            foreach (Border border in dots)
                border.Margin = new Thickness(-DotWidth - BorderThickness.Left, 0, 0, 0);
        }

        /// <summary>
        /// Normalizes a specified value between 0 and 1
        /// </summary>
        /// <param name="value">The value to be normalized</param>
        /// <param name="maxValue">The upper limit of the interval which _value can have</param>
        /// <returns>A value between 0 and 1, representing the normalized value of _value parameter</returns>
        public static double Normalize(double value, double maxValue)
        {
            return (1.0 - 0.0) * ((value - 0.0) / (maxValue - 0.0)) + 0.0;
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles AnimationTimer Tick event
        /// </summary>
        private async void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (Bounds.Width > 0)
            {
                double constantAreaWidth = Bounds.Width * 0.25;
                double middle = Bounds.Width / 2;
                double movementStep = GetLinearMovementOffset(Math.Abs(dots[0].Margin.Left - middle));
                double constantMovementStep = (int)Bounds.Width / 75;
                speed = (int)Bounds.Width / 6;
                if (isMovingRight ? dots[0].Margin.Left < Bounds.Width : dots[0].Margin.Left > 0)
                {
                    if (!isOutsideVisibleArea)
                    {
                        bool outsideMiddleArea = dots[0].Margin.Left < middle - constantAreaWidth / 2 || dots[0].Margin.Left > middle + constantAreaWidth / 2;
                        foreach (Border dot in dots)
                        {
                            dot.Margin = new Thickness(dot.Margin.Left - (isMovingRight ? -(outsideMiddleArea ? movementStep : constantMovementStep) : (outsideMiddleArea ? movementStep : constantMovementStep)), 0, 0, 0);
                            await Task.Delay(dotsMovementOffsetDelay);
                        }
                    }
                }
                else
                {
                    isOutsideVisibleArea = true;
                    if (delayCounter < DelayBetweenCycles)
                        delayCounter++;
                    else
                    {
                        delayCounter = 0;
                        isOutsideVisibleArea = false;
                        if (Oscillate)
                            isMovingRight = !isMovingRight;
                        else
                            AlignDotsAtStart();
                    }
                }
            }
        }
        #endregion
    }
}