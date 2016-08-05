using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayCode.Data
{
    class UndoManager
    {
        private static readonly int UNDO_LIMIT = 30;

        private Stack<UndoState> stateStack;

        private static UndoManager instance;
        public static UndoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UndoManager();
                }
                return instance;
            }
        }

        private UndoManager()
        {
            stateStack = new Stack<UndoState>();
        }

        public void SaveState()
        {
            UndoState newState = new UndoState(BlockManager.Instance.BlockList);
            stateStack.Push(newState);
        }

        public void Undo()
        {
            if (stateStack.Count != 0)
            {
                UndoState oldState = stateStack.Pop();
                BlockManager.Instance.LoadBlockStates(oldState.BlockStates);
            }
            /*
                // if the top action of the stack was pushed by an editor being closed, reopen it where it was
                if (action.EditorClosed)
                {
                    virtualSpace.AddBlock(virtualSpace.GetFileName(action.MovedBlock.GetEditor().getFilePath()), action.MovedBlock.GetEditor().getFilePath(),
                        action.BlockPositionX, action.BlockPositionY, action.BlockSizeY, action.BlockSizeX, action.ZIndex, action.BlockId);
                    globalStack.Pop();
                }
                // if the top action of the stack was pushed due to addition of a new editor, close it
                else if (action.EditorAdded)
                {
                    List<BlockControl> bcList = virtualSpace.FetchAllBlocks();
                    foreach (BlockControl bc in bcList)
                    {
                        if (bc.BlockId == action.BlockId)
                        {
                            virtualSpace.RemoveBlock(bc);
                            break;
                        }
                    }
                }
                // if the top action of the stack was pushed due to repositioning or resizing of an editor, position is back to where it was
                else
                {
                    action.MovedBlock.Width = action.BlockSizeX;
                    action.MovedBlock.Height = action.BlockSizeY;
                    Thickness t = action.MovedBlock.Margin;
                    t.Left = action.BlockPositionX;
                    t.Top = action.BlockPositionY;
                    action.MovedBlock.Margin = t;
                    Panel.SetZIndex(action.MovedBlock, action.ZIndex);
                }
            }*/
        }

        public void Reset()
        {
            stateStack.Clear();
        }

    }
}
