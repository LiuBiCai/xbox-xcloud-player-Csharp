using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingallingTest.Channel
{
    public class MessageChannel : BaseChannel
    {
        public MessageChannel(string channelName, xCloudPlayer client) : base(channelName, client)
        {
           
        }
    }
}
