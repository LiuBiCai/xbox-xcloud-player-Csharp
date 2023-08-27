using Microsoft.Extensions.Logging;
using SingallingTest.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SingallingTest.Channel
{
    public class VideoChannel : BaseChannel
    {
        //private VideoComponent _component;
        private VideoWorker _worker;
        private List<byte[]> _videoBuffer = new List<byte[]>();
        private List<Dictionary<string, object>> _frameMetadataQueue = new List<Dictionary<string, object>>();
        private bool _keyframeNeeded = true;
        // private FpsCounter _fpsCounter;
        // private BitrateCounter _bitrateCounter;
        // private LatencyCounter _latencyCounter;

        public VideoChannel(string channelName, xCloudPlayer client) : base(channelName, client)
        {
            //  this._component = new VideoComponent(this.getClient());
            //  this._fpsCounter = new FpsCounter(this.getClient(), "video");
            //  this._bitrateCounter = new BitrateCounter(this.getClient(), "video");
            //   this._latencyCounter = new LatencyCounter(this.getClient(), "video");
        }

        public override void OnOpen(object eventObj)
        {
            //base.OnOpen(eventObj);
            Console.WriteLine("xCloudPlayer Channels/Video.cs - [" + this._channelName + "] onOpen:" + eventObj);

            //this._component.create();
            //this._fpsCounter.start();
            //this._bitrateCounter.start();
            // this._latencyCounter.start();

            // Create worker to process Video

            /*
            var blob = new Blob(new string[] { "var func = " + VideoWorker.toString() + "; func(self)" });
            this._worker = new Worker(window.URL.createObjectURL(blob));

            // Process worker messages
            this._worker.onmessage = (workerMessage) =>
            {
                if (workerMessage.data.action == "doRender")
                {
                    if (workerMessage.data.status != 200)
                    {
                        Console.WriteLine("xCloudPlayer Channels/Video.cs - Worker doRender failed:" + workerMessage.data);
                    }
                    else
                    {
                        if (this._keyframeNeeded == true && workerMessage.data.data.isKeyFrame == 1)
                        {
            this._keyframeNeeded = false;
            this.doRender(workerMessage.data.data);
        }
                        else if (this._keyframeNeeded == false)
                        {
            this.doRender(workerMessage.data.data);
        }
        }
    }
            };
     }
        /*
        public void onMessage(object eventObject)
    {
         Console.WriteLine("xCloudPlayer Channels/Video.cs - [" + this._channelName + "] onMessage:" + eventObject);
    /*
        this._bitrateCounter.countPacket(event.data.byteLength);

        this._worker.postMessage(new
        {
            action = "onPacket",
            data = new
            {
                data = event.data,
                timePerformanceNow = performance.now(),
            },
        });
    */
            // this.#bitrateCounter.packets.push(event.data.byteLength)
        }

        private void DoRender(Dictionary<string, object> frame)
        {


            /*
    if (this._component.getSource().updating == false)
    {
        byte[] framesBuffer = new byte[0];

        // Process queued frames first
        while (this._videoBuffer.Count > 0)
        {
            var newFrame = this._videoBuffer[0];
            framesBuffer = this.mergeFrames(framesBuffer, newFrame);

            this.addProcessedFrame(new Dictionary<string, object>
                {
                    { "frameData", newFrame },
                    { "firstFramePacketArrivalTimeMs", performance.now() }
                });

            this._videoBuffer.RemoveAt(0);
        }

        this.addProcessedFrame(frame);
        framesBuffer = this.mergeFrames(framesBuffer, (byte[])frame["frameData"]);

        this._bitrateCounter.countData(framesBuffer.Length);

        this._component.getSource().appendBuffer(framesBuffer);
        this._component._videoRender.play();
        // this.#bitrateCounter.video.push(frame.frameData.byteLength)
    }
    else
    {
        this._videoBuffer.Add((byte[])frame["frameData"]);
    }
           
}

private void addProcessedFrame(Dictionary<string, object> frame)
{
    frame["frameRenderedTimeMs"] = performance.now();
    this._frameMetadataQueue.Add(frame);

    this._fpsCounter.count();

    var frameProcessedMs = (performance.now() - (double)frame["firstFramePacketArrivalTimeMs"]);
    this._latencyCounter.count(frameProcessedMs);
}

public List<Dictionary<string, object>> getMetadataQueue(int size = 30)
{
    var metadataQueue = this._frameMetadataQueue.GetRange(0, Math.Min(size - 1, this._frameMetadataQueue.Count));
    this._frameMetadataQueue.RemoveRange(0, Math.Min(size - 1, this._frameMetadataQueue.Count));
    return metadataQueue;
}

public int getMetadataQueueLength()
{
    return this._frameMetadataQueue.Count;
}

private byte[] mergeFrames(byte[] buffer1, byte[] buffer2)
{
    var tmp = new byte[buffer1.Length + buffer2.Length];
    Buffer.BlockCopy(buffer1, 0, tmp, 0, buffer1.Length);
    Buffer.BlockCopy(buffer2, 0, tmp, buffer1.Length, buffer2.Length);

    return tmp;
}

public override void onClose(Event event)
{
    Console.WriteLine("xCloudPlayer Channels/Video.cs - [" + this._channelName + "] onClose:" + event);

    this._component.destroy();

    base.onClose(event);
}

public void resetBuffer()
{
    // Request key frame index
    this.getClient().getChannelProcessor("control").requestKeyframeRequest();
    this._component.resetMediaSource();

    this._keyframeNeeded = true;
}

public override void destroy()
{
    this._fpsCounter.stop();
    this._bitrateCounter.stop();
    this._latencyCounter.stop();

    if (this._worker != null)
    {
        this._worker.terminate();
    }

    this._component.destroy();

    Console.WriteLine("xCloudPlayer Channels/Video.cs - Worker terminated" + this._worker);

    // Called when we want to destroy the channel.
    base.destroy();
} */
        }
    }
}


