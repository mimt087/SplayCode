using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SplayCode.Data
{
    /// <summary>
    /// This is a singleton class that manages the placement and manipulation of 
    /// blocks in the virtual space.
    /// </summary>
    class BlockManager
    {
        private List<BlockControl> blockList;
        public List<BlockControl> BlockList
        {
            get { return blockList; }
        }

        private List<BlockControl> selectedBlocks;
        private BlockControl activeBlock;
        public BlockControl ActiveBlock
        {
            get { return activeBlock; }
        }

        private static readonly int MINIMUM_Z_INDEX = 0;
        private int topmostZIndex;
        public int TopmostZIndex
        {
            get { return topmostZIndex; }
        }

        private static readonly int MINIMUM_BLOCK_ID = 0;
        private int currentBlockId;
        public int CurrentBlockId
        {
            get { return currentBlockId; }
        }

        private static BlockManager instance;
        public static BlockManager Instance
        {
            get
            {
                if (instance==null)
                {
                    instance = new BlockManager();
                }
                return instance;
            }
        }

        private BlockManager()
        {
            blockList = new List<BlockControl>();
            selectedBlocks = new List<BlockControl>();
            activeBlock = null;
            topmostZIndex = MINIMUM_Z_INDEX;
            currentBlockId = MINIMUM_BLOCK_ID;
        }

        /// <summary>
        /// Add a block using default properties.
        /// </summary>
        public void AddBlock(string label, string documentPath)
        {

            Point blockPosition = VirtualSpaceControl.Instance.GetNextBlockPosition(null);
            double xPos = blockPosition.X;
            double yPos = blockPosition.Y;
            double width = BlockControl.DEFAULT_BLOCK_WIDTH;
            double height = BlockControl.DEFAULT_BLOCK_HEIGHT;
            int zIndex = topmostZIndex + 1;
            int blockId = currentBlockId + 1;
            AddBlock(label, documentPath, xPos, yPos, width, height, zIndex, blockId, true);
        }

        /// <summary>
        /// Add a block at the preferred position.
        /// </summary>
        public void AddBlock(string label, string documentPath, double preferredXPos, double preferredYPos)
        {

            Point preferredPosition = new Point(preferredXPos, preferredYPos);
            Point blockPosition = VirtualSpaceControl.Instance.GetNextBlockPosition(preferredPosition);
            double xPos = blockPosition.X;
            double yPos = blockPosition.Y;
            double width = BlockControl.DEFAULT_BLOCK_WIDTH;
            double height = BlockControl.DEFAULT_BLOCK_HEIGHT;
            int zIndex = topmostZIndex + 1;
            int blockId = currentBlockId + 1;
            AddBlock(label, documentPath, xPos, yPos, width, height, zIndex, blockId, true);
        }

        /// <summary>
        /// Add a block with given parameters.
        /// </summary>
        public void AddBlock(string label, string documentPath, double xPos, 
            double yPos, double height, double width, int zIndex, int blockId, bool setActive)
        {

            UndoManager.Instance.SaveState();

            BlockControl newBlock = new BlockControl(label, documentPath, blockId);
            newBlock.Width = width;
            newBlock.Height = height;
            newBlock.Position(new Thickness(xPos, yPos, 0, 0));
            Panel.SetZIndex(newBlock, zIndex);
            if (zIndex > topmostZIndex)
            {
                topmostZIndex = zIndex;
            }
            if (blockId > currentBlockId)
            {
                currentBlockId = blockId;
            }

            blockList.Add(newBlock);
            if (setActive)
            {
                SetActiveBlock(newBlock);
            }

            VirtualSpaceControl.Instance.InsertBlock(newBlock);            
        }

        /// <summary>
        /// Replace the current state of blocks with the given state of blocks.
        /// </summary>
        public void LoadBlockStates(List<BlockState> newBlockStates)
        {
            List<BlockControl> blocksToAdd = new List<BlockControl>();
            List<BlockControl> blocksToRemove = new List<BlockControl>();
            foreach (BlockControl block in blockList)
            {
                bool hasBlockState = false;
                foreach(BlockState blockState in newBlockStates)
                {
                    // find the corresponding block in the replacement state and refresh
                    // block properties from its data
                    if (blockState.BlockId == block.BlockId)
                    {
                        block.Height = blockState.BlockHeight;
                        block.Width = blockState.BlockWidth;
                        Thickness t = block.Margin;
                        t.Left = blockState.BlockXPos;
                        t.Top = blockState.BlockYPos;
                        block.Margin = t;
                        Panel.SetZIndex(block, blockState.BlockZIndex);
                        hasBlockState = true;
                        break;
                    }
                }
                // if a block exists in the block list but not in the replacement state, remove it
                if (!hasBlockState)
                {
                    blocksToRemove.Add(block);
                }
            }
            foreach(BlockControl block in blocksToRemove)
            {
                RemoveBlock(block);
            }

            foreach(BlockState blockState in newBlockStates)
            {
                bool hasBlock = false;
                foreach(BlockControl block in blockList)
                {
                    if (block.BlockId == blockState.BlockId)
                    {
                        hasBlock = true;
                    }
                }
                if (!hasBlock)
                {
                    AddBlock(blockState.Label, blockState.DocumentPath, blockState.BlockXPos,
                        blockState.BlockYPos, blockState.BlockHeight, blockState.BlockWidth,
                        blockState.BlockZIndex, blockState.BlockId, false);
                }
            }
            SetTopmostBlockAsActive();
        }

        /// <summary>
        /// Set the given block as active. The active block gets highlight and is brought
        /// to the top.
        /// </summary>
        public void SetActiveBlock(BlockControl block)
        {
            if (Panel.GetZIndex(block) < topmostZIndex)
            {
                topmostZIndex++; // [BUG] possible but extremely unlikely to reach int limit
                Panel.SetZIndex(block, topmostZIndex);
            }
            if (activeBlock != null)
            {
                activeBlock.SetHighlight(false);
            }
            activeBlock = block;
            activeBlock.SetHighlight(true);
        }

        /// <summary>
        /// Remove the given block. If it is the active block, set the next
        /// topmost block as active.
        /// </summary>
        public void RemoveBlock(BlockControl block)
        {
            VirtualSpaceControl.Instance.DeleteBlock(block);
            blockList.Remove(block);
            RemoveBlockSelection(block);
            if (block.Equals(activeBlock))
            {
                SetTopmostBlockAsActive();                
            }
        }

        /// <summary>
        /// Remove all blocks and resets fields.
        /// </summary>
        public void RemoveAllBlocks()
        {
            foreach(BlockControl block in blockList)
            {
                VirtualSpaceControl.Instance.DeleteBlock(block);
            }
            blockList.Clear();
            selectedBlocks.Clear();
            activeBlock = null;
            topmostZIndex = MINIMUM_Z_INDEX;
        }

        /// <summary>
        /// Find the block that has the highest z-index and set it as active.
        /// If there are no blocks, nothing happens.
        /// </summary>
        private void SetTopmostBlockAsActive()
        {
            int currentZIndex = MINIMUM_Z_INDEX;
            BlockControl topmostBlock = null;
            foreach(BlockControl block in blockList)
            {
                if (Panel.GetZIndex(block) > currentZIndex)
                {
                    currentZIndex = Panel.GetZIndex(block);
                    topmostBlock = block;
                }
            }
            if (topmostBlock != null)
            {
                topmostZIndex = Panel.GetZIndex(topmostBlock);
                SetActiveBlock(topmostBlock);
            }
        }

        /// <summary>
        /// Shift the given block by the given x and y delta values.
        /// If the block is one of the selected blocks, all selected blocks
        /// will be moved similarly.
        /// </summary>
        public void ShiftBlock(BlockControl block, double xDelta, double yDelta)
        {
            if (selectedBlocks.Contains(block))
            {
                foreach (BlockControl selectedBlock in selectedBlocks)
                {
                    selectedBlock.Reposition(xDelta, yDelta);
                }
            }
            else
            {
                block.Reposition(xDelta, yDelta);
            }
        }

        /// <summary>
        /// Shift all blocks by the given x and y delta values.
        /// </summary>
        public void ShiftAllBlocks(double xDelta, double yDelta)
        {
            foreach (BlockControl block in blockList)
            {
                block.Reposition(xDelta, yDelta);
            }
        }

        public void RegisterBlockSelection(BlockControl block)
        {
            selectedBlocks.Add(block);
        }

        public void RemoveBlockSelection(BlockControl block)
        {
            selectedBlocks.Remove(block);
        }

        public void RemoveAllSelections()
        {
            foreach(BlockControl block in blockList)
            {
                block.selectionCheckBox.IsChecked = false;
            }
        }

        /// <summary>
        /// Test whether a block containing the given document already exists.
        /// </summary>
        public bool BlockAlreadyExists(string documentPath)
        {
            foreach (BlockControl block in blockList)
            {
                if (block.DocumentPath.Equals(documentPath))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
