/// Written by: Yulia Danilova
/// Creation Date: 15th of October, 2021
/// Purpose: Code behind for the path navigator item user control
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls;
using System.Diagnostics;
using Avalonia.Markup.Xaml;
#endregion

namespace Devonia.Views.Common.Controls
{
    public class PathNavigatorItem : UserControl
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event Action<PathNavigatorItem> PathClicked;
        public event Action<PathNavigatorItem> PathExpanded;
        
        private readonly Image imgIcon;
        private readonly Label lblText;
        private readonly Panel pnlRootExpander;
        private static readonly IBrush arrowBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#777777");
        
        private static GeometryDrawing geometryRight = new GeometryDrawing()
        {
            Geometry = Application.Current.Resources["RightArrowGeometry"] as Geometry,
            Brush = arrowBrush
        };
        private DrawingImage imageSourceArrowRight = new DrawingImage() { Drawing = geometryRight };
        private static GeometryDrawing geometryDown = new GeometryDrawing()
        {
            Geometry = Application.Current.Resources["DownArrowGeometry"] as Geometry,
            Brush = arrowBrush
        };
        private DrawingImage imageSourceArrowDown = new DrawingImage() { Drawing = geometryDown };
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public int Id { get; set; }
        
        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                if (imgIcon != null)
                    imgIcon.Source = value ? imageSourceArrowDown : imageSourceArrowRight;
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                if (lblText != null)
                    lblText.Content = value;
            }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public PathNavigatorItem()
        {
            AvaloniaXamlLoader.Load(this);
            pnlRootExpander = this.FindControl<Panel>("pnlRootExpander");
            imgIcon = this.FindControl<Image>("imgIcon");
            lblText = this.FindControl<Label>("lblText");
        }
        #endregion
        
        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles navigator item's PointerEnter event
        /// </summary>
        private void NavigatorItem_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            pnlRootExpander.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#33FFFFFF");
            lblText.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#33FFFFFF");
        }
        
        /// <summary>
        /// Handles navigator item's PointerLeave event
        /// </summary>
        private void NavigatorItem_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            pnlRootExpander.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#00FFFFFF");
            lblText.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#00FFFFFF");
        }

        /// <summary>
        /// Handles navigator item's PointerReleased event
        /// </summary>
        private void NavigatorItem_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            PathClicked?.Invoke(this);
            e.Handled = true;
        }
        
        /// <summary>
        /// Handles navigator expander's PointerReleased event
        /// </summary>
        private void Expander_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            IsExpanded = !isExpanded;
            PathExpanded?.Invoke(this);
            e.Handled = true;
        }
        #endregion
    }
}