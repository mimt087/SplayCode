using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayCode.Data
{
    
    /* The central 'model' of the model-view-controller structure of SplayCode. This will be 
    the runtime repository of various elements in SplayCode. */

    class DataModel
    {

        private List<MovableBlock> blockList;
        public VirtualSpace virtualSpace
        {
            get;
            private set;
        }

        public static DataModel Instance
        {
            get;
            private set;
        }

        public static void Initialize()
        {
            Instance = new DataModel();
        }

        private DataModel()
        {
            blockList = new List<MovableBlock>();
            VirtualSpace.Initialize();
            virtualSpace = VirtualSpace.Instance;
        }

    }
}
