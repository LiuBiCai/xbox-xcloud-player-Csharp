using SIPSorcery.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SingallingTest.Channel
{
    public class BaseChannel
    {
        private xCloudPlayer _client;
        public string _channelName;
        private string _state;

        private Dictionary<string, List<Action<object>>> _events = new Dictionary<string, List<Action<object>>>()
        {
            { "state", new List<Action<object>>() }
        };

        public BaseChannel(string channelName, xCloudPlayer client)
        {
            this._channelName = channelName;
            this._client = client;
            this._state = "new";
        }

        // Events
        public virtual void OnOpen(object eventObj)
        {
            Console.WriteLine("xCloudPlayer Channels/Base.cs - [" + this._channelName + "] onOpen: " + eventObj.ToString());
            //this.setState("connected");
        }
        public virtual void OnError(object eventObj,string error)
        {
            Console.WriteLine("xCloudPlayer Channels/Base.cs - [" + this._channelName + "] OnError: " + eventObj.ToString());
            //this.setState("connected");
        }

        public virtual void OnMessage(object eventObj, RTCDataChannel dc, DataChannelPayloadProtocols protocol, byte[] data)
        {
            Console.WriteLine("xCloudPlayer Channels/Base.cs - [" + this._channelName + "] onMessage: " + eventObj.ToString());
            //this.setState("connected");
        }
        // onMessage(event) {
        //     console.log('xSDK channel/base.js - ['+this._channelName+'] onMessage:', event)
        // }

        public virtual void OnClosing(object eventObj)
        {
            Console.WriteLine("xCloudPlayer Channel/Base.cs - [" + this._channelName + "] onClosing: " + eventObj.ToString());
            //this.SetState("closing");
        }

        public virtual void OnClose(object eventObj)
        {
            Console.WriteLine("xCloudPlayer Channel/Base.cs - [" + this._channelName + "] onClose: " + eventObj.ToString());
            //this.SetState("closed");
        }

        public virtual void Destroy()
        {
            // Called when we want to destroy the channel.
        }

        public virtual void SetState(string state)
        {
            this._state = state;
            this.emitEvent("state", new { state = this._state });
        }

        // Channel functions
        public void Send(string data)
        {
            var channel = this.GetClient().GetChannel(this._channelName);

            // Encode to ArrayBuffer if not ArrayBuffer

            if (channel.readyState == RTCDataChannelState.open)
            {
                if (this._channelName != "input")
                {
                    Console.WriteLine("xCloudPlayer Channel/Base.cs - [" + this._channelName + "] Sending message: " + data.ToString());
                }
                channel.send(data);
            }
            else
            {
                Console.WriteLine("xCloudPlayer Channel/Base.cs - [" + this._channelName + "] Channel is closed. Failed to send packet: " + data.ToString());
            }
        }

        // Base functions
        public xCloudPlayer GetClient()
        {
            return this._client;
        }

        public void AddEventListener(string name, Action<object> callback)
        {
            this._events[name].Add(callback);
        }

        public void emitEvent(string name, object eventObj)
        {
            foreach (var callback in this._events[name])
            {
                callback(eventObj);
            }
        }
    }
}
