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

        private int blockID;
        public int BlockId
        {
            get { return blockID; }
            set { blockID = value; }
        }

        public ActionDone() { }
        public ActionDone(bool eClosed, bool eAdded, bool eInteracted, BlockControl mBlock, double bSX, double bSY, double bPX, double bPY, int zI, int bId)
        {
            editorClosed = eClosed;
            editorAdded = eAdded;
            editorInteracted = eInteracted;
            movedBlock = mBlock;
            blockSizeX = bSX;
            blockSizeY = bSY;
            blockPositionX = bPX;
            blockPositionY = bPY;
            zIndex = zI;
            blockID = bId;
        }
    }
}
