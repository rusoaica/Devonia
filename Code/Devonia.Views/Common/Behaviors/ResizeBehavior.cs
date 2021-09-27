/// Written by: Yulia Danilova
/// Creation Date: 31st of July, 2021
/// Purpose: Behavior for windows resizing
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
#endregion

namespace Devonia.Views.Common.Behaviors
{
    public class ResizeBehavior : AvaloniaObject
    {
        #region ============================================================ ATTACHED PROPERTIES ============================================================================ 
        /// <summary>
        /// Identifies the <seealso cref="CommandProperty"/> avalonia attached property.
        /// </summary>
        /// <value>Provide an <see cref="string"/> derived object or binding.</value>
        public static readonly AttachedProperty<string> CommandProperty = AvaloniaProperty.RegisterAttached<ResizeBehavior, Interactive, string>(
            "Command", default!, false, BindingMode.OneTime);

        /// <summary>
        /// Identifies the <seealso cref="CommandParameterProperty"/> avalonia attached property.
        /// Use this as the parameter for the <see cref="CommandProperty"/>.
        /// </summary>
        /// <value>Any value of type <see cref="object"/>.</value>
        public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<ResizeBehavior, Interactive, object>(
            "CommandParameter", default!, false, BindingMode.OneWay, null);
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Static C-tor
        /// </summary>
        static ResizeBehavior()
        {
            CommandProperty.Changed.Subscribe(p => HandleCommandChanged(p.Sender));
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// <see cref="CommandProperty"/> changed event handler.
        /// </summary>
        /// <param name="element">The UI element that initiated the resize command</param>
        private static void HandleCommandChanged(IAvaloniaObject element)
        {
            if (element is Interactive interactElem)
                interactElem.AddHandler(InputElement.PointerPressedEvent, Handler);
            // local handler function
            void Handler(object? s, PointerPressedEventArgs e)
            {
                // get the parameter off of the gui element
                Window window = interactElem.GetValue(CommandParameterProperty) as Window;
                Border b = s as Border;
                if (b.Name == "Left")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.West, e);
                else if (b.Name == "Right")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.East, e);
                else if (b.Name == "Top")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.North, e);
                else if (b.Name == "Bottom")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.South, e);
                else if (b.Name == "TopLeft")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.NorthWest, e);
                else if (b.Name == "TopRight")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.NorthEast, e);
                else if (b.Name == "BottomLeft")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.SouthWest, e);
                else if (b.Name == "BottomRight")
                    window.PlatformImpl?.BeginResizeDrag(WindowEdge.SouthEast, e);
            }
        }

        /// <summary>
        /// Accessor for Attached property <see cref="CommandProperty"/>.
        /// </summary>
        public static void SetCommand(AvaloniaObject element, string commandValue)
        {
            element.SetValue(CommandProperty, commandValue);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="CommandProperty"/>.
        /// </summary>
        public static string GetCommand(AvaloniaObject element)
        {
            return element.GetValue(CommandProperty);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="CommandParameterProperty"/>.
        /// </summary>
        public static void SetCommandParameter(AvaloniaObject element, object parameter)
        {
            element.SetValue(CommandParameterProperty, parameter);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="CommandParameterProperty"/>.
        /// </summary>
        public static object GetCommandParameter(AvaloniaObject element)
        {
            return element.GetValue(CommandParameterProperty);
        }
        #endregion
    }
}
