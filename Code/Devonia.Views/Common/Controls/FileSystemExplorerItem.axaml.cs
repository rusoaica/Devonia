/// Written by: Yulia Danilova
/// Creation Date: 19th of October, 2021
/// Purpose: Code behind for the file system explorer item templated control
#region ========================================================================= USING =====================================================================================
using System;
using System.Collections.Generic;
using Avalonia;
using System.Linq;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using Avalonia.Media.Imaging;
using Avalonia.Controls.Shapes;
using Devonia.Infrastructure.Enums;
using Avalonia.Controls.Primitives;
using System.Runtime.CompilerServices;
#endregion

namespace Devonia.Views.Common.Controls
{
    public class FileSystemExplorerItem : TemplatedControl
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private Image imgIcon;
        private Line[] borders;
        private Panel pnlContainer;
        private Label[] lblContents;
        private bool isWindowLoaded;
        private readonly string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        public int VirtualId { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string IconSource { get; set; }
        public bool ShowBorder { get; set; }
        internal double IconSize { get; private set; } = 16;
        public DateTime DateModified { get; set; }
        public IBrush ForegroundColor { get; set; }
        public IBrush BackgroundColor { get; set; }
        public IBrush SelectionBorderColor { get; set; }
        public IBrush SelectionBackgroundColor { get; set; }
        public IBrush ForegroundSelectionColor { get; set; }
        public FileSystemItemTypes FileSystemItemType { get; set; }

        private Typeface currentTypeFace;
        public Typeface CurrentTypeFace
        {
            get { return currentTypeFace; }
            set { currentTypeFace = value; }
        }

        private FileSystemExplorerLayouts layout = FileSystemExplorerLayouts.List;
        public FileSystemExplorerLayouts Layout
        {
            get { return layout; }
            set
            {
                layout = value; 
                if (isWindowLoaded)
                    ArrangeElements();
            }
        }

        private Thickness textMargin;
        internal Thickness TextMargin
        {
            get { return textMargin; }
            private set
            {
                textMargin = value;
                if (lblContents != null)
                     lblContents[0].Margin = value;
            }
        }

        private string text; 
        public string Text
        {
            get { return lblContents?[0].Content?.ToString() ?? text; }
            set { text = value; }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            { 
                isSelected = value;
                ItemsBorderColorFirst = ShowBorder ? value ? SelectionBorderColor : itemsBorderColorFirst : Brushes.Transparent; 
                ItemsBorderColorSecond = ShowBorder ? value ? SelectionBorderColor : itemsBorderColorSecond : Brushes.Transparent; 
                if (pnlContainer != null)
                    pnlContainer.Background = value ? SelectionBackgroundColor : ShowBorder ? BackgroundColor : Brushes.Transparent;
                if (lblContents != null)
                    lblContents[0].Foreground = value ? ForegroundSelectionColor : ForegroundColor;
            }
        }

        private IBrush itemsBorderColorFirst;
        public IBrush ItemsBorderColorFirst
        {
            get { return itemsBorderColorFirst; }
            set
            {
                itemsBorderColorFirst = ShowBorder ? isSelected ? SelectionBorderColor : value : Brushes.Transparent; 
            }
        }
        
        private IBrush itemsBorderColorSecond;
        public IBrush ItemsBorderColorSecond
        {
            get { return itemsBorderColorSecond; }
            set
            {
                itemsBorderColorSecond = ShowBorder ? isSelected ? SelectionBorderColor : value : Brushes.Transparent; 
            }
        }

        private bool isSelectionHovered;
        public bool IsSelectionHovered
        {
            get { return isSelectionHovered; }
            set
            {
                isSelectionHovered = value;
                if (pnlContainer != null)
                {
                    if (value && !isSelected)
                        pnlContainer.Background = Brushes.Red;
                    else
                        pnlContainer.Background = isSelected ? SelectionBackgroundColor : ShowBorder ? BackgroundColor : Brushes.Transparent;
                }
            }
        }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public FileSystemExplorerItem() 
        {
            DataContext = this;
            Layout = layout;
            TemplateApplied += OnTemplateApplied;
        }

        /// <summary>
        /// Destuctor
        /// </summary>
        ~FileSystemExplorerItem()
        {
            TemplateApplied -= OnTemplateApplied;
        }
        #endregion
        
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Calculates the size of the current file system explorer item, in human readable form
        /// </summary>
        /// <returns>A string formatted size of the current file system explorer item</returns>
        private string CalculateHumanReadableSize()
        {
            // size can be 0 (ex: directories or 0 byte files)
            if (Size == 0 && FileSystemItemType != FileSystemItemTypes.File)
                return null;
            int order = 0;
            double length = Size;
            // keep dividing by 1024 (three order of magnitude) until the result is smaller than 1024
            // that is, get the highest order of magnitude for Size, which is expressed in bytes, for readability purposes
            while (length >= 1024 && order < sizes.Length - 1) 
            {
                order++;
                length = length / 1024;
            }
            return String.Format("{0:0.00} {1}", length, sizes[order]);
        }

        /// <summary>
        /// Sets the properties needed for the initial display of the file system explorer item
        /// </summary>
        public void InitializeItem()
        {
            // when selected, items have mandatory background color; otherwise, if border is disabled, they are transparent
            pnlContainer.Background = isSelected ? SelectionBackgroundColor : ShowBorder ? BackgroundColor : Brushes.Transparent;
            for (int i = 0; i < 4; i++)
            {
                lblContents[i].Foreground = isSelected ? ForegroundSelectionColor : ForegroundColor;
                lblContents[i].FontFamily = currentTypeFace.FontFamily;
                lblContents[i].FontStyle = currentTypeFace.Style;
                lblContents[i].FontWeight = currentTypeFace.Weight;
                borders[i].Stroke = isSelected ? SelectionBorderColor : i % 2 == 0 ? itemsBorderColorFirst : itemsBorderColorSecond;
                borders[i].IsVisible = ShowBorder;
            }
            UpdateItem();
        }

        /// <summary>
        /// Updates the displayed values of an existing file system explorer item
        /// </summary>
        public void UpdateItem()
        {
            // only things that can change after an item is created are displayed text, properties and icon
            if (lblContents != null)
            {
                lblContents[0].Content = text;
                lblContents[1].Content = DateModified;
                lblContents[2].Content = Type;
                lblContents[3].Content = CalculateHumanReadableSize();
                ToolTip.SetTip(this, Path); 
                // would be nice to display the time since it was last modified or created (ex: "3 days, 5 hours ago") 
                ToolTip.SetTip(lblContents[1], DateTime.Now.Subtract(DateModified)); // TODO: convert to human readable form!
            }
            // check if the current file system explorer item has a custom icon assigned, and if not, use a preselected one, depending on its type
            if (imgIcon != null)
            {
                if (!string.IsNullOrEmpty(IconSource))
                    imgIcon.Source = new Bitmap(IconSource);
                else
                    imgIcon.Source = Application.Current.Resources[FileSystemItemType == FileSystemItemTypes.Folder ? "FolderBitmap" : "FileBitmap"] as Bitmap;
            }
        }

        /// <summary>
        /// Aranges the position and size of the elements of the item, based on the current layout
        /// </summary>
        private void ArrangeElements()
        {
            switch (Layout)
            {
                case FileSystemExplorerLayouts.List:
                    TextMargin = new Thickness(IconSize + 5, 0, 0, 0);
                    if (lblContents != null)
                    {
                        lblContents[0].HorizontalContentAlignment = HorizontalAlignment.Left;
                        lblContents[1].IsVisible = false;
                        lblContents[2].IsVisible = false;
                        lblContents[3].IsVisible = false;
                    }
                    break;
                case FileSystemExplorerLayouts.Details:
                    TextMargin = new Thickness(IconSize + 5, 0, 0, 0);
                    if (lblContents != null)
                    {
                        lblContents[0].HorizontalContentAlignment = HorizontalAlignment.Left;
                        lblContents[1].IsVisible = true;
                        lblContents[2].IsVisible = true;
                        lblContents[3].IsVisible = true;
                    }
                    break;
                case FileSystemExplorerLayouts.Icons:
                    IconSize = Width;
                    TextMargin = new Thickness(0, IconSize + 3, 0, 0);
                    if (lblContents != null)
                    {
                        //lblContent.HorizontalAlignment = HorizontalAlignment.Center;
                        lblContents[0].HorizontalContentAlignment = HorizontalAlignment.Center;
                        lblContents[0].VerticalAlignment = VerticalAlignment.Bottom;
                        lblContents[1].IsVisible = false;
                        lblContents[2].IsVisible = false;
                        lblContents[3].IsVisible = false;
                    }
                    break;
            }
        }

        #endregion
        
        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles templated control's TemplateApplied event
        /// </summary>
        private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
        {
            List<IVisual> children = this.GetVisualDescendants().ToList();
            imgIcon = children.OfType<Image>().First();
            borders = children.OfType<Line>().ToArray();
            pnlContainer = children.OfType<Panel>().First();
            lblContents = children.OfType<Label>().ToArray();
            // subscribe to the event risen when the size of the control changes
            BoundsProperty.Changed.AddClassHandler<Window>((s, e) => UserControl_SizeChanged());
            InitializeItem();
            ArrangeElements();
            isWindowLoaded = true;
        }
        
        /// <summary>
        /// Handles control's SizeChanged event
        /// </summary>
        private void UserControl_SizeChanged()
        {
            if (isWindowLoaded)
                ArrangeElements();
        }
        #endregion
    }
}