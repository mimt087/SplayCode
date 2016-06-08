using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayCode.Data
{

    // Represents the 2D virtual space of SplayCode.

    class VirtualSpace
    {

        public VirtualSpaceControl control
        {
            get;
            private set;
        }

        public static VirtualSpace Instance
        {
            get;
            private set;
        }

        public static void Initialize()
        {
            Instance = new VirtualSpace();
        }

        private VirtualSpace()
        {
            control = new VirtualSpaceControl();
        }

    }
}
