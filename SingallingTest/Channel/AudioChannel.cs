using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingallingTest.Channel
{
    public class AudioChannel:BaseChannel
    {
        public AudioChannel(string channelName, xCloudPlayer client) : base(channelName, client)
        {
           
        }
    }
}
