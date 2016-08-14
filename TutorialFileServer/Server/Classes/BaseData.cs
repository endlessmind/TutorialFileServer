using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TutorialFileServer.Server.Classes
{
    class BaseData
    {
        private String mValue;

        public String Status
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
            }
        }
    }
}
