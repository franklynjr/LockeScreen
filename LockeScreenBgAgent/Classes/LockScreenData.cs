using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockeScreenBgAgent.Classes
{
    public class LockScreenData
    {
        private string current_image;
        private int current_indx;


        public LockScreenData()
        {
            current_indx = -1;
            current_image = null;
        }

        public string CurrentImage
        {
            get { return current_image; }
            set
            {
                if (current_image != value)
                    current_image = value;
            }
        }

        public int CurrentIndex
        {
            get { return current_indx; }
            set
            {
                if (current_indx != value)
                    current_indx = value;
            }
        }


    }
}
