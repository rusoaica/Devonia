/// Written by: Yulia Danilova
/// Creation Date: 03rd of July, 2021
/// Purpose: Code behind for the FileBrowseDialogV user control
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using System.Linq;
using Avalonia.Input;
using Avalonia.Controls;
using Devonia.Views.Startup;
using Avalonia.VisualTree;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using System.Collections.Generic;
using Devonia.ViewModels.Common.Models;
using Devonia.Models.Common.Models.Common;
using Devonia.Infrastructure.Configuration;
using Devonia.ViewModels.Common.Dialogs.FileBrowser;
#endregion

namespace Devonia.Views.Common.Dialogs
{
    public partial class FileBrowserDialogV : Window, IFileBrowserDialogView
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IAppConfig appConfig;
        private readonly Grid modalGrid;
        private readonly Grid grdPreview;
        private readonly Grid grdContainer;
        private readonly Border grdNewFolder;
        private readonly ListBox lstDirectories;
        private readonly GridSplitter spPreview;
        private readonly TextBox txtNewFolderName;
        private ScrollViewer directoriesScrollViewer;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor, stupidly and mandatorily required by Avalonia's designer. Does nothing...
        /// </summary>
        public FileBrowserDialogV()
        {

        }

        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="appConfig">The injected application's configuration</param>
        public FileBrowserDialogV(IAppConfig appConfig)
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
            this.appConfig = appConfig; 
            // set the size of the dialog to the last bounds saved in the application's configuration file
            Width = appConfig.Dialogs.DialogsWidth;
            Height = appConfig.Dialogs.DialogsHeight;
            modalGrid = this.FindControl<Grid>("modalGrid");
            grdPreview = this.FindControl<Grid>("grdPreview");
            grdContainer = this.FindControl<Grid>("grdContainer");
            grdNewFolder = this.FindControl<Border>("grdNewFolder");
            spPreview = this.FindControl<GridSplitter>("spPreview");
            lstDirectories = this.FindControl<ListBox>("lstDirectories");
            txtNewFolderName = this.FindControl<TextBox>("txtNewFolderName");
            // set the position of the grid splitter to the last position saved in application's configuration file
            grdContainer.ColumnDefinitions[0].Width = new GridLength(appConfig.Dialogs.NavigationPanelWidth, GridUnitType.Star);
            grdContainer.ColumnDefinitions[2].Width = new GridLength(appConfig.Dialogs.DirectoriesPanelWidth, GridUnitType.Star);
            grdContainer.ColumnDefinitions[4].Width = new GridLength(appConfig.Dialogs.PreviewPanelWidth, GridUnitType.Star);
            // avalonia has no "SizeChanged" event, subscribe to changes to the Bounds property with a method
            BoundsProperty.Changed.AddClassHandler<Window>((s, e) => Window_SizeChanged());
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
        #endregion

        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles window's Opened event
        /// </summary>
        private void Window_Opened(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((DataContext as FileBrowserDialogVM).InitialFolder) && !string.IsNullOrEmpty(appConfig.Dialogs.LastDirectory))
                (DataContext as FileBrowserDialogVM).InitialFolder = appConfig.Dialogs.LastDirectory;
            // set the selection mode
            lstDirectories.SelectionMode = (DataContext as FileBrowserDialogVM).AllowMultiselection ? SelectionMode.Multiple : SelectionMode.Single;
            directoriesScrollViewer = lstDirectories.FindDescendantOfType<ScrollViewer>();
            (DataContext as FileBrowserDialogVM).ClosingView += (s, e) => Close();
        }

        /// <summary>
        /// Handles Favorite's Checked and Unchecked events
        /// </summary>
        private async void Favorite_CheckedChanged(object? sender, RoutedEventArgs e)
        {
            (DataContext as FileBrowserDialogVM).IsFavoritePath = (sender as CheckBox).IsChecked == true;
            await (DataContext as FileBrowserDialogVM).SetIsFavoritePathAsync_Command.ExecuteAsync();
        }

        /// <summary>
        /// Handles extensions filter SelectionChanged event
        /// </summary>
        private async void Extensions_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {                
                (DataContext as FileBrowserDialogVM).SelectedExtensionFilter = (sender as ComboBox).SelectedItem as SearchEntity;
                await (DataContext as FileBrowserDialogVM).SelectedExtensionChangedAsync_Command.ExecuteAsync();
            }
        }

        /// <summary>
        /// Handles SelectionChanged event of the directories listview
        /// </summary>
        private void Directories_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            FileBrowserDialogVM viewmodel = (FileBrowserDialogVM)DataContext;
            // get a list of all selected items in the directories listview
            List<FileSystemEntity> selectedItems = (sender as ListBox).SelectedItems.Cast<FileSystemEntity>().ToList();
            // only files should be allowed in the selection (not drives or folders)
            if (selectedItems.Select(e => e.DirType).All(e => e == 0))
            {
                viewmodel.SelectedFiles = selectedItems.Count > 0 ? "\"" + string.Join("\" \"", selectedItems.Select(e => e.Path).ToArray()) + "\"" : string.Empty;
                if (selectedItems.Count == 1 && grdPreview.IsVisible)
                    viewmodel.PreviewFile();
            }
            else
                viewmodel.SelectedFiles = string.Empty;
        }

        /// <summary>
        /// Hides the modal box for entering a new folder name
        /// </summary>
        private void HideNewFolderModalBox(object sender, RoutedEventArgs e)
        {
            grdNewFolder.IsVisible = false;
            modalGrid.IsVisible = false;
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
        /// Handles the KeyUp event of the new folder name textbox
        /// </summary>
        private async void NewFolderName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty((sender as TextBox).Text))
            {
                HideNewFolderModalBox(sender, e);
                await (DataContext as FileBrowserDialogVM).CreateNewFolder_Command.ExecuteAsync();
            }
            else if (e.Key == Key.Escape)
                HideNewFolderModalBox(sender, e);
        }

        /// <summary>
        /// Handles Directories separator's DragCompleted event
        /// </summary>
        private async void SeparatorDirectories_DragCompleted(object sender, VectorEventArgs e)
        {
            appConfig.Dialogs.NavigationPanelWidth = grdContainer.ColumnDefinitions[0].Width.Value;
            appConfig.Dialogs.DirectoriesPanelWidth = grdContainer.ColumnDefinitions[2].Width.Value;
            await appConfig.UpdateConfigurationAsync();
        }

        /// <summary>
        /// Handles Preview separator's DragCompleted event
        /// </summary>
        private async void SeparatorPreview_DragCompleted(object sender, VectorEventArgs e)
        {
            if (grdPreview.IsVisible)
            {
                appConfig.Dialogs.DirectoriesPanelWidth = grdContainer.ColumnDefinitions[2].Width.Value;
                appConfig.Dialogs.PreviewPanelWidth = grdContainer.ColumnDefinitions[4].Width.Value;
                await appConfig.UpdateConfigurationAsync();
            }
        }

        /// <summary>
        /// Handles the preview panel toggle checkbox's CheckedChanged event
        /// </summary>
        private void TogglePreviewPanel_CheckChanged(object? sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                grdContainer.ColumnDefinitions[2].Width = new GridLength(appConfig.Dialogs.DirectoriesPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].Width = new GridLength(appConfig.Dialogs.PreviewPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].MinWidth = 75;
                grdPreview.IsVisible = true;
                spPreview.IsVisible = true;
            }
            else
            {
                grdContainer.ColumnDefinitions[2].Width = new GridLength(appConfig.Dialogs.DirectoriesPanelWidth + appConfig.Dialogs.PreviewPanelWidth, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);
                grdContainer.ColumnDefinitions[4].MinWidth = 0;
                grdPreview.IsVisible = false;
                spPreview.IsVisible = false;
            }
        }

        /// <summary>
        /// Handles Directories listbox's PointerWheelChanged event
        /// </summary>
        private void Directories_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            directoriesScrollViewer.Offset = new Vector(directoriesScrollViewer.Offset.X - (e.Delta.Y * 50), directoriesScrollViewer.Offset.Y);
        }

        /// <summary>
        /// Handles window's SizeChanged event
        /// </summary>
        private async void Window_SizeChanged()
        {
            appConfig.Dialogs.DialogsHeight = Height;
            appConfig.Dialogs.DialogsWidth = Width;
            await appConfig.UpdateConfigurationAsync();
        }
        #endregion
    }
}
