//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SplayCode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    public partial class VirtualSpaceControl : UserControl
    {

        // singleton instance for retrieval
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
            private set
            {
                instance = value;
            }
        }

        private List<BlockControl> BlockList;
        public double ZoomLevel
        {
            get;
            private set;
        }

        // this flag is used to ignore the 'false' touch input caused by the scrollviewer moving
        // as a counter action to actual touch input
        private bool duringTouch;

        private static int MINIMUM_Z_INDEX = 50;
        private BlockControl topmostBlock;
        private int topmostZIndex;

        private VirtualSpaceControl()
        {
            InitializeComponent();
            this.SizeChanged += sizeChanged;

            // the size of the grid determines the size of the virtual space;
            // it's initialized to the size of tool window
            baseGrid.Width = this.ActualWidth;
            baseGrid.Height = this.ActualHeight;

            BlockList = new List<BlockControl>();
            ZoomLevel = zoomSlider.Value;
            zoomSlider.ValueChanged += zoomChanged;
            duringTouch = false;

            topmostZIndex = MINIMUM_Z_INDEX;
        }

        // handler to expand the space if the window size changes
        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            ExpandToSize(ActualWidth / ZoomLevel, ActualHeight / ZoomLevel);
        }

        // handler for change in the zoom slider value; zooming is already done in the xaml binding
        private void zoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double previousZoomLevel = ZoomLevel;
            ZoomLevel = zoomSlider.Value;

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
            zoomSlider.Value = zoomSlider.Value + zoomSlider.LargeChange;
        }

        void zoomOut(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = zoomSlider.Value - zoomSlider.LargeChange;
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
                    foreach (BlockControl block in BlockList)
                    {
                        block.Reposition(0, verticalDelta);
                    }
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
                    foreach (BlockControl block in BlockList)
                    {
                        block.Reposition(horizontalDelta, 0);
                    }
                }
            }

            // perform scrolling
            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - (verticalDelta * ZoomLevel));
            ScrollView.ScrollToHorizontalOffset(ScrollView.HorizontalOffset - (horizontalDelta * ZoomLevel));
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

        // Add a block using default positioning
        public void AddBlock(string label, string documentPath)
        {
            double xPos = 900 * BlockList.Count + 100;
            double yPos = 100;
            AddBlock(label, documentPath, xPos, yPos, 750, 750, topmostZIndex + 1);
        }

        // Add a block with given parameters
        public void AddBlock(string label, string documentPath, double xPos, double yPos, double height, double width, int zIndex)
        {
            BlockControl newBlock = new BlockControl(label, documentPath);
            newBlock.Width = width;
            newBlock.Height = height;
            newBlock.Margin = new Thickness(xPos, yPos, 0, 0);

            // Z-indices are assumed to be unique, no checks are performed 
            // (probably should fix in future)
            Panel.SetZIndex(newBlock, zIndex);
            if (zIndex > topmostZIndex)
            {
                topmostZIndex = zIndex;
            }

            BlockList.Add(newBlock);
            baseGrid.Children.Add(newBlock);
            ExpandToSize(newBlock.Margin.Left + newBlock.Width, newBlock.Margin.Top + newBlock.Height);

        }

        // Bring the selected block to the top. There is no check for when the z-index reaches
        // the int limit but that's unlikely to happen so we'll leave it for now :)
        public void BringToTop(BlockControl block)
        {
            Panel.SetZIndex(block, topmostZIndex + 1);
            topmostZIndex++;
        }

        public void LoadLayoutSettings(double virtualSpaceX, double virtualSpaceY, double scrollOffsetH, double scrollOffsetV, double zoomLv)
        {
            baseGrid.Width = virtualSpaceX;
            baseGrid.Height = virtualSpaceY;
            ScrollView.ScrollToHorizontalOffset(scrollOffsetH);
            ScrollView.ScrollToVerticalOffset(scrollOffsetV);
            zoomSlider.Value = zoomLv;
        }

        public void RemoveBlock(BlockControl block)
        {
            BlockList.Remove(block);
            baseGrid.Children.Remove(block);
        }

        // clear all blocks and resets virtual space zoom and size
        public void Clear()
        {
            foreach (BlockControl block in BlockList)
            {
                baseGrid.Children.Remove(block);
            }
            BlockList.Clear();
            baseGrid.Width = ActualWidth;
            baseGrid.Height = ActualHeight;
            zoomSlider.Value = 1.0;
            topmostZIndex = MINIMUM_Z_INDEX;
        }

        public List<BlockControl> FetchAllBlocks()
        {
            return BlockList;
        }

        public BlockControl GetActiveBlock()
        {
            foreach (BlockControl block in BlockList)
            {
                if (block.IsKeyboardFocusWithin)
                {
                    return block;
                }
            }
            return null;
        }

    }
}