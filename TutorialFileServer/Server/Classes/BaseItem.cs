using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TutorialFileServer.Server.Classes
{
    class BaseItem
    {
        private String mRequest;
        private Object mData;

        public String Request
        {
            get
            {
                return mRequest;
            }
            set
            {
                mRequest = value;
            }
        }

        public Object Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
            }
        }


    }
}
