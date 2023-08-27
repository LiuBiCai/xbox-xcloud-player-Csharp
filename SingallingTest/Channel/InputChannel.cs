using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingallingTest.Channel
{
    public class InputChannel:BaseChannel
    {
        public InputChannel(string channelName, xCloudPlayer client) : base(channelName, client)
        {
            
        }
    }
}
