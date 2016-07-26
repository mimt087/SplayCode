using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayCode.Data
{
    public class ActionDone
    {
        private bool editorClosed;
        public bool EditorClosed
        {
            get { return editorClosed; }
            set { editorClosed = value; }
        }

        private bool editorAdded;
        public bool EditorAdded
        {
            get { return editorAdded; }
            set { editorAdded = value; }
        }

        private bool editorInteracted;
        public bool EditorInteracted
        {
            get { return editorInteracted; }
            set { editorInteracted = value; }
        }

        private double zoomLevel;
        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }

        private double virtualSpaceX;
        public double VirtualSpaceX
        {
            get { return virtualSpaceX; }
            set { virtualSpaceX = value; }
        }

        private double virtualSpaceY;
        public double VirtualSpaceY
        {
            get { return virtualSpaceY; }
            set { virtualSpaceY = value; }
        }

        private double scrollOffsetX;
        public double ScrollOffsetX
        {
            get { return scrollOffsetX; }
            set { scrollOffsetX = value; }
        }

        private double scrollOffsetY;
        public double ScrollOffsetY
        {
            get { return scrollOffsetY; }
            set { scrollOffsetY = value; }
        }

        private BlockControl movedBlock;
        public BlockControl MovedBlock
        {
            get { return movedBlock; }
            set { movedBlock = value; }
        }

        private double blockSizeX;
        public double BlockSizeX
        {
            get { return blockSizeX; }
            set { blockSizeX = value; }
        }

        private double blockSizeY;
        public double BlockSizeY
        {
            get { return blockSizeY; }
            set { blockSizeY = value; }
        }

        private double blockPositionX;
        public double BlockPositionX
        {
            get { return blockPositionX; }
            set { blockPositionX = value; }
        }

        private double blockPositionY;
        public double BlockPositionY
        {
            get { return blockPositionY; }
            set { blockPositionY = value; }
        }

        private int zIndex;
        public int ZIndex
        {
            get { return zIndex; }
            set { zIndex = value; }
        }

        public ActionDone() { }
        public ActionDone(bool eClosed, bool eAdded, bool eInteracted, double zLevel, double vSX, double vSY, double sOX, double sOY, BlockControl mBlock, double bSX, double bSY, double bPX, double bPY, int zI)
        {
            editorClosed = eClosed;
            editorAdded = eAdded;
            editorInteracted = eInteracted;
            zoomLevel = zLevel;
            virtualSpaceX = vSX;
            virtualSpaceY = vSY;
            scrollOffsetX = sOX;
            scrollOffsetY = sOY;
            movedBlock = mBlock;
            blockSizeX = bSX;
            blockSizeY = bSY;
            blockPositionX = bPX;
            blockPositionY = bPY;
            zIndex = zI;
        }
    }
}
