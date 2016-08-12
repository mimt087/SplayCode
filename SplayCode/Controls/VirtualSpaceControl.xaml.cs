using SplayCode.Controls;
using SplayCode.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SplayCode
{
    /// <summary>
    /// Represents the scrollable, zoomable, self-expanding virtual space
    /// in SplayCode.
    /// </summary>
    public partial class VirtualSpaceControl : UserControl
    {
        private static VirtualSpaceControl instance;
        public static VirtualSpaceControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VirtualSpaceControl();
                }
                return instance;
            }
        }

        private double zoomLevel;
        public double ZoomLevel
        {
            get { return zoomLevel; }
        }

        /// <summary>
        /// The displacement distance for adding multiple blocks.
        /// </summary>
        private static readonly double BLOCK_DISPLACEMENT_DISTANCE = 50;

        /// <summary>
        /// Counts how many blocks were added at the same location.
        /// Resets when the virtual space is moved.
        /// </summary>
        private int multipleBlockCounter;

        // this flag is used to ignore the 'false' touch input caused by the scrollviewer moving
        // as a counter action to actual touch input
        private bool duringTouch;

        private string currentLayoutFile = "";
        public string CurrentLayoutFile
        {
            get { return currentLayoutFile; }
            set { currentLayoutFile = value; }
        }

        private VirtualSpaceControl()
        {
            InitializeComponent();
            this.SizeChanged += sizeChanged;

            // the size of the grid determines the size of the virtual space;
            // it's initialized to the size of tool window
            baseGrid.Width = this.ActualWidth;
            baseGrid.Height = this.ActualHeight;

            zoomLevel = zoomSlider.Value;
            zoomSlider.ValueChanged += zoomChanged;
            duringTouch = false;
            ResetMultipleBlockCounter();
        }

        // handler to expand the space if the window size changes
        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            ExpandToSize(ActualWidth / zoomLevel, ActualHeight / zoomLevel);
        }

        // handler for change in the zoom slider value; zooming is already done in the xaml binding
        private void zoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double previousZoomLevel = ZoomLevel;
            zoomLevel = zoomSlider.Value;

            // expand the space if zooming out makes it smaller than the window space
            if (baseGrid.Width * ZoomLevel < this.ActualWidth)
                ExpandToSize(ActualWidth / ZoomLevel, 0);
            if (baseGrid.Height * ZoomLevel < this.ActualHeight)
                ExpandToSize(0, this.ActualHeight / ZoomLevel);

            // automatically adjust the position in the scrollviewer such that the centre of zoom
            // stays at the middle of the window
            double vertOffset = (ScrollView.VerticalOffset * (ZoomLevel / previousZoomLevel))
                + (ScrollView.ViewportHeight * (ZoomLevel / previousZoomLevel - 1) / 2);
            if (vertOffset < 0 || vertOffset + ScrollView.ViewportHeight > ScrollView.ExtentHeight)
            {
                ExpandToSize(0, baseGrid.Height + Math.Abs(vertOffset));
            }
            double horizOffset = (ScrollView.HorizontalOffset * (ZoomLevel / previousZoomLevel))
                + (ScrollView.ViewportWidth * (ZoomLevel / previousZoomLevel - 1) / 2);
            if (horizOffset < 0 || horizOffset + ScrollView.ViewportWidth > ScrollView.ExtentWidth)
            {
                ExpandToSize(baseGrid.Width + Math.Abs(horizOffset), 0);
            }
            ScrollView.ScrollToVerticalOffset(vertOffset);
            ScrollView.ScrollToHorizontalOffset(horizOffset);
        }

        // handler for touch input
        void manipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!duringTouch)
            {
                // set flag to negate the next input caused by counter action
                duringTouch = true;

                // if zoom gesture detected
                if (Math.Abs(e.DeltaManipulation.Scale.X - 1) > 0.0001 || Math.Abs(e.DeltaManipulation.Scale.Y - 1) > 0.0001)
                {
                    zoomSlider.Value = zoomSlider.Value * e.DeltaManipulation.Scale.X;
                }
                // drag space if not zoom
                else
                {
                    translateVirtualSpace(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                }
            }
            else
            {
                duringTouch = false;
            }
        }

        void manipulationComplete(object sender, ManipulationCompletedEventArgs e)
        {
            resetThumbLocation();
        }

        // handlers for zoom buttons
        void zoomIn(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = zoomSlider.Maximum;
        }

        void zoomOut(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = zoomSlider.Minimum;
        }

        // expands the size of the virtual space if the given size is larger than current size
        public void ExpandToSize(double width, double height)
        {
            if (width > baseGrid.Width)
            {
                baseGrid.Width = width;
            }
            if (height > baseGrid.Height)
            {
                baseGrid.Height = height;
            }
        }

        // move the virtual space by the given delta values
        private void translateVirtualSpace(double horizontalDelta, double verticalDelta)
        {
            // if dragging up, scroll down, expand space if needed
            if (verticalDelta < 0)
            {
                if (ScrollView.VerticalOffset + ScrollView.ViewportHeight + 2 >= baseGrid.Height * ZoomLevel)
                {
                    ExpandToSize(0, baseGrid.Height - verticalDelta);
                }
            }
            // if dragging down, scroll up, expand space if neeeded
            else
            {
                if (ScrollView.VerticalOffset - 2 <= 0)
                {
                    Thickness t = dragThumb.Margin;
                    t.Top = t.Top + verticalDelta;
                    dragThumb.Margin = t;
                    ExpandToSize(0, baseGrid.Height + verticalDelta);
                    BlockManager.Instance.ShiftAllBlocks(0, verticalDelta);
                }
            }
            // if dragging left, scroll right, expand space if neeeded
            if (horizontalDelta < 0)
            {
                if (ScrollView.HorizontalOffset + ScrollView.ViewportWidth + 2 >= baseGrid.Width * ZoomLevel)
                {
                    ExpandToSize(baseGrid.Width - horizontalDelta, 0);
                }
            }
            // if dragging right, scroll left, expand space if neeeded
            else
            {
                if (ScrollView.HorizontalOffset - 2 <= 0)
                {
                    Thickness t = dragThumb.Margin;
                    t.Left = t.Left + horizontalDelta;
                    dragThumb.Margin = t;
                    ExpandToSize(baseGrid.Width + horizontalDelta, 0);
                    BlockManager.Instance.ShiftAllBlocks(horizontalDelta, 0);
                }
            }

            // perform scrolling
            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - (verticalDelta * ZoomLevel));
            ScrollView.ScrollToHorizontalOffset(ScrollView.HorizontalOffset - (horizontalDelta * ZoomLevel));

            // resets multiple block counter when virtual space is moved
            ResetMultipleBlockCounter();
        }

        // resets the location of the dragging thumb after a drag
        private void resetThumbLocation()
        {
            Thickness t = dragThumb.Margin;
            t.Left = 0;
            t.Top = 0;
            dragThumb.Margin = t;
        }

        // handles mouse input of dragging on the virtual space
        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            translateVirtualSpace(e.HorizontalChange, e.VerticalChange);
        }

        void onDragComplete(object sender, DragCompletedEventArgs e)
        {
            resetThumbLocation();
        }

        // bring all the settings such as the size of the virtual space and the scroller position
        // and zoom level from the last saved instance
        public void LoadLayoutSettings(double virtualSpaceX, double virtualSpaceY, double scrollOffsetH, double scrollOffsetV, double zoomLv)
        {
            baseGrid.Width = virtualSpaceX;
            baseGrid.Height = virtualSpaceY;
            ScrollView.ScrollToHorizontalOffset(scrollOffsetH);
            ScrollView.ScrollToVerticalOffset(scrollOffsetV);
            zoomSlider.Value = zoomLv;
        }

        /// <summary>
        /// Add a block to the virtual space.
        /// </summary>
        public void InsertBlock(BlockControl block)
        {
            baseGrid.Children.Add(block);
        }

        /// <summary>
        /// Remove a block from the virtual space.
        /// </summary>
        public void DeleteBlock(BlockControl block)
        {
            baseGrid.Children.Remove(block);
        }

        /// <summary>
        /// Reset all virtual space properties to default.
        /// </summary>
        public void Reset()
        {
            zoomSlider.Value = 1.0;
            ScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            ScrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            baseGrid.Width = ActualWidth;
            baseGrid.Height = ActualHeight;
            ScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            ScrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            currentLayoutFile = "";
            ResetMultipleBlockCounter();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            e.Handled = true;
            base.OnDragEnter(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Handled = true;
                e.Effects = DragDropEffects.Copy;
                string files = (string)e.Data.GetData(DataFormats.StringFormat);
            } else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = true;
                e.Effects = DragDropEffects.Copy;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file = files[0];
            }
            base.OnDragOver(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Handled = true;
                string filePath = (string)e.Data.GetData(DataFormats.StringFormat);
                Point cursorPosition = e.GetPosition(dragThumb);
                ImportManager.Instance.AddSingleOrMultipleFiles(filePath, cursorPosition);
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = true;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file = files[0];
                Point cursorPosition = e.GetPosition(dragThumb);
                ImportManager.Instance.AddSingleOrMultipleFiles(file, cursorPosition);
            }
            base.OnDrop(e);
        }

        /// <summary>
        /// Find the next best place to put a block, considering the scenario of adding
        /// multiple blocks.If the preferred position is null, calculations begin at the
        /// top left space of the viewport. Otherwise it start at the preferred position.
        /// </summary>
        public Point GetNextBlockPosition(Point? preferredPosition)
        {
            Point nextBlockPosition = new Point();
            if (preferredPosition != null)
            {
                nextBlockPosition.X = preferredPosition.Value.X + (BLOCK_DISPLACEMENT_DISTANCE * multipleBlockCounter);
                nextBlockPosition.Y = preferredPosition.Value.Y + (BLOCK_DISPLACEMENT_DISTANCE * multipleBlockCounter);              
            }
            else
            {
                double xPos = ScrollView.HorizontalOffset + 100 + (BLOCK_DISPLACEMENT_DISTANCE * multipleBlockCounter);
                double yPos = ScrollView.VerticalOffset + 100 + (BLOCK_DISPLACEMENT_DISTANCE * multipleBlockCounter);
                nextBlockPosition.X = xPos;
                nextBlockPosition.Y = yPos;
            }
            multipleBlockCounter++;
            return nextBlockPosition;
        }

        /// <summary>
        /// Resets the counter for adding multiple blocks.
        /// </summary>
        private void ResetMultipleBlockCounter()
        {
            multipleBlockCounter = 0;
        }

        /// <summary>
        /// Focus view on the given block in the virtual space.
        /// </summary>
        public void FocusViewOn(BlockControl block)
        {
            zoomSlider.Value = 1.0;
            ScrollView.ScrollToHorizontalOffset(block.Margin.Left - (ScrollView.ViewportWidth / 10));
            ScrollView.ScrollToVerticalOffset(block.Margin.Top - (ScrollView.ViewportHeight / 10));
        }

        public void EnterEditorView(BlockControl block)
        {
            SplayCodeToolWindow.SetEditorViewMode(true);
            EditorViewControl.Instance.SetEditor(block);
            Content = EditorViewControl.Instance;
        }

        public void ExitEditorView()
        {
            SplayCodeToolWindow.SetEditorViewMode(false);
            Content = virtualSpace;
        }

        private void dragThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BlockManager.Instance.RemoveAllSelections();
        }
    }
}