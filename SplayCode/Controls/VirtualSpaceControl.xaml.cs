//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SplayCode
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

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
            BlockList = new List<BlockControl>();
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
            cavRoot.Children.Add(newBlock);
        }

        public void RemoveBlock(BlockControl block)
        {
            BlockList.Remove(block);
            cavRoot.Children.Remove(block);
        }

        public void RemoveAllBlocks()
        {
            BlockList.Clear();
            cavRoot.Children.Clear();
        }

        public List<BlockControl> FetchAllBlocks()
        {
            return BlockList;            
        }
    }
}