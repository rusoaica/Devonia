using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using Devonia.Infrastructure.Enums;

namespace Devonia.Views.Common.Controls
{
    public class FileSystemExplorerItem : TemplatedControl
    {
        private string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        private bool isWindowLoaded;

        private FileSystemItemTypes fileSystemItemType; //~
        public FileSystemItemTypes FileSystemItemType
        {
            get { return fileSystemItemType; }
            set
            {
                fileSystemItemType = value;
                // if (imgIcon != null)
                // {
                //     if (!string.IsNullOrEmpty(iconSource))
                //         imgIcon.Source = new Bitmap(iconSource);
                //     else
                //         imgIcon.Source = Application.Current.Resources[value == FileSystemItemTypes.Folder ? "FolderBitmap" : "FileBitmap"] as Bitmap;
                // }
            }
        }

        public string Path { get; set; }
        public DateTime DateModified { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }

        private string iconSource;
        public string IconSource
        {
            get { return iconSource; }
            set
            {
                iconSource = value;
                // if (imgIcon != null)
                //     imgIcon.Source = new Bitmap(value);
            }
        }

        private Typeface currentTypeFace; //~
        public Typeface CurrentTypeFace
        {
            get { return currentTypeFace; }
            set
            {
                currentTypeFace = value;
                // if (lblContent != null)
                // {
                //     lblContent.FontFamily = value.FontFamily;
                //     lblContent.FontStyle = value.Style;
                //     lblContent.FontWeight = value.Weight;
                // }
            }
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

        private double iconSize = 16;
        internal double IconSize
        {
            get { return iconSize; }
            private set { iconSize = value;  }
        }

        private Thickness textMargin; //~
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
        
        private string text; //~
        public string Text
        {
            get { return lblContents?[0].Content?.ToString() ?? text; }
            set
            {
                text = value;
                // if (lblContent != null)
                //     lblContent.Content = value;
            }
        }

        
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            { 
                isSelected = value;
                ItemsBorderColorFirst = showBorder ? value ? selectionBorderColor : itemsBorderColorFirst : Brushes.Transparent; 
                ItemsBorderColorSecond = showBorder ? value ? selectionBorderColor : itemsBorderColorSecond : Brushes.Transparent; 
                if (pnlContainer != null)
                    pnlContainer.Background = value ? selectionBackgroundColor : showBorder ? backgroundColor : Brushes.Transparent;
                if (lblContents != null)
                    lblContents[0].Foreground = value ? foregroundSelectionColor : foregroundColor;
            }
        }

        private bool showBorder;
        public bool ShowBorder
        {
            get { return showBorder; }
            set
            {
                showBorder = value; 
                // ItemsBorderColorFirst = value ? isSelected ? selectionBorderColor : itemsBorderColorFirst : Brushes.Transparent; 
                // ItemsBorderColorSecond = value ? isSelected ? selectionBorderColor : itemsBorderColorSecond : Brushes.Transparent; 
                // if (pnlContainer != null)
                //     pnlContainer.Background = value ? isSelected ? selectionBackgroundColor : backgroundColor : Brushes.Transparent; 
            }
        }
        
        private IBrush foregroundColor;
        public IBrush ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                foregroundColor = value;
                //Foreground = isSelected ? value : foregroundSelectionColor;
            }
        }

        private IBrush foregroundSelectionColor;
        public IBrush ForegroundSelectionColor
        {
            get { return foregroundSelectionColor; }
            set
            {
                foregroundSelectionColor = value;
                //Foreground = isSelected ? value : foregroundColor;
            }
        }
        
        private IBrush itemsBorderColorFirst;
        public IBrush ItemsBorderColorFirst
        {
            get { return itemsBorderColorFirst; }
            set
            {
                itemsBorderColorFirst = showBorder ? isSelected ? selectionBorderColor : value : Brushes.Transparent; 
            }
        }
        
        private IBrush itemsBorderColorSecond;
        public IBrush ItemsBorderColorSecond
        {
            get { return itemsBorderColorSecond; }
            set
            {
                itemsBorderColorSecond = showBorder ? isSelected ? selectionBorderColor : value : Brushes.Transparent; 
            }
        }
        
        private IBrush selectionBorderColor;
        public IBrush SelectionBorderColor
        {
            get { return selectionBorderColor; }
            set
            {
                selectionBorderColor = value; 
                //BorderBrush = isSelected ? selectionBorderColor : showBorder ? borderColor : Brushes.Transparent; 
            }
        }
        
        private IBrush backgroundColor;
        public IBrush BackgroundColor
        {
            get { return backgroundColor; }
            set 
            { 
                backgroundColor = value;
                // if (pnlContainer != null)
                //     pnlContainer.Background = isSelected ? selectionBackgroundColor : backgroundColor; 
            }
        }
        
        private IBrush selectionBackgroundColor;
        public IBrush SelectionBackgroundColor
        {
            get { return selectionBackgroundColor; }
            set
            {
                selectionBackgroundColor = value;  
                // if (pnlContainer != null)
                //     pnlContainer.Background = isSelected ? selectionBackgroundColor : backgroundColor;
            }
        }
        
        private Panel pnlContainer;
        private Label[] lblContents;
        private Image imgIcon;
        private Line[] borders;
      
        public FileSystemExplorerItem() 
        {
            DataContext = this;
            Layout = layout;
            PointerPressed += Control_OnPointerPressed;
            TemplateApplied += OnTemplateApplied;
        }

        ~FileSystemExplorerItem()
        {
            PointerPressed -= Control_OnPointerPressed;
            TemplateApplied -= OnTemplateApplied;
        }
        
        private string CalculateHumanReadableSize()
        {
            if (Size == 0)
                return null;
            int order = 0;
            double length = Size;
            while (length >= 1024 && order < sizes.Length - 1) 
            {
                order++;
                length = length / 1024;
            }
            return String.Format("{0:0.00} {1}", length, sizes[order]);
        }

        private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
        {
            var children = this.GetVisualDescendants().ToList();
            pnlContainer = children.OfType<Panel>().First();
            lblContents = children.OfType<Label>().ToArray();
            imgIcon = children.OfType<Image>().First();
            borders = children.OfType<Line>().ToArray();
            
            BoundsProperty.Changed.AddClassHandler<Window>((s, e) => UserControl_SizeChanged());

            // create separate methods for settings all initial properties, and updating just text, icon and layout
            InitializeItem();
            ArrangeElements();
            isWindowLoaded = true;
        }

        private bool isSelectionHovered;
        public bool IsSelectionHovered
        {
            get { return isSelectionHovered; }
            set
            {
                isSelectionHovered = value;
                if (value && !isSelected)
                    pnlContainer.Background = Brushes.Red;
                else
                    pnlContainer.Background = isSelected ? selectionBackgroundColor : showBorder ? backgroundColor : Brushes.Transparent;
            }
        }

        public void InitializeItem()
        {
            pnlContainer.Background = isSelected ? selectionBackgroundColor : showBorder ? backgroundColor : Brushes.Transparent;
            lblContents[0].Content = text;
            lblContents[1].Content = DateModified;
            ToolTip.SetTip(this, Path); 
            ToolTip.SetTip(lblContents[1], DateTime.Now.Subtract(DateModified)); // TODO: convert to human readable form!
            lblContents[2].Content = Type;
            lblContents[3].Content = CalculateHumanReadableSize();
            for (int i = 0; i < 4; i++)
            {
                lblContents[i].Foreground = isSelected ? foregroundSelectionColor : foregroundColor;
                lblContents[i].FontFamily = currentTypeFace.FontFamily;
                lblContents[i].FontStyle = currentTypeFace.Style;
                lblContents[i].FontWeight = currentTypeFace.Weight;
                borders[i].Stroke = isSelected ? selectionBorderColor : i % 2 == 0 ? itemsBorderColorFirst : itemsBorderColorSecond;
                borders[i].IsVisible = showBorder;
            }
            if (!string.IsNullOrEmpty(iconSource))
                imgIcon.Source = new Bitmap(iconSource);
            else
                imgIcon.Source = Application.Current.Resources[fileSystemItemType == FileSystemItemTypes.Folder ? "FolderBitmap" : "FileBitmap"] as Bitmap;
        }

        public void UpdateItem()
        {
            if (lblContents != null)
            {
                lblContents[0].Content = text;
                lblContents[1].Content = DateModified;
                ToolTip.SetTip(this, Path); 
                ToolTip.SetTip(lblContents[1], DateTime.Now.Subtract(DateModified)); // ? convert to human readable form!
                lblContents[2].Content = Type;
                lblContents[3].Content = CalculateHumanReadableSize();
            }
            if (imgIcon != null)
            {
                if (!string.IsNullOrEmpty(iconSource))
                    imgIcon.Source = new Bitmap(iconSource);
                else
                    imgIcon.Source = Application.Current.Resources[fileSystemItemType == FileSystemItemTypes.Folder ? "FolderBitmap" : "FileBitmap"] as Bitmap;
            }
        }
        
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
        
        /// <summary>
        /// Handles control's PointerPressed event
        /// </summary>
        private void Control_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            //IsSelected = !isSelected;
            //e.Handled = true;
        }
        
        /// <summary>
        /// Handles control's SizeChanged event
        /// </summary>
        private void UserControl_SizeChanged()
        {
            if (isWindowLoaded)
                ArrangeElements();
        }
    }
}