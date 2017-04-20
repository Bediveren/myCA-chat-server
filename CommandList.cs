using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    [Flags]
    public enum Commands : Int32
    {
        Connect =  1,
        Disconnect =  2,
        SendMessage =  3,
        RecieveMessage =  4
            //change channel
            // macro called
        //To be added
    }
}
