using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayCode.Data
{
    /// <summary>
    /// This is an entity class that is used for serialising spatial layout information to an XML file
    /// and deserialising the XML file to an identical spatial layout
    /// The properties of this class specify some information about the virtual space and the editor blocks
    /// </summary>
    [Serializable()]
    public class XmlFormat
    {
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

        private double scrollOffsetHorizontal;
        public double ScrollOffsetHorizontal
        {
            get { return scrollOffsetHorizontal; }
            set { scrollOffsetHorizontal = value; }
        }

        private double scrollOffsetVertical;
        public double ScrollOffsetVertical
        {
            get { return scrollOffsetVertical; }
            set { scrollOffsetVertical = value; }
        }

        private double zoomLevel;
        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }

        private List<Editor> editors = new List<Editor>();
        public List<Editor> Editors
        {
            get { return editors; }
            set { editors = value; }
        }
    }
}
