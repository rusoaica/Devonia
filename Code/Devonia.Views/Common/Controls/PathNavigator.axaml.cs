/// Written by: Yulia Danilova
/// Creation Date: 15th of October, 2021
/// Purpose: Custom user control for a file explorer navigation's bar
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#endregion

namespace Devonia.Views.Common.Controls
{
    public class PathNavigator : UserControl
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event Func<string, bool, Task> NavigatorLocationChanged;
        public event Action<PathNavigatorItem> PathExpanded;
        public event Action HistoryExpanded;

        private readonly Image imgIcon;
        private readonly TextBox txtPath;
        private readonly Panel pnlRootExpander;
        private readonly StackPanel pnlContainer;
        #endregion
        
        #region ================================================================ PROPERTIES =================================================================================
        private string iconSource;
      
        public string IconSource
        {
            get { return iconSource; }
            set
            {
                iconSource = value;
                if (imgIcon != null)
                {
                    if (!string.IsNullOrEmpty(value))
                        imgIcon.Source = new Bitmap(iconSource);
                    else
                        imgIcon.Source = Application.Current.Resources["FolderBitmap"] as Bitmap;
                }
            }
        }

        public string CurrentPath
        {
            get { return txtPath.Text; }
        }

        private bool isHistoryExpanded;
        public bool IsHistoryExpanded
        {
            get { return isHistoryExpanded; }
            set { isHistoryExpanded = value; }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public PathNavigator()
        {
            AvaloniaXamlLoader.Load(this);
            imgIcon = this.FindControl<Image>("imgIcon");
            txtPath = this.FindControl<TextBox>("txtPath");
            pnlContainer = this.FindControl<StackPanel>("pnlContainer");
            pnlRootExpander = this.FindControl<Panel>("pnlRootExpander");
        }
        #endregion
    
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets the concatenated path starting from the root element and up to (and including) <paramref name="sender"/>
        /// </summary>
        /// <param name="sender">The path navigator item that ends the returned path, starting from root</param>
        /// <returns>A path that starts at the root element and continues up to (and including) <paramref name="sender"/></returns>
        public string GetInternalPath(PathNavigatorItem sender)
        {
            OperatingSystemType OS = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem;
            return OS == OperatingSystemType.WinNT
                ? string.Join(Path.DirectorySeparatorChar, pnlContainer.Children.Select(control => control as PathNavigatorItem)
                                                                                .Where(item => item.Id <= sender.Id)
                                                                                .Select(item => item.Text))
                : Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar, pnlContainer.Children.Select(control => control as PathNavigatorItem)
                                                                                                              .Where(item => item.Id <= sender.Id)
                                                                                                              .Select(item => item.Text));
        }

        /// <summary>
        /// Sets the expanders of all children path navigator items to collapsed
        /// </summary>
        public void CollapseAllExpanders()
        {
            foreach (PathNavigatorItem item in pnlContainer.Children.OfType<PathNavigatorItem>())
                item.IsExpanded = false;
        }
        
        /// <summary>
        /// Clears the path navigation child nodes and re-draws a new one for each directory of <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path for which to draw the child path navigation nodes</param>
        /// <param name="isRefresh">Indicates whether the navigation is done to the same folder or not (refreshing location)</param>
        /// <exception cref="DirectoryNotFoundException">Exception thrown when <paramref name="path"/> does not exist</exception>
        public async Task NavigateToPath(string path, bool isRefresh = false)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Invalid directory path!");
            // paths should always end with separator character
            if (!path.EndsWith(Path.DirectorySeparatorChar))
                path += Path.DirectorySeparatorChar;
            // if the user was in manual path editing, exit it
            txtPath.Text = path;
            txtPath.IsVisible = false;
            // beware deferred execution - collection must be delivered from start, otherwise it will throw exceptions about collection being modified
            foreach (PathNavigatorItem disposableItem in pnlContainer.Children.ToArray())
            {
                disposableItem.PathClicked -= NavigationItemClicked;
                disposableItem.PathExpanded -= NavigationItemExpanded;
                pnlContainer.Children.Remove(disposableItem);
            }
            pnlContainer.Children.Clear();

            // get a list of directories composing the provided path
            string[] directories = path.Split(Path.DirectorySeparatorChar)
                                       .Where(dir => !string.IsNullOrWhiteSpace(dir))
                                       .ToArray();
            // for each directory in provided path, create a new path navigator control item
            for (int i = 0; i < directories.Count(); i++)
            {
                PathNavigatorItem item = new PathNavigatorItem();
                item.Text = directories[i];
                item.PathClicked += NavigationItemClicked;
                item.PathExpanded += NavigationItemExpanded;
                item.Id = i;
                pnlContainer.Children.Add(item);
            }
            IsHistoryExpanded = false;
            // signal any subscribers of the navigator path having been changed
            await NavigatorLocationChanged?.Invoke(path, isRefresh);
        }
        #endregion
        
        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Hanldes the root item expander's PointerEnter event
        /// </summary>
        private void RootExpander_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            (sender as Panel)!.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#33FFFFFF");
        }
        
        /// <summary>
        /// Hanldes the root item expander's PointerLeave event
        /// </summary>
        private void RootExpander_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            (sender as Panel)!.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00FFFFFF");
        }

        /// <summary>
        /// Handles root icon's PointerReleased event
        /// </summary>
        private void Icon_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            // when the icon is clicked, enter the manual path editing mode
            if (!txtPath.IsVisible)
            {
                txtPath.IsVisible = true;
                txtPath.SelectAll();
                txtPath.Focus();
            }
        }

        /// <summary>
        /// Handles path textbox'es KeyUp event
        /// </summary>
        private async void TextBoxPath_OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) // exit manual path editing mode
                txtPath.IsVisible = false;
            else if (e.Key == Key.Enter) // navigate to the manual path
                await NavigateToPath(txtPath.Text);
        }

        /// <summary>
        /// Handles path textbox'es LostFocus event
        /// </summary>
        private void TextBoxPath_OnLostFocus(object? sender, RoutedEventArgs e)
        {
            txtPath.IsVisible = false;
        }

        /// <summary>
        /// Handles the Click event of <paramref name="sender"/> child path navigator control
        /// </summary>
        /// <param name="sender">The child path navigator control that was clicked</param>
        private async void NavigationItemClicked(PathNavigatorItem sender)
        {
            await NavigateToPath(GetInternalPath(sender));
        }

        /// <summary>
        /// Handles the PathExpanded event of <paramref name="sender"/> child path navigator control
        /// </summary>
        /// <param name="sender">The child path navigator that was expanded</param>
        private void NavigationItemExpanded(PathNavigatorItem sender)
        {
            PathExpanded?.Invoke(sender);
        }

        /// <summary>
        /// Handles the refresh button's Click event
        /// </summary>
        private async void RefreshLocation_Click(object? sender, RoutedEventArgs e)
        {
            await NavigateToPath(txtPath.Text, true);
        }

        /// <summary>
        /// Handles the history button's Click event
        /// </summary>
        private void ShowHistory_Click(object? sender, RoutedEventArgs e)
        {
            // needs sizes so that the drop down folders panel is positioned just under the expanded path item
            IsHistoryExpanded = !IsHistoryExpanded;
            HistoryExpanded?.Invoke();
        }
        #endregion
    }
}