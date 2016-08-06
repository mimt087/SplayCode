namespace SplayCode.Data
{
    /* Represents a snapshot of the state of a block. */
    public class BlockState
    {
        private string label;
        public string Label
        {
            get { return label; }
        }

        private string documentPath;
        public string DocumentPath
        {
            get { return documentPath; }
        }
                   
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

        public BlockState(string docName, string docPath, double width, double height,
            double xPos, double yPos, int zIndex, int id)
        {
            label = docName;
            documentPath = docPath;
            blockWidth = width;
            blockHeight = height;
            blockXPos = xPos;
            blockYPos = yPos;
            blockZIndex = zIndex;
            blockId = id;
        }
    }
}
