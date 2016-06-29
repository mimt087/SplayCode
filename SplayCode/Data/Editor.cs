using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayCode.Data
{
    public class Editor
    {
        public double X;
        public double Y;
        public string source;
        public double height;
        public double width;

        public Editor() { }
        public Editor(double dx, double dy, string dsource, double dheight, double dwidth)
        {
            X = dx;
            Y = dy;
            source = dsource;
            height = dheight;
            width = dwidth;
        }
    }
}
