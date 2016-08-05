namespace SplayCode.Data
{
    /* Represents a snapshot of the state of a block. */
    public class BlockState
    {           
        private double blockWidth;
        public double BlockWidth
        {
            get { return blockWidth; }
        }

        private double blockHeight;
        public double BlockHeight
        {
            get { return blockHeight; }
        }

        private double blockXPos;
        public double BlockXPos
        {
            get { return blockXPos; }
        }

        private double blockYPos;
        public double BlockYPos
        {
            get { return blockYPos; }
        }

        private int blockZIndex;
        public int BlockZIndex
        {
            get { return blockZIndex; }
        }

        private int blockId;
        public int BlockId
        {
            get { return blockId; }
        }

        public BlockState(double width, double height, double xPos, double yPos, int zIndex, int id)
        {
            blockWidth = width;
            blockHeight = height;
            blockXPos = xPos;
            blockYPos = yPos;
            blockZIndex = zIndex;
            blockId = id;
        }
    }
}
