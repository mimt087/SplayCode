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
        //public bool EditorClosed { get; set; }

        private bool editorAdded;

        private bool editorInteracted;
        //public bool EditorInteracted { get; set; }

        private double zoomLevel;
        //public double ZoomLevel { get; set; }

        private double virtualSpaceX;
        //public double VirtualSpaceX { get; set; }

        private double virtualSpaceY;
        //public double VirtualSpaceY { get; set; }

        private double scrollOffsetX;
        //public double ScrollOffsetX { get; set; }

        private double scrollOffsetY;
        //public double ScrollOffsetY { get; set; }

        private BlockControl movedBlock;
        //public BlockControl MovedBlock { get; set; }

        private double blockSizeX;
        //public double BlockSizeX { get; set; }

        private double blockSizeY;
        //public double BlockSizeY { get; set; }

        private double blockPositionX;
        //public double BlockPositionX { get; set; }

        private double blockPositionY;
        //public double BlockPositionY { get; set; }

        private int zIndex;

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
