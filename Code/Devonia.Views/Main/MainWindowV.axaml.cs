/// Written by: Yulia Danilova
/// Creation Date: 27th of September, 2021
/// Purpose: Code behind for the MainWindowV view

#region ========================================================================= USING =====================================================================================

using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Devonia.Views.Startup;
using System.Threading.Tasks;
using Devonia.ViewModels.Main;
using Devonia.Infrastructure.Configuration;
using Avalonia.Interactivity;
using Avalonia.Input;
using Devonia.Infrastructure.Enums;
using System.Collections.Generic;
using Devonia.ViewModels.Common.Models;
using System.Linq;
using Devonia.Models.Common.Models.Common;
using Devonia.Views.Common.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls.Presenters;
using Avalonia.Media;

#endregion

namespace Devonia.Views.Main
{
    public partial class MainWindowV : Window, IMainWindowView
    {
        #region ============================================================== FIELD MEMBERS ================================================================================

        private bool isWindowLoaded;
        private readonly IAppConfig appConfig;
        private readonly Grid grdContainer;
        private readonly Grid modalGrid;
        private readonly Grid grdPreview;
        private readonly GridSplitter spPreview;
        private readonly Border grdNewFolder;
        private readonly TextBox txtNewFolderName;
        private readonly ListBox lstNavigationDirectories;
        private readonly BrushConverter brushConverter = new();
        private FileSystemExplorer fileExplorerControl;

        private readonly PathNavigator navigator;
        #endregion

        #region ================================================================== CTOR =====================================================================================

        /// <summary>
        /// Default C-tor
        /// </summary>
        public MainWindowV()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="appConfig">Injected application's configuration service</param>
        public MainWindowV(IAppConfig appConfig)
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
            this.appConfig = appConfig;
#endif
            navigator = this.FindControl<PathNavigator>("txtNavigator");
            fileExplorerControl = this.FindControl<FileSystemExplorer>("lstDirectories");
            grdPreview = this.FindControl<Grid>("grdPreview");
            grdContainer = this.FindControl<Grid>("grdContainer");
            spPreview = this.FindControl<GridSplitter>("spPreview");
            grdNewFolder = this.FindControl<Border>("grdNewFolder");
            modalGrid = this.FindControl<Grid>("modalGrid");
            txtNewFolderName = this.FindControl<TextBox>("txtNewFolderName");
            lstNavigationDirectories = this.FindControl<ListBox>("lstNavigationDirectories");
            
            // set the position of the grid splitter to the last position saved in application's configuration file
            grdContainer.ColumnDefinitions[0].Width = new GridLength(appConfig.Explorer.NavigationPanelWidth, GridUnitType.Star);
            grdContainer.ColumnDefinitions[2].Width = new GridLength(appConfig.Explorer.DirectoriesPanelWidth, GridUnitType.Star);
            grdContainer.ColumnDefinitions[4].Width = new GridLength(appConfig.Explorer.PreviewPanelWidth, GridUnitType.Star);
            BoundsProperty.Changed.AddClassHandler<Window>((s, e) => Window_SizeChanged());
            
            navigator.PathExpanded += NavigatorOnPathExpanded;
        }

        private void NavigatorOnPathExpanded(PathNavigatorItem obj, double offset)
        {
            lstNavigationDirectories.IsVisible = obj.IsExpanded;
            lstNavigationDirectories.Margin =  new Thickness(obj.TransformedBounds.Value.Clip.Left, obj.TransformedBounds.Value.Clip.Top - 12,0,0);
            string path = navigator.GetInternalPath(obj);
            IEnumerable<ListBoxItem> folders = Directory.GetDirectories(path).Select(dir =>
            {
                ListBoxItem item = new ListBoxItem()
                {
                    Content = dir.Contains(Path.DirectorySeparatorChar) ? dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1) : dir,
                    Tag = dir
                };
                item.Tapped += (sender, args) => navigator.NavigateToPath(item.Tag.ToString());
                return item;
            });
            lstNavigationDirectories.Items = folders;
            lstNavigationDirectories.Width = folders.Count() > 0 ? folders.Select(items => MeasureString(items.Content.ToString()).Width).Max() + 
                                             (folders.Count() * 19 + 8 < Bounds.Height - 100 ? 12 : 35) : 100; // change to widest folder inside
            lstNavigationDirectories.Height = folders.Count() > 0 ? folders.Count() * 19 + 8 < Bounds.Height - 100 ? folders.Count() * 19 + 8 : Bounds.Height - 100 : 19;
        }

        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Shows the current window as a modal dialog
        /// </summary>
        public async Task<bool?> ShowDialogAsync()
        {
            return await ShowDialog<bool?>(StartupV.Instance);
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
                new Typeface(FontFamily, FontStyle, FontWeight), 
                FontSize, 
                TextAlignment.Left, 
                TextWrapping.NoWrap, 
                Bounds.Size);
            return new Size(formattedText.Bounds.Width, formattedText.Bounds.Height);
        }
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================

        /// <summary>
        /// Handles window's Opened event
        /// </summary>
        private async void Window_Opened(object? sender, EventArgs e)
        {
            (DataContext as MainWindowVM).ShowingView += (s, e) => Show();
            (DataContext as MainWindowVM).HidingView += (s, e) => Hide();
            if (appConfig.Application.MainWindowPositionX != null)
                Position = Position.WithX((int) appConfig.Application.MainWindowPositionX);
            if (appConfig.Application.MainWindowPositionY != null)
                Position = Position.WithY((int) appConfig.Application.MainWindowPositionY);
            if (appConfig.Application.MainWindowWidth != null)
                Width = (int) appConfig.Application.MainWindowWidth;
            if (appConfig.Application.MainWindowHeight != null)
                Height = (int) appConfig.Application.MainWindowHeight;
            isWindowLoaded = true;

            // explorer settings
            fileExplorerControl.ItemsSelectionBackgroundColor = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsSelectionBackgroundColor ?? "#FFD1E8FF");
            fileExplorerControl.ItemsSelectionBorderColor = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsSelectionBorderColor ?? "#FF26A0DA");
            fileExplorerControl.ItemsSelectionForegroundColor = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsSelectionForegroundColor ?? "#FF000000");
            fileExplorerControl.ItemsBackgroundColorFirst = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsBackgroundColorFirst ?? "#FF28292B");
            fileExplorerControl.ItemsBackgroundColorSecond = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsBackgroundColorSecond ?? "#FF191E21");
            fileExplorerControl.ItemsBorderColorFirst = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsBorderColorFirst ?? "#FF363636");
            fileExplorerControl.ItemsBorderColorSecond = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsBorderColorSecond ?? "#FF000000");
            fileExplorerControl.ItemsForegroundColor = (SolidColorBrush)brushConverter.ConvertFromString(appConfig.Explorer.ItemsForegroundColor ?? "#FF6DCBFE");
            fileExplorerControl.ItemsTypeFace = new Typeface(new FontFamily(appConfig.Explorer.ItemsFont), (FontStyle)appConfig.Explorer.ItemsFontStyle, (FontWeight)appConfig.Explorer.ItemsFontWeight);
            fileExplorerControl.ItemsFontSize = appConfig.Explorer.ItemsFontSize;
            fileExplorerControl.ShowGrid = true;
            fileExplorerControl.ItemsHorizontalSpacing = 10;
            fileExplorerControl.ItemsVerticalSpacing = 1;

            fileExplorerControl.FolderBrowsed += FileExplorer_OnFolderBrowsed;
            //await Task.Delay(2000);
            fileExplorerControl.isWindowLoaded = true;
            // fileExplorerControl.Items = new AvaloniaList<FileSystemEntity>(Directory
            //     .GetFiles("/mnt/STORAGE/MULTIMEDIA/MUSIC/", "*", SearchOption.AllDirectories)
            //     .Select(path => new FileSystemEntity() {Path = path}).OrderBy(e => e.Path).Take(2000));
            //(DataContext as MainWindowVM).IsProgressBarVisible = true;
            //await fileExplorerControl.NavigateToPath("/mnt/STORAGE/MULTIMEDIA/MUSIC/");
            //(DataContext as MainWindowVM).IsProgressBarVisible = false;
            //fileExplorerControl.Focus();
            //await Task.Delay(3000);
            //fileExplorerControl.Layout = Layouts.Icons;
            //await Task.Delay(3000);
            //fileExplorerControl.Layout = Layouts.Details;

          

            // (DataContext as MainWindowVM).SourceDirectories = new ObservableCollection<FileSystemEntity>(Directory
            //     .GetFiles("/mnt/STORAGE/MULTIMEDIA/MUSIC/", "*", SearchOption.AllDirectories)
            //     .Select(path => new FileSystemEntity() {Path = path, IconSource = "file.png"}).OrderBy(e => e.Path)
            //     .Take(200));
            navigator.NavigateToPath("/mnt/STORAGE/MULTIMEDIA/MUSIC/");
        }

        private void FileExplorer_OnFolderBrowsed(string path)
        {
            navigator.NavigateToPath(path);
        }

        /// <summary>
        /// Handles window's SizeChanged event
        /// </summary>
        private async void Window_SizeChanged()
        {
            if (isWindowLoaded)
            {
                appConfig.Application.MainWindowHeight = (int) Height;
                appConfig.Application.MainWindowWidth = (int) Width;
                await appConfig.UpdateConfigurationAsync();
            }
        }

        /// <summary>
        /// Handles Window's PositionChanged event
        /// </summary>
        private async void Window_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            // do not allow the application's configuration to be updated with the new position
            // unless the window is loaded and the user is the one changing it
            if (isWindowLoaded)
            {
                appConfig.Application.MainWindowPositionX = Position.X;
                appConfig.Application.MainWindowPositionY = Position.Y;
                await appConfig.UpdateConfigurationAsync();
            }
        }


        /// <summary>
        /// Handles Window's KeyUp event
        /// </summary>
        private void Window_KeyUp(object? sender, KeyEventArgs e)
        {
            fileExplorerControl.IsCtrlPressed = false;
            fileExplorerControl.IsShiftPressed = false;
        }


        /// <summary>
        /// Handles Window's KeyDown event
        /// </summary>
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                fileExplorerControl.IsCtrlPressed = true;
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                fileExplorerControl.IsShiftPressed = true;
            else if (e.Key == Key.F2)
                fileExplorerControl.RenameItem();
        }

        /// <summary>
        /// Handles Favorite's Checked and Unchecked events
        /// </summary>
        private async void Favorite_CheckedChanged(object? sender, RoutedEventArgs e)
        {
            //(DataContext as FileBrowserDialogVM).IsFavoritePath = (sender as CheckBox).IsChecked == true;
            //await (DataContext as FileBrowserDialogVM).SetIsFavoritePathAsync_Command.ExecuteAsync();
        }

        /// <summary>
        /// Handles the preview panel toggle checkbox's CheckedChanged event
        /// </summary>
        private void TogglePreviewPanel_CheckChanged(object? sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                grdContainer.ColumnDefinitions[2].Width =
                    new GridLength(appConfig.Explorer.DirectoriesPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].Width =
                    new GridLength(appConfig.Explorer.PreviewPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].MinWidth = 75;
                grdPreview.IsVisible = true;
                spPreview.IsVisible = true;
            }
            else
            {
                grdContainer.ColumnDefinitions[2].Width = new GridLength(appConfig.Explorer.DirectoriesPanelWidth + appConfig.Explorer.PreviewPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].MinWidth = 0;
                grdPreview.IsVisible = false;
                spPreview.IsVisible = false;
            }
        }

        /// <summary>
        /// Shows the modal box for entering a new folder name
        /// </summary>
        private void ShowNewFolderModalBox(object sender, RoutedEventArgs e)
        {
            grdNewFolder.IsVisible = true;
            modalGrid.IsVisible = true;
            txtNewFolderName.Focus();
        }

        /// <summary>
        /// Handles Directories separator's DragCompleted event
        /// </summary>
        private async void SeparatorDirectories_DragCompleted(object sender, VectorEventArgs e)
        {
            appConfig.Explorer.NavigationPanelWidth = grdContainer.ColumnDefinitions[0].Width.Value;
            appConfig.Explorer.DirectoriesPanelWidth = grdContainer.ColumnDefinitions[2].Width.Value;
            await appConfig.UpdateConfigurationAsync();
        }

        /// <summary>
        /// Handles Directories listbox's PointerWheelChanged event
        /// </summary>
        private void Directories_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            //directoriesScrollViewer.Offset = new Vector(directoriesScrollViewer.Offset.X - (e.Delta.Y * 50), directoriesScrollViewer.Offset.Y);
        }

        /// <summary>
        /// Handles SelectionChanged event of the directories listview
        /// </summary>
        private void Directories_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //FileBrowserDialogVM viewmodel = (FileBrowserDialogVM)DataContext;
            //// get a list of all selected items in the directories listview
            //List<FileSystemEntity> selectedItems = (sender as ListBox).SelectedItems.Cast<FileSystemEntity>().ToList();
            //// only files should be allowed in the selection (not drives or folders)
            //if (selectedItems.Select(e => e.DirType).All(e => e == 0))
            //{
            //    viewmodel.SelectedFiles = selectedItems.Count > 0 ? "\"" + string.Join("\" \"", selectedItems.Select(e => e.Path).ToArray()) + "\"" : string.Empty;
            //    if (selectedItems.Count == 1 && grdPreview.IsVisible)
            //        viewmodel.PreviewFile();
            //}
            //else
            //    viewmodel.SelectedFiles = string.Empty;
        }

        /// <summary>
        /// Handles extensions filter SelectionChanged event
        /// </summary>
        private async void Extensions_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                (DataContext as MainWindowVM).SelectedExtensionFilter =
                    (sender as ComboBox).SelectedItem as SearchEntity;
                await (DataContext as MainWindowVM).SelectedExtensionChangedAsync_Command.ExecuteAsync();
            }
        }

        /// <summary>
        /// Handles Preview separator's DragCompleted event
        /// </summary>
        private async void SeparatorPreview_DragCompleted(object sender, VectorEventArgs e)
        {
            if (grdPreview.IsVisible)
            {
                appConfig.Explorer.DirectoriesPanelWidth = grdContainer.ColumnDefinitions[2].Width.Value;
                appConfig.Explorer.PreviewPanelWidth = grdContainer.ColumnDefinitions[4].Width.Value;
                await appConfig.UpdateConfigurationAsync();
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the new folder name textbox
        /// </summary>
        private async void NewFolderName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty((sender as TextBox).Text))
            {
                HideNewFolderModalBox(sender, e);
                await (DataContext as MainWindowVM).CreateNewFolder_Command.ExecuteAsync();
            }
            else if (e.Key == Key.Escape)
                HideNewFolderModalBox(sender, e);
        }

        /// <summary>
        /// Hides the modal box for entering a new folder name
        /// </summary>
        private void HideNewFolderModalBox(object sender, RoutedEventArgs e)
        {
            grdNewFolder.IsVisible = false;
            modalGrid.IsVisible = false;
        }

        #endregion

        private void ViewModes_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            fileExplorerControl.Layout = (FileSystemExplorerLayouts) (sender as Image).Tag;
        }

        private async Task PathNavigator_OnLocationChanged(string path)
        {
            (DataContext as MainWindowVM).IsProgressBarVisible = true;
            await fileExplorerControl.NavigateToPath(path); // TODO: replace with current tab's file explorer
            (DataContext as MainWindowVM).IsProgressBarVisible = false;

        }

        private void Window_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // when clicking anywhere in the window, close the directories drop down list, if it was expanded
            lstNavigationDirectories.IsVisible = false;
            navigator.CollapseAllExpanders();
        }
    }
}