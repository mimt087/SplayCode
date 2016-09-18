using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayCode.Data
{
    /// <summary>
    /// This is an entity class for individual editor instances.
    /// This class contains a list of properties that are required to recreate an editor
    /// A list of this class is used as a property of XmlFormat class.
    /// </summary>
    /// 

    [Serializable()]
    public class Editor
    {
        public double X;
        public double Y;
        public string source;
        public double height;
        public double width;
        public int ZIndex;
        public int BlockId;

        public Editor() { }
        public Editor(double dx, double dy, string dsource, double dheight, double dwidth, int dZIndex, int dBlockId)
        {
            X = dx;
            Y = dy;
            source = dsource;
            height = dheight;
            width = dwidth;
            ZIndex = dZIndex;
            BlockId = dBlockId;
        }
    }
}
