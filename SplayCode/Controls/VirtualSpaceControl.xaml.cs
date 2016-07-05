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
            baseGrid.Height = 200;
            baseGrid.Width = 200;
            BlockList = new List<BlockControl>();
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ScrollView.VerticalOffset + ScrollView.ViewportHeight + 10 >= baseGrid.Height)
                ExpandToSize(baseGrid.Height - e.VerticalChange, 0);
            if (ScrollView.HorizontalOffset + ScrollView.ViewportWidth + 10 >= baseGrid.Width)
                ExpandToSize(0, baseGrid.Width - e.HorizontalChange);

            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - e.VerticalChange);
            ScrollView.ScrollToHorizontalOffset(ScrollView.HorizontalOffset - e.HorizontalChange);
        }

        public void ExpandToSize(double height, double width)
        {
            if (height > baseGrid.Height)
                baseGrid.Height = height;
            if (width > baseGrid.Width)
                baseGrid.Width = width;
        }

        // Add a block using default positioning
        public void AddBlock(string label, string documentPath)
        {
            double xPos = 200 * BlockList.Count;
            double yPos = 50;
            AddBlock(label, documentPath, xPos, yPos);
        }

        public void AddBlock(string label, string documentPath, double xPos, double yPos)
        {
            BlockControl newBlock = new BlockControl(label, documentPath);
            newBlock.Margin = new Thickness(xPos, yPos, 0, 0);
            BlockList.Add(newBlock);
            baseGrid.Children.Add(newBlock);
            ExpandToSize(newBlock.Margin.Top + newBlock.Height, newBlock.Margin.Left + newBlock.Width);
        }

        public void AddBlock(string label, string documentPath, double xPos, double yPos, double height, double width)
        {
            BlockControl newBlock = new BlockControl(label, documentPath);
            newBlock.Width = width;
            newBlock.Height = height;
            newBlock.Margin = new Thickness(xPos, yPos, 0, 0);
            BlockList.Add(newBlock);
            baseGrid.Children.Add(newBlock);
            ExpandToSize(newBlock.Margin.Top + newBlock.Height, newBlock.Margin.Left + newBlock.Width);
        }

        public void RemoveBlock(BlockControl block)
        {
            BlockList.Remove(block);
            baseGrid.Children.Remove(block);
        }

        public void RemoveAllBlocks()
        {
            BlockList.Clear();
            baseGrid.Children.Clear();
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