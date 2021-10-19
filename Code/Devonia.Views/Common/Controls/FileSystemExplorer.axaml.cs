using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Devonia.Infrastructure.Enums;
using Devonia.ViewModels.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Devonia.Models.Common.Extensions;
using Path = System.IO.Path;

namespace Devonia.Views.Common.Controls
{
    public class FileSystemExplorer : UserControl, INotifyPropertyChanged
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event Action<string> FolderBrowsed;
        public event PropertyChangedEventHandler PropertyChanged;
       
        private readonly Canvas container;
        private readonly Grid grdDetails;
        private readonly Image imgName;
        private readonly Image imgSize;
        private readonly Image imgType;
        private readonly Image imgModified;
        private readonly TextBox txtRename = new TextBox();

        public bool isWindowLoaded;
        private bool isMouseDown;
        private bool isInRenameMode;
        private int currentIndex;
        private int numberOfVisibleItems;
        private int numberOfVerticalItems;
        private int numberOfHorizontalItems;
        private FileSystemEntity[] virtualizedItems;

        private SortingItems currentSortBy = SortingItems.Name;
        private bool isSortedDescending; 
        private List<FileSystemEntity> temp;
        Point mouseDownPoint;
        Point mousePoint;
        Rectangle selectionRectangle = new Rectangle();
        private FileSystemExplorerItem lastSelectedItem;
        private DateTime lastClickTime;
        #endregion
        
        #region ============================================================ BINDING PROPERTIES =============================================================================
        private double verticalScrollBarMaximumValue;
        public double VerticalScrollBarMaximumValue 
        { 
            get { return verticalScrollBarMaximumValue; } 
            set { verticalScrollBarMaximumValue = value; Notify(); } 
        }
        
        private double horizontalScrollBarMaximumValue;
        public double HorizontalScrollBarMaximumValue 
        { 
            get { return horizontalScrollBarMaximumValue; } 
            set { horizontalScrollBarMaximumValue = value; Notify(); } 
        }
        
        private double verticalScrollBarValue;
        public double VerticalScrollBarValue 
        { 
            get { return verticalScrollBarValue; } 
            set { verticalScrollBarValue = value; Notify(); } 
        }
        
        private double horizontalScrollBarValue;
        public double HorizontalScrollBarValue 
        { 
            get { return horizontalScrollBarValue; } 
            set { horizontalScrollBarValue = value; Notify(); } 
        }
        
        private double scrollbarThumbSize = 100;
        public double ScrollbarThumbSize 
        { 
            get { return scrollbarThumbSize; } 
            set { scrollbarThumbSize = value; Notify(); } 
        }
        
        private ScrollBarVisibility isVerticalScrollBarEnabled = ScrollBarVisibility.Disabled;
        public ScrollBarVisibility IsVerticalScrollBarEnabled 
        { 
            get { return isVerticalScrollBarEnabled; } 
            set { isVerticalScrollBarEnabled = value; Notify(); } 
        }
        
        private ScrollBarVisibility isHorizontalScrollBarEnabled = ScrollBarVisibility.Disabled;
        public ScrollBarVisibility IsHorizontalScrollBarEnabled 
        { 
            get { return isHorizontalScrollBarEnabled; } 
            set { isHorizontalScrollBarEnabled = value; Notify(); } 
        }
        #endregion

        #region ================================================================ PROPERTIES =================================================================================
        private FileSystemExplorerLayouts layout = FileSystemExplorerLayouts.List;
        public FileSystemExplorerLayouts Layout
        {
            get { return layout; }
            set
            {
                layout = value;
                Initialize();
            }
        }

        private bool showGrid;
        public bool ShowGrid
        {
            get { return showGrid; }
            set
            {
                showGrid = value; 
                DrawUIElements();
            }
        }
        
        private bool autosizeWidth = true;
        public bool AutosizeWidth
        {
            get { return autosizeWidth; }
            set
            {
                autosizeWidth = value;
                Initialize();
            }
        }
        
        private bool alternatesBackgroundColor = true;
        public bool AlternatesBackgroundColor
        {
            get { return alternatesBackgroundColor; }
            set
            {
                alternatesBackgroundColor = value;
                Initialize();
            }
        }

        public bool IsCtrlPressed { get; set; }
        public bool IsShiftPressed { get; set; }

        private Typeface itemsTypeFace = new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Normal);
        public Typeface ItemsTypeFace
        {
            get { return itemsTypeFace; }
            set
            {
                itemsTypeFace = value; 
                Initialize();
            }
        }

        private int itemsFontSize;
        public int ItemsFontSize
        {
            get { return itemsFontSize; }
            set
            {
                itemsFontSize = value;
                Initialize();
            }
        }

        #endregion
        
        #region ========================================================== DEPENDENCY PROPERTIES ============================================================================
        private double itemsHeight = 20;
        public double ItemsHeight
        {
            get { return itemsHeight; }
            set 
            { 
                if (value < 3) return;
                SetAndRaise(ItemsHeightProperty, ref itemsHeight, value);
                Initialize();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsHeight"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, double> ItemsHeightProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, double>(nameof(ItemsHeight), e => e.ItemsHeight, (e, v) => e.ItemsHeight = v);

        private double itemsWidth = 400;
        public double ItemsWidth
        {
            get { return itemsWidth; }
            set 
            { 
                SetAndRaise(ItemsWidthProperty, ref itemsWidth, value);
                Initialize();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsWidth"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, double> ItemsWidthProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, double>(nameof(ItemsWidth), e => e.ItemsWidth, (e, v) => e.ItemsWidth = v);
        
        
        private IBrush itemsSelectionForegroundColor;
        public IBrush ItemsSelectionForegroundColor
        {
            get { return itemsSelectionForegroundColor; }
            set 
            { 
                SetAndRaise(ItemsSelectionForegroundColorProperty, ref itemsSelectionForegroundColor, value);
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsSelectionForegroundColor"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> ItemsSelectionForegroundColorProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsSelectionForegroundColor), e => e.ItemsSelectionForegroundColor, (e, v) => e.ItemsSelectionForegroundColor = v);

        
        private IBrush itemsForegroundColor;
        public IBrush ItemsForegroundColor
        {
            get { return itemsForegroundColor; }
            set 
            { 
                SetAndRaise(ItemsForegroundColorProperty, ref itemsForegroundColor, value);
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsForegroundColor"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> ItemsForegroundColorProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsForegroundColor), e => e.ItemsForegroundColor, (e, v) => e.ItemsForegroundColor = v);

        private IBrush itemsBorderColorFirst;
        public IBrush ItemsBorderColorFirst
        {
            get { return itemsBorderColorFirst; }
            set 
            { 
                SetAndRaise(ItemsBorderColorFirstProperty, ref itemsBorderColorFirst, value);
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsBorderColorFirst"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> ItemsBorderColorFirstProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsBorderColorFirst), e => e.ItemsBorderColorFirst, (e, v) => e.ItemsBorderColorFirst = v);
        
        
        private IBrush itemsBorderColorSecond;
        public IBrush ItemsBorderColorSecond
        {
            get { return itemsBorderColorSecond; }
            set 
            { 
                SetAndRaise(ItemsBorderColorSecondProperty, ref itemsBorderColorSecond, value);
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsBorderColorSecond"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> ItemsBorderColorSecondProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsBorderColorSecond), e => e.ItemsBorderColorSecond, (e, v) => e.ItemsBorderColorSecond = v);

        private IBrush itemsSelectionBorderColor;
        public IBrush ItemsSelectionBorderColor
        {
            get { return itemsSelectionBorderColor; }
            set
            {
                SetAndRaise(temsSelectionBorderColorProperty, ref itemsSelectionBorderColor, value); 
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsSelectionBorderColor"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> temsSelectionBorderColorProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsSelectionBorderColor), e => e.ItemsSelectionBorderColor, (e, v) => e.ItemsSelectionBorderColor = v);
        
        private IBrush itemsBackgroundColorSecond;
        public IBrush ItemsBackgroundColorSecond
        {
            get { return itemsBackgroundColorSecond; }
            set 
            { 
                SetAndRaise(ItemsBackgroundColorSecondProperty, ref itemsBackgroundColorSecond, value);
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsBackgroundColorSecond"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> ItemsBackgroundColorSecondProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsBackgroundColorSecond), e => e.ItemsBackgroundColorSecond, (e, v) => e.ItemsBackgroundColorSecond = v);

        private IBrush itemsBackgroundColorFirst;
        public IBrush ItemsBackgroundColorFirst
        {
            get { return itemsBackgroundColorFirst; }
            set 
            { 
                SetAndRaise(ItemsBackgroundColorFirstProperty, ref itemsBackgroundColorFirst, value);
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsBackgroundColorFirst"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> ItemsBackgroundColorFirstProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsBackgroundColorFirst), e => e.ItemsBackgroundColorFirst, (e, v) => e.ItemsBackgroundColorFirst = v);
        
        private IBrush itemsSelectionBackgroundColor;
        public IBrush ItemsSelectionBackgroundColor
        {
            get { return itemsSelectionBackgroundColor; }
            set
            {
                SetAndRaise(ItemsSelectionBackgroundColorProperty, ref itemsSelectionBackgroundColor, value);       
                DrawUIElements();
            }
        }
        
        /// <summary>
        /// Defines the <see cref="ItemsSelectionBackgroundColor"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, IBrush> ItemsSelectionBackgroundColorProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, IBrush>(nameof(ItemsSelectionBackgroundColor), e => e.ItemsSelectionBackgroundColor, (e, v) => e.ItemsSelectionBackgroundColor = v);

        private AvaloniaList<FileSystemEntity> items = new AvaloniaList<FileSystemEntity>();
        public AvaloniaList<FileSystemEntity> Items
        {
            get { return items; }
            set { SetAndRaise(ItemsProperty, ref items, value); Initialize(); }
        }

        /// <summary>
        /// Defines the <see cref="Items"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, AvaloniaList<FileSystemEntity>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, AvaloniaList<FileSystemEntity>>(nameof(Items), e => e.Items, (e, v) => e.Items = v);
        
        private int itemsHorizontalSpacing = 10;
        public int ItemsHorizontalSpacing
        {
            get { return itemsHorizontalSpacing; }
            set
            {
                SetAndRaise(ItemsHorizontalSpacingProperty, ref itemsHorizontalSpacing, value); 
                Initialize(); 
            }
        }

        /// <summary>
        /// Defines the <see cref="ItemsHorizontalSpacing"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, int> ItemsHorizontalSpacingProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, int>(nameof(ItemsHorizontalSpacing), e => e.ItemsHorizontalSpacing, (e, v) => e.ItemsHorizontalSpacing = v);
        
        private int itemsVerticalSpacing = 1;
        public int ItemsVerticalSpacing
        {
            get { return itemsVerticalSpacing; }
            set
            {
                SetAndRaise(ItemsVerticalSpacingProperty, ref itemsVerticalSpacing, value); 
                Initialize(); 
            }
        }

        /// <summary>
        /// Defines the <see cref="ItemsVerticalSpacing"/> property.
        /// </summary>
        public static readonly DirectProperty<FileSystemExplorer, int> ItemsVerticalSpacingProperty =
            AvaloniaProperty.RegisterDirect<FileSystemExplorer, int>(nameof(ItemsVerticalSpacing), e => e.ItemsVerticalSpacing, (e, v) => e.ItemsVerticalSpacing = v);
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public FileSystemExplorer()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            container = this.FindControl<Canvas>("container");
            grdDetails = this.FindControl<Grid>("grdDetails");
            imgName = this.FindControl<Image>("imgName");
            imgSize = this.FindControl<Image>("imgSize");
            imgType = this.FindControl<Image>("imgType");
            imgModified = this.FindControl<Image>("imgModified");
            //BoundsProperty.Changed.AddClassHandler<Window>((s, e) => UserControl_SizeChanged());

            txtRename.ZIndex = 1;
            container.PropertyChanged += FileSystemExploer_PropertyChanged;
            txtRename.IsVisible = false;
            txtRename.Padding = new Thickness(0);
            txtRename.KeyDown += RenameTextBox_KeyDown;
            txtRename.LostFocus += RenameTextBox_LostFocus;
            selectionRectangle.ZIndex = 1;
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
            selectionRectangle.StrokeThickness = 1;
            selectionRectangle.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#770078D7"); // TODO: take from app config
            selectionRectangle.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0078D7");
       
            container.Children.Add(txtRename);
            container.Children.Add(selectionRectangle);
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Notifies the UI about a bound property's value being changed
        /// </summary>
        /// <param name="propertyName">The property that had the value changed</param>
        public void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ApplyGlobalTemplate()
        {
            // settings from appsettings.json, for any location
            
            
            // item selection bg/border color
            // item bg/border color (file and folder)
            // show items border
            // font size/color/weight/etc
            // autosize width for list layout
        }
        
        private void ApplyFolderTemplate()
        {
            // settings from database, for each location
            
            // explorer background color/image
            // items size
            // show preview icon
            // icons layout
            // description
            
            foreach (FileSystemExplorerItem item in container.Children)
            {
                
            }
        }

        public void RenameItem(FileSystemExplorerItem clickedItem = null)
        {
            // rename mode
            txtRename.IsVisible = true;
            isInRenameMode = true;
            if (clickedItem == null)
            {
                IEnumerable<FileSystemExplorerItem> selectedItems = container.Children.OfType<FileSystemExplorerItem>().Where(item => item.IsSelected);
                if (selectedItems.Count() == 0)
                    throw new InvalidOperationException("Renaming requires at least one selected item!");
                else
                    clickedItem = selectedItems.First();
            }
            if (!clickedItem.IsSelected)
                clickedItem.IsSelected = true;
            txtRename.Margin = clickedItem.Margin;
            txtRename.Width = clickedItem.Width - clickedItem.TextMargin.Left; // TODO: change in different views!
            txtRename.MinWidth = clickedItem.Width - clickedItem.TextMargin.Left;
            txtRename.Height = clickedItem.Height;
            txtRename.MinHeight = clickedItem.Height;
            txtRename.VerticalAlignment = clickedItem.VerticalAlignment;
            txtRename.HorizontalAlignment = clickedItem.HorizontalAlignment;
            txtRename.Text = clickedItem.Text;
            txtRename.FontFamily = clickedItem.CurrentTypeFace.FontFamily;
            txtRename.FontStyle = clickedItem.CurrentTypeFace.Style;
            txtRename.FontSize = clickedItem.FontSize;
            txtRename.FontWeight = clickedItem.CurrentTypeFace.Weight;
            txtRename.SelectAll();
            Canvas.SetTop(txtRename, Canvas.GetTop(clickedItem));
            Canvas.SetLeft(txtRename, Canvas.GetLeft(clickedItem) + clickedItem.TextMargin.Left);
            txtRename.Focus();
            // TODO: use list for selected items, instead of the controls themselves
            if (container.Children.OfType<FileSystemExplorerItem>().Where(item => item.IsSelected).Count() > 0)
            {
                isInRenameMode = true;
                txtRename.IsVisible = true;
                txtRename.Focus();
            }
        }
        
        private void SortItems(SortingItems sortBy)
        {
            // when sorting by the same currently sorted item, just change sorting direction
            //if (currentSortBy == sortBy)
                //isSortedDescending = !isSortedDescending;
            //else
                currentSortBy = sortBy;
            
            Func<IOrderedEnumerable<FileSystemEntity>,Func<FileSystemEntity,object>,IOrderedEnumerable<FileSystemEntity>> orderDelegate = isSortedDescending ? 
                    Enumerable.ThenByDescending : Enumerable.ThenBy;

            IOrderedEnumerable<FileSystemEntity> sortedByExten = temp.OrderByDescending(e => e.Extension == null);

            temp = orderDelegate(sortedByExten, x =>
                currentSortBy == SortingItems.Name ? x.Name :
                currentSortBy == SortingItems.Size ? x.Size :
                currentSortBy == SortingItems.Type ? x.Extension :
                currentSortBy == SortingItems.Date ? x.Date : null).ToList();
        }
        
        /// <summary>
        /// Updates the geometry and visibility of the sorting button images
        /// </summary>
        private void UpdateSortAdorners()
        {
            imgName.IsVisible = false;
            imgSize.IsVisible = false;
            imgType.IsVisible = false;
            imgModified.IsVisible = false;
            GeometryDrawing geometryDrawing = new GeometryDrawing()
            {
                Geometry = Application.Current.Resources[isSortedDescending ? "SortDescendingGeometry" : "SortAscendingGeometry"] as Geometry,
                Brush = Brushes.Red
            };
            DrawingImage drawingImage = new DrawingImage() { Drawing = geometryDrawing };

            if (currentSortBy == SortingItems.Name)
            {
                imgName.Source = drawingImage;
                imgName.IsVisible = true;
            }
            else if (currentSortBy == SortingItems.Size)
            {
                imgSize.Source = drawingImage;
                imgSize.IsVisible = true;
            }
            else if (currentSortBy == SortingItems.Type)
            {
                imgType.Source = drawingImage;
                imgType.IsVisible = true;
            }
            else if (currentSortBy == SortingItems.Date)
            {
                imgModified.Source = drawingImage;
                imgModified.IsVisible = true;
            }
        }
        
        /// <summary>
        /// Re-displays the required scroll bars 
        /// </summary>
        private void CalculateScrollBars()
        {
            switch (Layout)
            {
                case FileSystemExplorerLayouts.List:
                    HorizontalScrollBarValue = currentIndex;
                    IsVerticalScrollBarEnabled = ScrollBarVisibility.Disabled;
                    IsHorizontalScrollBarEnabled = ScrollBarVisibility.Visible;
                    // scroll bar only makes sense when not all items fit on visible area
                    if (numberOfVisibleItems < Items.Count)
                        HorizontalScrollBarMaximumValue = Items.Count / numberOfVerticalItems;
                    else
                        HorizontalScrollBarMaximumValue = 0;
                    ScrollbarThumbSize = (numberOfHorizontalItems / (HorizontalScrollBarMaximumValue + numberOfHorizontalItems)) * container.Bounds.Width;
                    break;
                case FileSystemExplorerLayouts.Icons:
                    VerticalScrollBarValue = currentIndex;
                    IsVerticalScrollBarEnabled = ScrollBarVisibility.Visible;
                    IsHorizontalScrollBarEnabled = ScrollBarVisibility.Disabled;
                    // scroll bar only makes sense when not all items fit on visible area
                    if (numberOfVisibleItems < Items.Count)
                        VerticalScrollBarMaximumValue = Items.Count / numberOfHorizontalItems;
                    else
                        VerticalScrollBarMaximumValue = 0;
                    ScrollbarThumbSize = (numberOfVerticalItems / (VerticalScrollBarMaximumValue + numberOfVerticalItems)) * container.Bounds.Height;
                    break;
                case FileSystemExplorerLayouts.Details:
                    VerticalScrollBarValue = currentIndex;
                    IsVerticalScrollBarEnabled = ScrollBarVisibility.Visible;
                    IsHorizontalScrollBarEnabled = ScrollBarVisibility.Disabled;
                    // in details view, each scroll step means 3 items
                    if (numberOfVisibleItems < Items.Count) // also add one extra unit if there is a remainder of less than three items at the end
                        VerticalScrollBarMaximumValue = (Items.Count - numberOfVerticalItems) / 3 + ((Items.Count - numberOfVerticalItems) % 3 > 0 ? 1 : 0);
                    else
                        VerticalScrollBarMaximumValue = 0; // scroll bar only makes sense when not all items fit on visible area
                    ScrollbarThumbSize = (numberOfVerticalItems / (VerticalScrollBarMaximumValue + numberOfVerticalItems)) * container.Bounds.Height;
                    break;
            }
        }
        
        /// <summary>
        /// Measures the size of <paramref name="candidate"/> with a specific font
        /// </summary>
        /// <param name="candidate">The text to measure</param>
        /// <returns>The size of <paramref name="candidate"/></returns>
        private Size MeasureString(string candidate)
        {
            FormattedText formattedText = new FormattedText(candidate, itemsTypeFace, itemsFontSize, TextAlignment.Left, TextWrapping.NoWrap, container.Bounds.Size);
            return new Size(formattedText.Bounds.Width, formattedText.Bounds.Height);
        }
        
        /// <summary>
        /// Measures the widest element in a column starting from <paramref name="startingIndex"/>
        /// </summary>
        /// <param name="startingIndex">The index from which to start the column to be measured</param>
        /// <returns>The width of the widest element in a column starting from <paramref name="startingIndex"/></returns>
        private double GetWidestElementInColumn(int startingIndex)
        {
            double result = 0;
            // if available, take a whole column from the provided info
            if (startingIndex <= Items.Count - numberOfVerticalItems)
                result = Items?.Skip(startingIndex)?
                               .Take(numberOfVerticalItems)?
                               .Max(e => MeasureString(e.Name).Width) + 10 ?? 0;
            else // there is not an entire column available, get the remainder of items
                result = Items?.Skip(Items.Count - (Items.Count % numberOfVerticalItems))?
                               .Take(Items.Count % numberOfVerticalItems)?
                               .Max(e => MeasureString(e.Name).Width) + 10 ?? 0; // + 10 = add extra space near string end
            if (layout == FileSystemExplorerLayouts.List)
                result += itemsHeight; // add icon's width (icon's width and height are equal to item's height) 
            return result;
        }
        
        /// <summary>
        /// Updates displayed info on already created file system items
        /// </summary>
        private async Task RecycleUIElements()
        {
            double xCoordinate = 0, yCoordinate = 0;
            int currentColumn = 0;
            // determine the width of the first column
            double widestItem = autosizeWidth && layout == FileSystemExplorerLayouts.List ? GetWidestElementInColumn(currentIndex * numberOfVerticalItems) : itemsWidth;
            // iterate all visible items
            try
            {
                for (int i = 0; i < numberOfVisibleItems; i++)
                {
                    
                    FileSystemExplorerItem item = null;
                    switch (Layout)
                    {
                        case FileSystemExplorerLayouts.List:
                            if (i >= container.Children.OfType<FileSystemExplorerItem>().Count())
                            {
                                // when more columns fit when recycling items than they were initially created when drawn, add a new column
                                item = new FileSystemExplorerItem();
                                item.CurrentTypeFace = ItemsTypeFace;
                                item.SelectionBackgroundColor = itemsSelectionBackgroundColor;
                                item.SelectionBorderColor = itemsSelectionBorderColor;
                                item.ShowBorder = showGrid;
                                item.BackgroundColor = alternatesBackgroundColor ? i % 2 != 0 ? itemsBackgroundColorFirst : itemsBackgroundColorSecond : itemsBackgroundColorFirst;
                                item.ForegroundColor = itemsForegroundColor;
                                item.ForegroundSelectionColor = itemsSelectionForegroundColor;
                                item.ItemsBorderColorFirst = itemsBorderColorFirst;
                                item.ItemsBorderColorSecond = itemsBorderColorSecond;
                                item.Height = itemsHeight;
                                //item.FontSize = itemsFontSize;
                                Canvas.SetTop(item, yCoordinate);
                                item.Layout = layout;
                                container.Children.Add(item);
                            }
                            else
                            {
                                try
                                {
                                    // re-use a previously drawn item
                                    item = container.Children.OfType<FileSystemExplorerItem>().ToArray()[i];
                                    item.IsVisible = true;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }
                            }

                            Canvas.SetLeft(item, xCoordinate);
                            item.Width = autosizeWidth ? widestItem : itemsWidth;
                            // if the next item's Y coordinate will go beyond canvas's height, reset it and move to a new column 
                            if (yCoordinate > (numberOfVerticalItems - 2) * (itemsHeight + itemsVerticalSpacing))
                            {
                                yCoordinate = 0;
                                xCoordinate += (autosizeWidth ? widestItem : itemsWidth) + itemsHorizontalSpacing;
                                // if automatic width is enabled, calculate which will be the widest element in the new column
                                if (autosizeWidth)
                                    widestItem = GetWidestElementInColumn((currentIndex + ++currentColumn) * numberOfVerticalItems);
                            }
                            else
                                yCoordinate += itemsHeight + itemsVerticalSpacing;

                            break;
                        case FileSystemExplorerLayouts.Icons:
                            if (i >= container.Children.OfType<FileSystemExplorerItem>().Count())
                            {
                                // when more columns fit when recycling items than they were initially created when drawn, add a new column
                                item = new FileSystemExplorerItem();
                                item.CurrentTypeFace = ItemsTypeFace;
                                item.SelectionBackgroundColor = itemsSelectionBackgroundColor;
                                item.SelectionBorderColor = itemsSelectionBorderColor;
                                item.ShowBorder = showGrid;
                                item.BackgroundColor = alternatesBackgroundColor ? i % 2 != 0 ? itemsBackgroundColorFirst : itemsBackgroundColorSecond : itemsBackgroundColorFirst;
                                item.ForegroundColor = itemsForegroundColor;
                                item.ForegroundSelectionColor = itemsSelectionForegroundColor;
                                item.ItemsBorderColorFirst = itemsBorderColorFirst;
                                item.ItemsBorderColorSecond = itemsBorderColorSecond;
                                item.Height = itemsWidth + 20;
                                item.Width = itemsWidth;
                                // item.FontSize = itemsFontSize;
                                Canvas.SetTop(item, yCoordinate);
                                Canvas.SetLeft(item, xCoordinate);
                                item.Layout = layout;
                                container.Children.Add(item);
                            }
                            else
                            {
                                // re-use a previously drawn item
                                item = container.Children.OfType<FileSystemExplorerItem>().ToArray()[i];
                                item.IsVisible = true;
                            }

                            Canvas.SetTop(item, yCoordinate);
                            // if the next item's X coordinate will go beyond canvas's width, reset it and move to a new row 
                            if (xCoordinate > (numberOfHorizontalItems - 2) * (itemsWidth + itemsHorizontalSpacing))
                            {
                                xCoordinate = 0;
                                yCoordinate += itemsWidth + 20 + itemsVerticalSpacing;
                            }
                            else
                                xCoordinate += itemsWidth + itemsHorizontalSpacing;
                            break;
                        case FileSystemExplorerLayouts.Details:
                            item = container.Children.OfType<FileSystemExplorerItem>().ToArray()[i];
                            item.IsVisible = true;
                            Canvas.SetTop(item, yCoordinate);
                            item.Width = container.Bounds.Width - 15;
                            yCoordinate += itemsHeight + itemsVerticalSpacing;
                            break;
                    }
                    if (i < virtualizedItems.Length)
                    {
                        item.Type = virtualizedItems[i].FileSystemItemType == FileSystemItemTypes.File ? "File" : "Directory";
                        item.DateModified = virtualizedItems[i].Date;
                        item.Size = virtualizedItems[i].Size;
                        item.Text = virtualizedItems[i].Name;
                        item.Path = virtualizedItems[i].Path;
                        item.FileSystemItemType = virtualizedItems[i].FileSystemItemType;
                        item.IconSource = virtualizedItems[i].IconSource;
                        item.UpdateItem();
                    }
                    else
                        item.IsVisible = false; // when last column doesnt fill visual area, hide remainder of created items 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            // if there are existing created items that are not displayed (ie: when displaying the last column, that doesn't fill all the available space), hide them  
            if (virtualizedItems.Length < container.Children.OfType<FileSystemExplorerItem>().Count())
                for (int j = virtualizedItems.Length; j < container.Children.OfType<FileSystemExplorerItem>().Count(); j++)
                    (container.Children.OfType<FileSystemExplorerItem>().ToArray()[j]).IsVisible = false;
        }
        
        /// <summary>
        /// Draws the file system items that fit in the visible area of the canvas
        /// </summary>
        private void DrawUIElements()
        {
            foreach (FileSystemExplorerItem control in container.Children.OfType<FileSystemExplorerItem>().ToArray())
                container.Children.Remove(control);
             //container.Children.OfType<FileSystemExplorerItem>().ForEach(item => item.rem);
            if (virtualizedItems == null || virtualizedItems.Length == 0) 
                return;
            double xCoordinate = 0, yCoordinate = 0;
            int currentColumn = 0;
            // determine the width of the first column
            double widestItem = autosizeWidth && layout == FileSystemExplorerLayouts.List ? GetWidestElementInColumn(currentIndex * numberOfVerticalItems) : itemsWidth;
            // add all visible file system items
            for (int i = 0; i < numberOfVisibleItems; i++)
            {
                FileSystemExplorerItem item = new FileSystemExplorerItem();
                item.CurrentTypeFace = itemsTypeFace;
                item.SelectionBackgroundColor = itemsSelectionBackgroundColor;
                item.SelectionBorderColor = itemsSelectionBorderColor;
                item.ShowBorder = showGrid;
                item.BackgroundColor = alternatesBackgroundColor ? i % 2 != 0 ? itemsBackgroundColorFirst : itemsBackgroundColorSecond : itemsBackgroundColorFirst;
                item.ForegroundColor = itemsForegroundColor;
                item.ForegroundSelectionColor = itemsSelectionForegroundColor;
                item.FontSize = itemsFontSize;
                item.ItemsBorderColorFirst = itemsBorderColorFirst;
                item.ItemsBorderColorSecond = itemsBorderColorSecond;
                switch (Layout)
                {
                    case FileSystemExplorerLayouts.List:
                        item.Height = itemsHeight;
                        item.Width = autosizeWidth ? widestItem : itemsWidth;
                        Canvas.SetLeft(item, xCoordinate);
                        Canvas.SetTop(item, yCoordinate);
                        // if the next item's Y coordinate will go beyond canvas's height, reset it and move to a new column 
                        if (yCoordinate > (numberOfVerticalItems - 2) * (itemsHeight + itemsVerticalSpacing))
                        {
                            yCoordinate = 0;
                            xCoordinate += (autosizeWidth ? widestItem : itemsWidth) + itemsHorizontalSpacing;
                            // if automatic width is enabled, calculate which will be the widest element in the new column
                            if (autosizeWidth)
                                widestItem = GetWidestElementInColumn((currentIndex + ++currentColumn) * numberOfVerticalItems);
                        }
                        else
                            yCoordinate += itemsHeight + itemsVerticalSpacing;
                        break;
                    case FileSystemExplorerLayouts.Icons:
                        item.Height = itemsWidth + 20;
                        item.Width = itemsWidth;
                        Canvas.SetLeft(item, xCoordinate);
                        Canvas.SetTop(item, yCoordinate);
                        // if the next item's X coordinate will go beyond canvas's width, reset it and move to a new row 
                        if (xCoordinate > (numberOfHorizontalItems - 2) * (itemsWidth + itemsHorizontalSpacing))
                        {
                            xCoordinate = 0;
                            yCoordinate += itemsWidth + 20 + itemsVerticalSpacing;
                        }
                        else
                            xCoordinate += itemsWidth + itemsHorizontalSpacing;
                        break;
                    case FileSystemExplorerLayouts.Details:
                        item.Height = itemsHeight;
                        item.Width = container.Bounds.Width - 15;
                        Canvas.SetLeft(item, xCoordinate);
                        Canvas.SetTop(item, yCoordinate);
                        yCoordinate += itemsHeight + itemsVerticalSpacing;
                        break;
                }
                if (i < virtualizedItems.Length)
                {
                    item.Type = virtualizedItems[i].FileSystemItemType == FileSystemItemTypes.File ? "File" : "Directory";
                    item.DateModified = virtualizedItems[i].Date;
                    item.Size = virtualizedItems[i].Size;
                    item.Text = virtualizedItems[i].Name;
                    item.Path = virtualizedItems[i].Path;
                    if (!string.IsNullOrEmpty(virtualizedItems[i].IconSource))
                        item.IconSource = virtualizedItems[i].IconSource;
                    item.FileSystemItemType = virtualizedItems[i].FileSystemItemType;
                }
                else
                    item.IsVisible = false; // when last column doesnt fill visual area, hide remainder of created items 
                item.Layout = layout;
                container.Children.Add(item);
            }
        }

        /// <summary>
        /// Gets another data chunk either after or before the displayed chunk
        /// </summary>
        /// <param name="fetchForward">Whether to fetch data after or before the displayed chunk</param>
        private void FetchMoreData(bool fetchForward)
        {
            if (fetchForward) 
            {
                if (layout == FileSystemExplorerLayouts.List)
                {
                    // move items to the left while the rightmost item doesn't go beyond first column 
                    if (currentIndex * numberOfVerticalItems < Items.Count - numberOfVerticalItems)
                    { 
                        HorizontalScrollBarValue++;
                        currentIndex++;
                    }
                    else return; 
                }
                else if (layout == FileSystemExplorerLayouts.Details || layout == FileSystemExplorerLayouts.Icons)
                {
                    if (currentIndex < Items.Count - numberOfVerticalItems + 1)
                    {
                        VerticalScrollBarValue++;
                        if ((Items.Count - numberOfVerticalItems) - currentIndex > (layout == FileSystemExplorerLayouts.Details ? 3 : 1))
                            currentIndex += (layout == FileSystemExplorerLayouts.Details ? 3 : 1);
                        else
                            currentIndex += (Items.Count - numberOfVerticalItems + 1) - currentIndex;
                    }
                    else return;
                }
            }
            else
            {
                if (layout == FileSystemExplorerLayouts.List)
                {
                    // move items to the right while the current page is not the first page
                    if (currentIndex > 0)
                    {  
                        HorizontalScrollBarValue--;
                        currentIndex--;
                    }
                    else return;
                }
                else if (layout == FileSystemExplorerLayouts.Details || layout == FileSystemExplorerLayouts.Icons)
                {
                    if (currentIndex > (layout == FileSystemExplorerLayouts.Details ? 3 : 1))
                    {
                        VerticalScrollBarValue--;
                        currentIndex -= (layout == FileSystemExplorerLayouts.Details ? 3 : 1);
                    }
                    else
                    {
                        VerticalScrollBarValue = 0;
                        currentIndex = 0;
                    }
                }
            }
            numberOfVisibleItems = CalculateNumberOfVisibleItems();
            FetchDataChunk();
            RecycleUIElements();
        }
        
        /// <summary>
        /// Gets the data portion that will be displayed in the UI
        /// </summary>
        private void FetchDataChunk()
        {
            // if all items fit in the visible area, set current index to 0
            if (numberOfVisibleItems >= Items.Count())
                currentIndex = 0;
            if (layout == FileSystemExplorerLayouts.List)
            {
                // when dealing a large, sudden resize (ie: maximizing from a small size), numberOfVerticalItems can increase dramatically in a single measure pass - 
                // decrease current index until it fits a valid range
                while (currentIndex * numberOfVerticalItems >= Items.Count())
                    currentIndex--;
                virtualizedItems = Items.Skip(currentIndex * numberOfVerticalItems)
                                 .Take(numberOfVisibleItems)
                                 .ToArray();
            }
            else if (layout == FileSystemExplorerLayouts.Icons)
            {
                // when dealing a large, sudden resize (ie: maximizing from a small size), numberOfHorizontalItems can increase dramatically in a single measure pass - 
                // decrease current index until it fits a valid range
                while (currentIndex * numberOfHorizontalItems >= Items.Count())
                    currentIndex--;
                virtualizedItems = Items.Skip(currentIndex * numberOfHorizontalItems)
                                 .Take(numberOfVisibleItems)
                                 .ToArray();
            }
            else if (layout == FileSystemExplorerLayouts.Details)
                virtualizedItems = Items.Skip(currentIndex)
                                 .Take(numberOfVisibleItems)
                                 .ToArray();
        }

        /// <summary>
        /// Measures the number of items that fit in the visible area of the canvas
        /// </summary>
        /// <returns>The number of file system items that fit in the visible area of the canvas</returns>
        private int CalculateNumberOfVisibleItems()
        {
            switch (Layout)
            {
                case FileSystemExplorerLayouts.List:
                {
                    double width = 0;
                    // need to start from current index so that measurements reflect next columns
                    int columns = currentIndex;
                    // container.Bounds.Height 811 itemsHeight 20 itemsVerticalSpacing 1
                    numberOfVerticalItems = (int)(container.Bounds.Height / (itemsHeight + itemsVerticalSpacing));
                    // calculate number of columns in the visible area based on their visible measurement or the user provided width
                    while (width < container.Bounds.Width)
                    {
                        if (autosizeWidth)
                            width += GetWidestElementInColumn(columns++ * numberOfVerticalItems);
                        else
                        {
                            width += itemsWidth + itemsHorizontalSpacing;
                            columns++;
                        }
                    }
                    // add one extra column that is only partially visible, if there is remainder space
                    if (container.Bounds.Width - width > 0)
                        columns++;
                    // revert from current index based column count
                    numberOfHorizontalItems = columns - currentIndex;
                    break;
                }
                case FileSystemExplorerLayouts.Details:
                {
                    numberOfVerticalItems = (int)(container.Bounds.Height / (itemsHeight + itemsVerticalSpacing));
                    if (container.Bounds.Height - (int)(container.Bounds.Height / (itemsHeight + itemsVerticalSpacing)) > 0)
                        numberOfVerticalItems++;
                    numberOfHorizontalItems = 1;
                    break;
                }
                case FileSystemExplorerLayouts.Icons:
                {
                    double height = 0;
                    // need to start from current index so that measurements reflect next columns
                    int rows = currentIndex;
                    numberOfHorizontalItems = (int)(container.Bounds.Width / (itemsWidth + itemsHorizontalSpacing));
                    // calculate number of rows in the visible area based on their visible measurement or the user provided height
                    // (in Icons layout, height is equal to width plus 20)
                    while (height < container.Bounds.Height)
                    {
                        height += itemsWidth + 20 + itemsVerticalSpacing;
                        rows++;
                    }
                    // add one extra row that is only partially visible, if there is remainder space
                    if (container.Bounds.Height - height > 0)
                        rows++;
                    // revert from current index based row count
                    numberOfVerticalItems = rows - currentIndex;
                    break;
                }
            }
            return numberOfVerticalItems * numberOfHorizontalItems;
        }

        /// <summary>
        /// Initializes the file system explorer
        /// </summary>
        private void Initialize()
        {
            if (isWindowLoaded && Items.Count > 0)
            {
                numberOfVisibleItems = CalculateNumberOfVisibleItems();
                FetchDataChunk();
                CalculateScrollBars();
                DrawUIElements();
                grdDetails.IsVisible = layout == FileSystemExplorerLayouts.Details;
                container.Margin = new Thickness(0, layout == FileSystemExplorerLayouts.Details ? 25 : 0, 25, 25);
            }    
        }

        public async Task NavigateToPath(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            await Task.Run(() =>
            {
                temp = new List<FileSystemEntity>();
                try
                {
                    FileSystemInfo info = null;
                    temp.AddRange(Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly)
                        .Select(path => new FileSystemEntity()
                        {
                            Path = path,
                            Date = new DirectoryInfo(path).LastAccessTime,
                            FileSystemItemType = FileSystemItemTypes.Folder,
                            Name = path.Contains(Path.DirectorySeparatorChar) ? path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1) : path,
                        }));
                    temp.AddRange(Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly)
                        .Select(path =>
                        {
                            info = new FileInfo(path);
                            return new FileSystemEntity()
                            {
                                Path = path,
                                Date = info.LastAccessTime,
                                Size = (info as FileInfo).Length,
                                FileSystemItemType = FileSystemItemTypes.File,
                                IconSource = "/mnt/STORAGE/MULTIMEDIA/PHOTO/ICONS/anime.ico",
                                Name = path.Contains(Path.DirectorySeparatorChar) ? path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1) : path,
                                Extension = Path.GetExtension(path)
                            };
                        }));
                    SortItems(SortingItems.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            UpdateSortAdorners();
            Items = new AvaloniaList<FileSystemEntity>(temp);
        }
        #endregion
        
        #region ============================================================= EVENT HANDLERS ================================================================================
        /// <summary>
        /// Handles FileSystemExploer's PropertyChanged event
        /// </summary>
        private void FileSystemExploer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == BoundsProperty & isWindowLoaded)
                Initialize();
        }

        /// <summary>
        /// Handles control's PointerWheelChanged event
        /// </summary>
        private void UserControl_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            isInRenameMode = false;
            txtRename.IsVisible = false;
            // either change items size or scroll items list
            if (IsCtrlPressed && !isMouseDown) // only change items size if the mouse is not pressed (in selection mode)
            {
                if (e.Delta.Y > 0)
                {
                    if (layout == FileSystemExplorerLayouts.Icons)
                        ItemsWidth++;
                    else
                        ItemsHeight++;
                }
                else
                {
                    if (layout == FileSystemExplorerLayouts.Icons)
                        ItemsWidth--;
                    else
                        ItemsHeight--;
                }
                Initialize();
            }
            else
                FetchMoreData(e.Delta.Y < 0);
            sw.Stop();
            Trace.WriteLine(sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Handles control's PointerPressed event
        /// </summary>
        private void UserControl_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            isMouseDown = true;
            mousePoint = mouseDownPoint = e.GetPosition(container);
        }
    
        /// <summary>
        /// Handles control's PointerMoved event
        /// </summary>
        private void UserControl_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (isMouseDown)
            {
                e.Device.Capture(container);
                mousePoint = e.GetPosition(container);
                //Region r = new Region(this.ClientRectangle);

                selectionRectangle.Width = Math.Abs(mouseDownPoint.X - mousePoint.X);
                selectionRectangle.Height = Math.Abs(mouseDownPoint.Y - mousePoint.Y);
                Canvas.SetLeft(selectionRectangle, Math.Min(mouseDownPoint.X, mousePoint.X));
                Canvas.SetTop(selectionRectangle, Math.Min(mouseDownPoint.Y, mousePoint.Y));
                
                if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
                {
                    container.Children.OfType<FileSystemExplorerItem>()
                                      .Where(item => !selectionRectangle.Bounds.Intersects(item.Bounds))
                                      .ForEach(item => item.IsSelectionHovered = false);
                    container.Children.OfType<FileSystemExplorerItem>()
                                      .Where(item => selectionRectangle.Bounds.Intersects(item.Bounds))
                                      .ForEach(item => item.IsSelectionHovered = true);
                }
            }
        }

        /// <summary>
        /// Handles control's PointerReleased event
        /// </summary>
        private void UserControl_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            isMouseDown = false;
            // clicks on items should be at least one second apart, otherwise it is double click
            if (DateTime.Now.Subtract(lastClickTime) < TimeSpan.FromSeconds(1))
                return;
            lastClickTime = DateTime.Now;
            // if neither Control or Shift keys are pressed and a selection rectangle is present, deselect the items that were not inside it
            if (!IsCtrlPressed && !IsShiftPressed && selectionRectangle.Width > 0 && selectionRectangle.Bounds.Height > 0)
                container.Children.OfType<FileSystemExplorerItem>()
                    .Where(item => !selectionRectangle.Bounds.Intersects(item.Bounds))
                    .ForEach(item => item.IsSelected = false);
            // if there was a selection rectangle, mark the items inside it as selected
            if (selectionRectangle.Width > 0 && selectionRectangle.Bounds.Height > 0)
                container.Children.OfType<FileSystemExplorerItem>()
                    .Where(item => selectionRectangle.Bounds.Intersects(item.Bounds))
                    .ForEach(item => item.IsSelected = true);
         
            // if no selection rectangle is present
            if (selectionRectangle.Width == 0 && selectionRectangle.Bounds.Height == 0)
            {
                // only handle selections if the user is not renaming an item
                // (if the user is clicking outside the renaming text field, the LostFocus event will exit renaming mode, no need to handle it here)
                if (!isInRenameMode)
                {
                    // check if the mouse was clicked over an item
                    var clickedItem = container.Children.OfType<FileSystemExplorerItem>()
                        .Where(item => item.Bounds.Contains(e.GetPosition(container)))
                        .FirstOrDefault();
                    if (IsCtrlPressed)
                    {
                        
                        // if an item was clicked, store if it was already selected 
                        bool wasSelected = clickedItem?.IsSelected ?? false;
                        // if any item was clicked
                        if (clickedItem != null)
                            clickedItem.IsSelected = !clickedItem.IsSelected;
                    }
                    else if (IsShiftPressed)
                    {
                        if (clickedItem != null)
                        {
                            int initialItemIndex = lastSelectedItem != null ? Array.FindIndex(virtualizedItems, row => row.Path == lastSelectedItem.Path) : 0;
                            int clickedItemIndex = Array.FindIndex(virtualizedItems, row => row.Path == clickedItem.Path);
                            // TODO: implement with virtualization!
                            // deselect previously selected items!
                            container.Children.OfType<FileSystemExplorerItem>().ForEach(item => item.IsSelected = false);
                            for (int i = Math.Min(initialItemIndex, clickedItemIndex); i <= Math.Max(initialItemIndex, clickedItemIndex); i++)
                                container.Children.OfType<FileSystemExplorerItem>().ToArray()[i].IsSelected = true;
                        }
                    }
                    else
                    {
                        lastSelectedItem = clickedItem;
                        // store the number of selected items before deselecting them, because deferred execution
                        int selectedItemsCount = container.Children.OfType<FileSystemExplorerItem>()
                            .Where(item => item.IsSelected)
                            .Count();
                        // if an item was clicked, store if it was already selected 
                        bool wasSelected = clickedItem?.IsSelected ?? false;
                        // deselect all items
                        container.Children.OfType<FileSystemExplorerItem>()
                            .ForEach(item => item.IsSelected = false);
                        // if any item was clicked
                        if (clickedItem != null)
                        {
                            // and if it was part of a greater selection, or if it was not already selected, select it
                            if (selectedItemsCount > 1 || !wasSelected)
                            {
                                clickedItem.IsSelected = true;
                                isInRenameMode = false;
                                txtRename.IsVisible = false;
                            }
                            else // there was no previous selection group, and the clicked item was already selected - enter rename mode
                                RenameItem(clickedItem);
                        }
                    }
                }
            }

            // remove the selection rectangle
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
        }

        private void RenameTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).IsVisible = false;
                isInRenameMode = false;
            }
            else if (e.Key == Key.Enter)
            {
                (sender as TextBox).IsVisible = false;
                isInRenameMode = false;
                // perform renaming
                // handle case when multiple selected items are named
                // handle case when both folders and files are selected
            }
        }


        private void RenameTextBox_LostFocus(object? sender, RoutedEventArgs e)
        {
            // exit rename mode
            isInRenameMode = false;
            txtRename.IsVisible = false;
        }

        
        /// <summary>
        /// Handles horizontal scrollbar's Scroll event
        /// </summary>
        private void HorizontalScrollBar_OnScroll(object? sender, ScrollEventArgs e)
        {
            HorizontalScrollBarValue = Math.Round(e.NewValue);
            currentIndex = (int)horizontalScrollBarValue;
            numberOfVisibleItems = CalculateNumberOfVisibleItems();
            FetchDataChunk();
            RecycleUIElements();
        }
        
        /// <summary>
        /// Handles vertical scrollbar's Scroll event
        /// </summary>
        private void VerticalScrollBar_OnScroll(object? sender, ScrollEventArgs e)
        {
            VerticalScrollBarValue = Math.Round(e.NewValue);
            // scroll three more items from the current position (vertical scrollbar apply only on details and icons layouts)
            if ((Items.Count - numberOfVerticalItems) / (layout == FileSystemExplorerLayouts.Details ? 3 : 1) > (int)verticalScrollBarValue)
                currentIndex = (int)verticalScrollBarValue * (layout == FileSystemExplorerLayouts.Details ? 3 : 1);
            else // if there are less than three items left at the end of the list, add them too as an extra scroll unit
                currentIndex = (int)verticalScrollBarValue * (layout == FileSystemExplorerLayouts.Details ? 3 : 1) - ((Items.Count - numberOfVerticalItems) % (layout == FileSystemExplorerLayouts.Details ? 3 : 1) > 0 ? 1 : 0);
            FetchDataChunk();
            RecycleUIElements();
        }

        /// <summary>
        /// Handles sort button's Click event 
        /// </summary>
        private void BtnSort_OnClick(object? sender, RoutedEventArgs e)
        {
            // when sorting by the same currently sorted item, just change sorting direction
            if (currentSortBy == (SortingItems)(sender as Button).Tag)
                isSortedDescending = !isSortedDescending;
            SortItems((SortingItems)(sender as Button).Tag);
            UpdateSortAdorners();
            Items = new AvaloniaList<FileSystemEntity>(temp);
            Initialize();
        }
        #endregion

        private async void UserControl_OnDoubleTapped(object? sender, RoutedEventArgs e)
        {
            // check if the mouse was clicked over an item
            var clickedItem = container.Children.OfType<FileSystemExplorerItem>()
                .Where(item => item.Bounds.Contains((e as TappedEventArgs).GetPosition(container)))
                .FirstOrDefault();
            // a folder was double clicked, browse it
            if (clickedItem != null && clickedItem.FileSystemItemType == FileSystemItemTypes.Folder)
                FolderBrowsed?.Invoke(clickedItem.Path);
            else if (clickedItem != null && clickedItem.FileSystemItemType == FileSystemItemTypes.File)
            {
                // TODO: handle file launching in cross platform way
            }
            e.Handled = true;
        }

    }
}