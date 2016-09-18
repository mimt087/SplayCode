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
        /// <summary>
        /// Zoom level of the virtual space.
        /// </summary>
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

        /// <summary>
        /// This flag is used to ignore the 'fake' touch input caused by the scrollviewer moving
        /// as a counter action to actual touch input.
        /// </summary>
        private bool duringTouch;

        /// <summary>
        /// Flag used to disable to zoom slider change handler once.
        /// </summary>
        private bool disableZoomSliderHandler = false;

        private string currentLayoutFile = "";
        public string CurrentLayoutFile
        {
            get { return currentLayoutFile; }
            set { currentLayoutFile = value; }
        }

        // stores the scroll position for switching between editor and virtual space view
        private double horizontalScrollPos = 0;
        private double verticalScrollPos = 0;

        private VirtualSpaceControl()
        {
            InitializeComponent();
            this.SizeChanged += sizeChanged;

            baseGrid.Width = this.ActualWidth;
            baseGrid.Height = this.ActualHeight;

            zoomLevel = zoomSlider.Value;
            zoomSlider.ValueChanged += zoomChanged;

            duringTouch = false;
        }

        /// <summary>
        /// Event handler to expand the virtual space if window size changes.
        /// </summary>
        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            ExpandToSize(ActualWidth / zoomLevel, ActualHeight / zoomLevel);
        }

        /// <summary>
        /// Call this function to change the zoom level of the virtual space.
        /// </summary>
        /// <param name="zoomValue">The target zoom level, should be between min and max of the zoom slider.</param>
        /// <param name="centreZoomFocus">If set to true, adjust positions in the virtual space to keep zoom focus at centre.</param>
        /// <param name="adjustZoomSlider">If set to true, adjust the value of the zoom slider as well.</param>
        private void SetVirtualSpaceZoomLevel(double zoomValue, bool centreZoomFocus, bool adjustZoomSlider)
        {
            // if zoom value given is outside valid range
            if (zoomValue < zoomSlider.Minimum)
            {
                zoomValue = zoomSlider.Minimum;
            }
            else if (zoomValue > zoomSlider.Maximum)
            {
                zoomValue = zoomSlider.Maximum;
            }

            // actual zooming
            baseGrid.LayoutTransform = new ScaleTransform(zoomValue, zoomValue);
            if (adjustZoomSlider)
            {
                disableZoomSliderHandler = true;
                zoomSlider.Value = zoomValue;
            }
            double previousZoomLevel = ZoomLevel;
            zoomLevel = zoomValue;

            if (centreZoomFocus)
            {
                // expand the space if zooming out makes it smaller than the window space
                if (baseGrid.Width * ZoomLevel < this.ActualWidth)
                    ExpandToSize(ActualWidth / ZoomLevel, 0);
                if (baseGrid.Height * ZoomLevel < this.ActualHeight)
                    ExpandToSize(0, this.ActualHeight / ZoomLevel);

                // adjust the scroll position such that the centre of zoom
                // stays at the middle of the window, shift blocks by offset overflow if 
                // exceeds the boundary
                double vertOffset = (ScrollView.VerticalOffset * (ZoomLevel / previousZoomLevel))
                    + (ScrollView.ViewportHeight * (ZoomLevel / previousZoomLevel - 1) / 2);
                if (vertOffset < 0)
                {
                    BlockManager.Instance.ShiftAllBlocks(0, Math.Abs(vertOffset) / ZoomLevel);
                }
                else if (vertOffset + ScrollView.ViewportHeight > baseGrid.Height * ZoomLevel)
                {
                    BlockManager.Instance.ShiftAllBlocks(0, 
                        (baseGrid.Height * ZoomLevel - (vertOffset + ScrollView.ViewportHeight)) / zoomLevel);
                }
                double horizOffset = (ScrollView.HorizontalOffset * (ZoomLevel / previousZoomLevel))
                    + (ScrollView.ViewportWidth * (ZoomLevel / previousZoomLevel - 1) / 2);
                if (horizOffset < 0)
                {
                    BlockManager.Instance.ShiftAllBlocks(Math.Abs(horizOffset) / ZoomLevel, 0);
                }
                else if (horizOffset + ScrollView.ViewportWidth > baseGrid.Width * ZoomLevel)
                {
                    BlockManager.Instance.ShiftAllBlocks(
                        (baseGrid.Width * ZoomLevel - (horizOffset + ScrollView.ViewportWidth)) / zoomLevel , 0);
                }

                // actual scroll command
                ScrollView.ScrollToVerticalOffset(vertOffset);
                ScrollView.ScrollToHorizontalOffset(horizOffset);
            }
        }

        /// <summary>
        /// Event handler for changes in the zoom slider value.
        /// </summary>
        private void zoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (disableZoomSliderHandler)
            {
                disableZoomSliderHandler = false;
            }
            else
            {
                SetVirtualSpaceZoomLevel(zoomSlider.Value, true, false);
            }
        }

        /// <summary>
        /// Event handler for touch input.
        /// </summary>
        void manipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!duringTouch)
            {
                // set flag to negate the next input caused by counter action
                duringTouch = true;

                // if zoom gesture detected
                if (Math.Abs(e.DeltaManipulation.Scale.X - 1) > 0.0001 || Math.Abs(e.DeltaManipulation.Scale.Y - 1) > 0.0001)
                {
                    SetVirtualSpaceZoomLevel(ZoomLevel * e.DeltaManipulation.Scale.X, true, true);
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

        /// <summary>
        /// Event handler for touch input completion.
        /// </summary>
        void manipulationComplete(object sender, ManipulationCompletedEventArgs e)
        {
            resetThumbLocation();
        }

        /// <summary>
        /// Event handler for zoom in button.
        /// </summary>
        void zoomIn(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = zoomSlider.Value + zoomSlider.LargeChange;
        }

        /// <summary>
        /// Event handler for zoom out button.
        /// </summary>
        void zoomOut(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = zoomSlider.Value - zoomSlider.LargeChange;
        }

        /// <summary>
        /// Expands the size of the virtual space if the given size is larger than current size.
        /// </summary>
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

        /// <summary>
        /// Call this function to move the virtual space by the given delta values.
        /// </summary>
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

        /// <summary>
        /// Resets the location of the dragging thumb after a drag.
        /// </summary>
        private void resetThumbLocation()
        {
            Thickness t = dragThumb.Margin;
            t.Left = 0;
            t.Top = 0;
            dragThumb.Margin = t;
        }

        /// <summary>
        /// Event handler for mouse drag on virtual space.
        /// </summary>
        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            translateVirtualSpace(e.HorizontalChange, e.VerticalChange);
        }

        void onDragComplete(object sender, DragCompletedEventArgs e)
        {
            resetThumbLocation();
        }

        /// <summary>
        /// Load parameters for the virtual space from a saved layout.
        /// </summary>
        public void LoadLayoutSettings(double virtualSpaceX, double virtualSpaceY, double scrollOffsetH, double scrollOffsetV, double zoomLv)
        {
            SetVirtualSpaceZoomLevel(zoomLv, false, true);
            baseGrid.Width = virtualSpaceX;
            baseGrid.Height = virtualSpaceY;
            ScrollView.ScrollToHorizontalOffset(scrollOffsetH);
            ScrollView.ScrollToVerticalOffset(scrollOffsetV);
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
            SetVirtualSpaceZoomLevel(1.0, false, true);
            ScrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            ScrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            baseGrid.Width = (ActualWidth / ZoomLevel);
            baseGrid.Height = (ActualHeight / ZoomLevel);
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

        /// <summary>
        /// Event handler for the drag-over of the drag-n-drop operations.
        /// </summary>
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

        /// <summary>
        /// Event handler for the drop of the drag-n-drop operations.
        /// </summary>
        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Handled = true;
                string filePath = (string)e.Data.GetData(DataFormats.StringFormat);
                Point cursorPosition = e.GetPosition(dragThumb);
                ImportManager.Instance.AddSingleOrMultipleFiles(filePath, cursorPosition, false);
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = true;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool doNotResetBlockCounter = false;
                if (files.Length > 1)
                {
                    doNotResetBlockCounter = true;
                }
                Point cursorPosition = e.GetPosition(dragThumb);
                foreach (string s in files)
                {
                    ImportManager.Instance.AddSingleOrMultipleFiles(s, cursorPosition, doNotResetBlockCounter);
                }
                ResetMultipleBlockCounter();                
            }
            base.OnDrop(e);
        }

        /// <summary>
        /// Find the next best place to put a block, considering the scenario of adding
        /// multiple blocks. If the preferred position is null, calculations begin at the
        /// top left space of the viewport. Otherwise it starts at the preferred position.
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
                double xPos = (ScrollView.HorizontalOffset / ZoomLevel) + ((ScrollView.ViewportWidth / zoomLevel) / 2) 
                    - (BlockControl.DEFAULT_BLOCK_WIDTH / 2) + (BLOCK_DISPLACEMENT_DISTANCE * multipleBlockCounter);
                double yPos = (ScrollView.VerticalOffset / ZoomLevel) + ((ScrollView.ViewportHeight / zoomLevel) / 2)
                    - (BlockControl.DEFAULT_BLOCK_HEIGHT / 2) + (BLOCK_DISPLACEMENT_DISTANCE * multipleBlockCounter);
                nextBlockPosition.X = xPos;
                nextBlockPosition.Y = yPos;
            }
            multipleBlockCounter++;
            return nextBlockPosition;
        }

        /// <summary>
        /// Resets the counter for adding multiple blocks.
        /// </summary>
        public void ResetMultipleBlockCounter()
        {
            multipleBlockCounter = 0;
        }

        /// <summary>
        /// Center view on the given block in the virtual space.
        /// </summary>
        public void CenterViewOn(BlockControl block)
        {
            SetVirtualSpaceZoomLevel(1.0, true, true);
            ScrollView.ScrollToHorizontalOffset(block.Margin.Left - (ScrollView.ViewportWidth / 10));
            ScrollView.ScrollToVerticalOffset(block.Margin.Top - (ScrollView.ViewportHeight / 10));
        }

        /// <summary>
        /// Temporarily records the current scroll offsets.
        /// </summary>
        public void SaveScrollOffsets()
        {
            horizontalScrollPos = ScrollView.HorizontalOffset;
            verticalScrollPos = ScrollView.VerticalOffset;
        }

        /// <summary>
        /// Load the recorded scroll offsets.
        /// </summary>
        public void LoadScrollOffsets()
        {
            ScrollView.ScrollToHorizontalOffset(horizontalScrollPos);
            ScrollView.ScrollToVerticalOffset(verticalScrollPos);
        }

        /// <summary>
        /// Switch to the full-window editor view for the given editor block.
        /// </summary>
        public void EnterEditorView(BlockControl block)
        {
            SaveScrollOffsets();
            EditorViewControl.Instance.SetEditor(block);
            Content = EditorViewControl.Instance;
            SplayCodeToolWindow.SetEditorViewMode(true);
        }

        /// <summary>
        /// Switch back to the virtual space from the full-window editor view.
        /// </summary>
        public void ExitEditorView()
        {
            SplayCodeToolWindow.SetEditorViewMode(false);
            Content = virtualSpace;
            LoadScrollOffsets();           
        }

        /// <summary>
        /// Event handler for double-clicking on empty spots in the virtual space.
        /// </summary>
        private void dragThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BlockManager.Instance.RemoveAllSelections();
        }
    }
}