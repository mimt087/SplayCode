//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SplayCode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    public partial class VirtualSpaceControl : UserControl
    {

        private static VirtualSpaceControl instance;
        public static VirtualSpaceControl Instance
        {
            get
            {
                if (instance == null) {
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
        private double zoomLevel;
        private double verticalScrollPos;
        private double horizontalScrollPos;

        private VirtualSpaceControl()
        {
            InitializeComponent();
            this.SizeChanged += sizeChanged;
            baseGrid.Width = this.ActualWidth;
            baseGrid.Height = this.ActualHeight;
            BlockList = new List<BlockControl>();
            zoomLevel = zoomSlider.Value;
            zoomSlider.ValueChanged += zoomChanged;
            verticalScrollPos = 0;
            horizontalScrollPos = 0;
            ScrollView.ScrollChanged += scrollChanged;
        }

        private void scrollChanged(object sender, ScrollChangedEventArgs e)
        {
            /*if (e.VerticalChange != 0)
                verticalScrollPos = ScrollView.VerticalOffset / ScrollView.ExtentHeight;
            if (e.HorizontalChange != 0)
                horizontalScrollPos = ScrollView.HorizontalOffset / ScrollView.ExtentWidth;*/
        }

        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            ExpandToSize(ActualWidth, ActualHeight);
        }

        private void zoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double previousZoomLevel = zoomLevel;
            zoomLevel = zoomSlider.Value;
            if (baseGrid.Width * zoomLevel < this.ActualWidth)
                ExpandToSize(this.ActualWidth / zoomLevel, 0);
            if (baseGrid.Height * zoomLevel < this.ActualHeight)
                ExpandToSize(0, this.ActualHeight / zoomLevel);

            ScrollView.ScrollToVerticalOffset((ScrollView.VerticalOffset * (zoomLevel / previousZoomLevel))
                + (ScrollView.ViewportHeight * (zoomLevel / previousZoomLevel - 1) / 2));
            ScrollView.ScrollToHorizontalOffset((ScrollView.HorizontalOffset * (zoomLevel / previousZoomLevel))
                + (ScrollView.ViewportWidth * (zoomLevel / previousZoomLevel - 1) / 2));
        }

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

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.VerticalChange < 0)
            {
                Debug.Print("Up: " + e.VerticalChange);
                if (ScrollView.VerticalOffset + ScrollView.ViewportHeight + 2 >= baseGrid.Height * zoomLevel)
                {
                    ExpandToSize(0, baseGrid.Height - e.VerticalChange);
                }
            }
            else
            {
                Debug.Print("Down: " + e.VerticalChange);
                if (ScrollView.VerticalOffset - 2 <= 0)
                {
                    Thickness t = dragThumb.Margin;
                    t.Top = t.Top + e.VerticalChange;
                    dragThumb.Margin = t;
                    ExpandToSize(0, baseGrid.Height + e.VerticalChange);
                    foreach (BlockControl block in BlockList)
                    {
                        block.Reposition(0, e.VerticalChange);
                    }
                }
            }
            if (e.HorizontalChange < 0)
            {
                Debug.Print("Left: " + e.VerticalChange);
                if (ScrollView.HorizontalOffset + ScrollView.ViewportWidth + 2 >= baseGrid.Width * zoomLevel)
                {
                    ExpandToSize(baseGrid.Width - e.HorizontalChange, 0);
                }
            }
            else
            {
                Debug.Print("Right: " + e.VerticalChange);
                if (ScrollView.HorizontalOffset - 2 <= 0)
                {
                    Thickness t = dragThumb.Margin;
                    t.Left = t.Left + e.HorizontalChange;
                    dragThumb.Margin = t;
                    ExpandToSize(baseGrid.Width + e.HorizontalChange, 0);
                    foreach (BlockControl block in BlockList)
                    {
                        block.Reposition(e.HorizontalChange, 0);
                    }
                }
            }

            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - (e.VerticalChange * zoomLevel));
            ScrollView.ScrollToHorizontalOffset(ScrollView.HorizontalOffset - (e.HorizontalChange * zoomLevel));

        }

        void onDragComplete(object sender, DragCompletedEventArgs e)
        {
            Thickness t = dragThumb.Margin;
            t.Left = 0;
            t.Top = 0;
            dragThumb.Margin = t;
        }

        // Add a block using default positioning
        public void AddBlock(string label, string documentPath)
        {
            double xPos = 300 * BlockList.Count + 100;
            double yPos = 100;
            AddBlock(label, documentPath, xPos, yPos);
        }

        public void AddBlock(string label, string documentPath, double xPos, double yPos)
        {
            BlockControl newBlock = new BlockControl(label, documentPath);
            newBlock.Margin = new Thickness(xPos, yPos, 0, 0);
            BlockList.Add(newBlock);
            baseGrid.Children.Add(newBlock);
            ExpandToSize(newBlock.Margin.Left + newBlock.Width, newBlock.Margin.Top + newBlock.Height);
        }

        public void AddBlock(string label, string documentPath, double xPos, double yPos, double height, double width)
        {
            BlockControl newBlock = new BlockControl(label, documentPath);
            newBlock.Width = width;
            newBlock.Height = height;
            newBlock.Margin = new Thickness(xPos, yPos, 0, 0);
            BlockList.Add(newBlock);
            baseGrid.Children.Add(newBlock);
            ExpandToSize(newBlock.Margin.Left + newBlock.Width, newBlock.Margin.Top + newBlock.Height);
        }

        public void RemoveBlock(BlockControl block)
        {
            BlockList.Remove(block);
            baseGrid.Children.Remove(block);
        }

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