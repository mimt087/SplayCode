using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SplayCode.Data
{
    /* Class that represents a snapshot of the program state of SplayCode.
    Currently just involves the state of each block. */ 
    class UndoState
    {
        public List<BlockState> BlockStates
        {
            get;
            private set;
        }

        public UndoState(List<BlockControl> blockList)
        {
            BlockStates = new List<BlockState>();
            foreach(BlockControl block in blockList)
            {
                double width = block.Width;
                double height = block.Height;
                double xPos = block.Margin.Left;
                double yPos = block.Margin.Top;
                int zIndex = Panel.GetZIndex(block);
                int id = block.BlockId;
                BlockStates.Add(new BlockState(width, height, xPos, yPos, zIndex, id));
            }
        }
    }
}
