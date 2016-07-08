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

        private VirtualSpaceControl()
        {
            InitializeComponent();
            this.SizeChanged += sizeChanged;
            baseGrid.Width = this.ActualWidth;
            baseGrid.Height = this.ActualHeight;
            BlockList = new List<BlockControl>();
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

        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            ExpandToSize(ActualWidth, ActualHeight);
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.VerticalChange < 0)
            {
                Debug.Print("Up: " + e.VerticalChange);
                if (ScrollView.VerticalOffset + ScrollView.ViewportHeight + 2 >= baseGrid.Height)
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
                if (ScrollView.HorizontalOffset + ScrollView.ViewportWidth + 2 >= baseGrid.Width)
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
            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - e.VerticalChange);
            ScrollView.ScrollToHorizontalOffset(ScrollView.HorizontalOffset - e.HorizontalChange);

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