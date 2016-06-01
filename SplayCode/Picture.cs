using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SplayCode
{
    public class Picture
    {
        public double _X;
        public double _Y;
        public string _source;
        public double _height;
        public double _width;

        public Picture () { }
        public Picture (double X, double Y, string source, double height, double width)
        {
            this._X = X;
            this._Y = Y;
            this._source = source;
            this._height = height;
            this._width = width;
        }
    }
}
