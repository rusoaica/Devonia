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
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Devonia.Infrastructure.Notification;
using Devonia.ViewModels;
using Devonia.ViewModels.Common.Dispatcher;
using System.ComponentModel;
using System.Globalization;
#endregion

namespace Devonia.Views.Main
{


    public class YTypeFaceConverter : AvaloniaPropertyTypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(Typeface);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            YTypeFace typeface = (Typeface)value;
            return typeface;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Typeface);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return (Typeface)value;
        }
    }

    [TypeConverter(typeof(YTypeFaceConverter))]
    public class YTypeFace
    {
        public FontWeight FontWeight { get; set; }

        public static implicit operator Typeface(YTypeFace myTypeFace)
        {
            return new Typeface(FontFamily.Default, FontStyle.Italic, myTypeFace.FontWeight);
        }

        public static implicit operator YTypeFace(Typeface typeFace)
        {
            return new YTypeFace { FontWeight = typeFace.Weight };
        }


    }
    
    public partial class MainWindowV : Window, IMainWindowView
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private bool isWindowLoaded;
        private readonly IAppConfig? appConfig;
        private readonly Grid? modalGrid;
        private readonly Grid? grdPreview;
        private readonly Grid? grdContainer;
        private readonly Border? grdNewFolder;
        private readonly GridSplitter? spPreview;
        private readonly PathNavigator? navigator;
        private readonly TextBox? txtNewFolderName;
        private readonly ListBox? lstNavigationDirectories;
        private readonly BrushConverter brushConverter = new();
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public MainWindowVM? ViewModel => DataContext as MainWindowVM;
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
#endif
            this.appConfig = appConfig;
            modalGrid = this.FindControl<Grid>("modalGrid");
            grdPreview = this.FindControl<Grid>("grdPreview");
            grdContainer = this.FindControl<Grid>("grdContainer");
            grdNewFolder = this.FindControl<Border>("grdNewFolder");
            spPreview = this.FindControl<GridSplitter>("spPreview");
            navigator = this.FindControl<PathNavigator>("txtNavigator");
            txtNewFolderName = this.FindControl<TextBox>("txtNewFolderName");
            lstNavigationDirectories = this.FindControl<ListBox>("lstNavigationDirectories");

            // set the position of the grid splitter to the last position saved in application's configuration file
            grdContainer.ColumnDefinitions[0].Width = new GridLength(appConfig.Explorer.NavigationPanelWidth, GridUnitType.Star);
            grdContainer.ColumnDefinitions[2].Width = new GridLength(appConfig.Explorer.DirectoriesPanelWidth, GridUnitType.Star);
            grdContainer.ColumnDefinitions[4].Width = new GridLength(appConfig.Explorer.PreviewPanelWidth, GridUnitType.Star);
            BoundsProperty.Changed.AddClassHandler<Window>((s, e) => Window_SizeChanged());

            navigator.PathExpanded += NavigatorOnPathExpanded;
            navigator.HistoryExpanded += NavigatorOnHistoryExpanded;


            // TODO: move in resource loading method
            // re-assign the values of the application resources used in customizing the explorer items using the values stored in the configuration file
            Application.Current.Resources["ShowGridRes"] = appConfig.Explorer.ShowGrid;
            Application.Current.Resources["AutosizeWidthRes"] = appConfig.Explorer.AutosizeWidth;
            Application.Current.Resources["ItemsVerticalSpacingRes"] = appConfig.Explorer.ItemsVerticalSpacing;
            Application.Current.Resources["ItemsHorizontalSpacingRes"] = appConfig.Explorer.ItemsHorizontalSpacing;
            Application.Current.Resources["ItemsForegroundColorRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsForegroundColor ?? "#FF6DCBFE");
            Application.Current.Resources["SelectionBorderColorRes"] = brushConverter.ConvertFromString(appConfig.Explorer.SelectionBorderColor ?? "#FF0078D7");
            Application.Current.Resources["ItemsBorderColorFirstRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsBorderColorFirst ?? "#FF363636");
            Application.Current.Resources["ItemsBorderColorSecondRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsBorderColorSecond ?? "#FF000000");
            Application.Current.Resources["SelectionBackgroundColorRes"] = brushConverter.ConvertFromString(appConfig.Explorer.SelectionBackgroundColor ?? "#770078D7");
            Application.Current.Resources["ItemsSelectionBorderColorRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsSelectionBorderColor ?? "#FF26A0DA");
            Application.Current.Resources["ItemsBackgroundColorFirstRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsBackgroundColorFirst ?? "#FF28292B");
            Application.Current.Resources["ItemsBackgroundColorSecondRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsBackgroundColorSecond ?? "#FF191E21");
            Application.Current.Resources["ItemsSelectionBackgroundColorRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsSelectionBackgroundColor ?? "#FFD1E8FF");
            Application.Current.Resources["ItemsSelectionForegroundColorRes"] = brushConverter.ConvertFromString(appConfig.Explorer.ItemsSelectionForegroundColor ?? "#FF000000");
            Application.Current.Resources["SelectionHoverItemsBackgroundColorRes"] = brushConverter.ConvertFromString(appConfig.Explorer.SelectionHoverItemsBackgroundColor ?? "#FF0048D7");
            Application.Current.Resources["ItemsTypeFaceRes"] = new Typeface(
                new FontFamily(appConfig.Explorer.ItemsFont),
                (FontStyle)appConfig.Explorer.ItemsFontStyle,
                (FontWeight)appConfig.Explorer.ItemsFontWeight);
            if (appConfig.Explorer.ItemsFontSize > 0)
                Application.Current.Resources["ItemsFontSizeRes"] = appConfig.Explorer.ItemsFontSize;
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
            ViewModel!.ShowingView += (s, e) => Show();
            ViewModel.HidingView += (s, e) => Hide();
            ViewModel.FolderBrowsed += FileSystemExplorerPage_FolderBrowsed;
            if (appConfig!.Application.MainWindowPositionX != null)
                Position = Position.WithX((int)appConfig.Application.MainWindowPositionX);
            if (appConfig.Application.MainWindowPositionY != null)
                Position = Position.WithY((int)appConfig.Application.MainWindowPositionY);
            if (appConfig.Application.MainWindowWidth != null)
                Width = (int)appConfig.Application.MainWindowWidth;
            if (appConfig.Application.MainWindowHeight != null)
                Height = (int)appConfig.Application.MainWindowHeight;
            isWindowLoaded = true;
            ViewModel.PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Handles file system explorers' FolderBrowsed event
        /// </summary>
        /// <param name="path">The new path of the file system explorer, after the folder was browsed</param>
        private void FileSystemExplorerPage_FolderBrowsed(string path)
        {
            navigator!.NavigateToPath(path);
        }

        /// <summary>
        /// Handles the window's PropertyChanged event
        /// </summary>
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // when changing the selected file system explorer page, update the navigator location too
            if (e.PropertyName == nameof(MainWindowVM.SelectedExplorerPage))
            {
                if (isWindowLoaded && ViewModel!.SelectedExplorerPage is ExplorerPageVM selectedPage)
                    if (navigator!.CurrentPath != selectedPage.CurrentPath)
                        navigator.NavigateToPath(selectedPage.CurrentPath);
            }
        }

        /// <summary>
        /// Handles the navigator's LocationChanged event
        /// </summary>
        /// <param name="path">The new path of the navigator</param>
        /// <param name="isRefresh">Indicates whether the navigation is done to the same folder or not (refreshing location)</param>
        private async Task PathNavigator_OnLocationChanged(string path, bool isRefresh)
        {
            if (isWindowLoaded && ViewModel!.SelectedExplorerPage != null)
                if (isRefresh || (!isRefresh && ViewModel.SelectedExplorerPage.CurrentPath != path))
                    ViewModel.SelectedExplorerPage.CurrentPath = path;
        }

        /// <summary>
        /// Handles window's SizeChanged event
        /// </summary>
        private async void Window_SizeChanged()
        {
            if (isWindowLoaded)
            {
                appConfig!.Application.MainWindowHeight = (int)Height;
                appConfig!.Application.MainWindowWidth = (int)Width;
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
                appConfig!.Application.MainWindowPositionX = Position.X;
                appConfig!.Application.MainWindowPositionY = Position.Y;
                await appConfig.UpdateConfigurationAsync();
            }
        }


        /// <summary>
        /// Handles Window's KeyUp event
        /// </summary>
        private void Window_KeyUp(object? sender, KeyEventArgs e)
        {
            if (isWindowLoaded)
                ViewModel!.ResetSpecialModes();
        }


        /// <summary>
        /// Handles Window's KeyDown event
        /// </summary>
        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            lstNavigationDirectories!.IsVisible = false;
            navigator!.CollapseAllExpanders();
            navigator.IsHistoryExpanded = false;
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                ViewModel!.SetControlSpecialMode();
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ViewModel!.SetShiftSpecialMode();
            else if (e.Key == Key.F5 && ViewModel!.SelectedExplorerPage != null) // refresh current explorer page
                ViewModel!.SelectedExplorerPage.CurrentPath = navigator.CurrentPath;
            //else if (e.Key == Key.F2)
            //fileExplorerControl.RenameItem();
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
            if ((sender as CheckBox)!.IsChecked == true)
            {
                grdContainer!.ColumnDefinitions[2].Width =
                    new GridLength(appConfig!.Explorer.DirectoriesPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].Width =
                    new GridLength(appConfig!.Explorer.PreviewPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].MinWidth = 75;
                grdPreview!.IsVisible = true;
                spPreview!.IsVisible = true;
            }
            else
            {
                grdContainer!.ColumnDefinitions[2].Width = new GridLength(appConfig!.Explorer.DirectoriesPanelWidth + appConfig!.Explorer.PreviewPanelWidth, GridUnitType.Star);
                grdContainer!.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);
                grdContainer!.ColumnDefinitions[4].MinWidth = 0;
                grdPreview!.IsVisible = false;
                spPreview!.IsVisible = false;
            }
        }

        /// <summary>
        /// Shows the modal box for entering a new folder name
        /// </summary>
        private void ShowNewFolderModalBox(object sender, RoutedEventArgs e)
        {
            grdNewFolder!.IsVisible = true;
            modalGrid!.IsVisible = true;
            txtNewFolderName!.Focus();
        }

        /// <summary>
        /// Handles Directories separator's DragCompleted event
        /// </summary>
        private async void SeparatorDirectories_DragCompleted(object sender, VectorEventArgs e)
        {
            appConfig!.Explorer.NavigationPanelWidth = grdContainer!.ColumnDefinitions[0].Width.Value;
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
            if ((sender as ComboBox)!.SelectedItem != null)
            {
                ViewModel!.SelectedExtensionFilter = (sender as ComboBox)!.SelectedItem as SearchEntity;
                await ViewModel.SelectedExtensionChangedAsync_Command.ExecuteAsync();
            }
        }

        /// <summary>
        /// Handles Preview separator's DragCompleted event
        /// </summary>
        private async void SeparatorPreview_DragCompleted(object sender, VectorEventArgs e)
        {
            if (grdPreview!.IsVisible)
            {
                appConfig!.Explorer.DirectoriesPanelWidth = grdContainer!.ColumnDefinitions[2].Width.Value;
                appConfig!.Explorer.PreviewPanelWidth = grdContainer.ColumnDefinitions[4].Width.Value;
                await appConfig.UpdateConfigurationAsync();
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the new folder name textbox
        /// </summary>
        private async void NewFolderName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty((sender as TextBox)!.Text))
            {
                HideNewFolderModalBox(sender, e);
                await ViewModel!.CreateNewFolder_Command.ExecuteAsync();
            }
            else if (e.Key == Key.Escape)
                HideNewFolderModalBox(sender, e);
        }

        /// <summary>
        /// Hides the modal box for entering a new folder name
        /// </summary>
        private void HideNewFolderModalBox(object sender, RoutedEventArgs e)
        {
            grdNewFolder!.IsVisible = false;
            modalGrid!.IsVisible = false;
        }

        /// <summary>
        /// Handles window's PointerReleased event
        /// </summary>
        private void Window_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // when clicking anywhere in the window outside the navigator area, close the directories drop down list, if it was expanded
            if (!navigator!.TransformedBounds!.Value.Contains(e.GetPosition(this)))
            {
                lstNavigationDirectories!.IsVisible = false;
                navigator.CollapseAllExpanders();
                navigator.IsHistoryExpanded = false;
            }
        }

        /// <summary>
        /// Handles Navigate Back's PointerReleased event
        /// </summary>
        private async void NavigateBack_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (isWindowLoaded && ViewModel!.SelectedExplorerPage != null)
            {
                await ViewModel.SelectedExplorerPage.NavigateBack_Command.ExecuteAsync();
                navigator!.NavigateToPath(ViewModel.SelectedExplorerPage.CurrentPath);
            }
        }

        /// <summary>
        /// Handles Navigate Forward's PointerReleased event
        /// </summary>
        private async void NavigateForward_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (isWindowLoaded && ViewModel!.SelectedExplorerPage != null)
            {
                await ViewModel.SelectedExplorerPage.NavigateForward_Command.ExecuteAsync();
                navigator!.NavigateToPath(ViewModel.SelectedExplorerPage.CurrentPath);
            }
        }

        /// <summary>
        /// Handles Navigate Up's PointerReleased event
        /// </summary>
        private async void NavigateUp_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (isWindowLoaded && ViewModel!.SelectedExplorerPage != null)
            {
                await ViewModel.SelectedExplorerPage.NavigateUp_Command.ExecuteAsync();
                navigator!.NavigateToPath(ViewModel.SelectedExplorerPage.CurrentPath);
            }
        }

        /// <summary>
        /// Handles the navigator's path items' Expanded events
        /// </summary>
        /// <param name="sender">The path navigator item that has been expanded</param>
        private void NavigatorOnPathExpanded(PathNavigatorItem sender)
        {
            // only show subfolders list if the path navigator item is expanded
            lstNavigationDirectories!.IsVisible = sender.IsExpanded;
            // only get the list of subfolders if the path navigator item is expanded
            if (sender.IsExpanded)
            {
                // position the subfolders list to be just under the expanded path navigator item
                lstNavigationDirectories.Margin = new Thickness(sender!.TransformedBounds!.Value.Clip.Left, sender.TransformedBounds.Value.Clip.Top - 12, 0, 0);
                // reconstruct the path from the root folder to expanded item
                string path = navigator!.GetInternalPath(sender);
                // for the above path, get all subfolders and add them to the subfolders list
                IEnumerable<ListBoxItem> folders = new DirectoryInfo(path).GetDirectories("*", SearchOption.TopDirectoryOnly).Select(di =>
                {
                    ListBoxItem item = new()
                    {
                        Content = di.Name,
                        Tag = di.FullName
                    };
                    item.Tapped += (s, a) => navigator!.NavigateToPath(item.Tag.ToString()!);
                    return item;
                });
                lstNavigationDirectories.Items = folders;
                int count = lstNavigationDirectories.ItemCount;
                // get the widest element in the subfolders list and resize it accordingly 
                // (take into account if the subfolders list can be entirely displayed, or a scrollbar is required too)
                lstNavigationDirectories.Width = count > 0 ? folders.Select(items => MeasureString(items.Content.ToString()!).Width).Max() +
                                                 (count * 19 + 8 < Bounds.Height - 100 ? 12 : 35) : 100;
                // try to display the entire list of subfolders, if they can fit in the space no lower than 100 pixels from window's bottom
                lstNavigationDirectories.Height = count > 0 ? count * 19 + 8 < Bounds.Height - 100 ? count * 19 + 8 : Bounds.Height - 100 : 19;
            }
        }

        /// <summary>
        /// Handles the navigator history's Expanded event
        /// </summary>
        private void NavigatorOnHistoryExpanded()
        {
            // re-uses list of subfolders for history list too (no need for a different one)
            // only show history list if the navigator's history is expanded
            lstNavigationDirectories!.IsVisible = navigator!.IsHistoryExpanded;
            // only get the history list if the navigator's history list is expanded
            if (navigator.IsHistoryExpanded)
            {
                // position the history list to be just under the navigator
                lstNavigationDirectories.Margin = new Thickness(navigator!.TransformedBounds!.Value.Clip.Left, navigator.TransformedBounds.Value.Clip.Top - 12, 0, 0);
                IEnumerable<ListBoxItem> folders = ViewModel!.SelectedExplorerPage.SourceHistory.Select(dir =>
                {
                    ListBoxItem item = new()
                    {
                        Content = dir
                    };
                    item.Tapped += (s, a) => navigator!.NavigateToPath(item.Content.ToString()!);
                    item.Width = navigator.Bounds.Width;
                    return item;
                });
                lstNavigationDirectories.Items = folders;
                int count = lstNavigationDirectories.ItemCount;
                // the history list should be as wide as the navigator itself
                lstNavigationDirectories.Width = navigator.Bounds.Width;
                // try to display the entire history list, if it can fit in the space no lower than 100 pixels from window's bottom
                lstNavigationDirectories.Height = count > 0 ? count * 19 + 8 < Bounds.Height - 100 ? count * 19 + 8 : Bounds.Height - 100 : 19;
            }
        }
        #endregion
    }
}