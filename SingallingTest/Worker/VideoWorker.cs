using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SingallingTest.Worker
{
    public  class VideoWorker
    {
        private Dictionary<uint, FrameData> _frameQueue;
        Stopwatch stopwatch = new Stopwatch();
        
        public async Task<RenderFrame> OnPacket(byte[] eventData, int timePerformanceNow)
        {
            stopwatch.Start();
            return await Task.Run(() =>
            {
                var buffer = eventData as byte[];
                var messageBuffer = buffer;

                var frameId =BitConverter.ToUInt32(messageBuffer,0);
                var timestamp = BitConverter.ToUInt32(messageBuffer, 4) / 10;
                var frameSize = BitConverter.ToUInt32(messageBuffer, 8);
                var frameOffset = BitConverter.ToUInt32(messageBuffer,12);
                var serverDataKey = BitConverter.ToUInt32(messageBuffer, 16);
                var isKeyFrame =BitConverter.ToBoolean(messageBuffer,20);
                var offset = 21;

                var frameBufferData = new byte[frameSize - frameOffset];
                Buffer.BlockCopy(buffer, (int)offset, frameBufferData, 0, (int)frameSize - (int)frameOffset);
                var frameData = new FrameData
                {
                    frameId = frameId,
                    timestamp = timestamp,
                    frameSize = frameSize,
                    frameOffset = frameOffset,
                    serverDataKey = serverDataKey,
                    isKeyFrame = isKeyFrame,
                    frameData = frameBufferData,
                };

                // Check if frame already exists
                if (!_frameQueue.ContainsKey(frameId))
                {
                    var bytesReceived = frameBufferData.Length;

                    _frameQueue.Add(frameId, new FrameData
                    {
                        frameId = frameId,
                        timestamp = frameData.timestamp,
                        frameSize = frameData.frameSize,
                        frameData = frameData.frameData,
                        bytesReceived = bytesReceived,
                        serverDataKey = frameData.serverDataKey,
                        isKeyFrame = frameData.isKeyFrame,
                        fullFrame = false,
                        firstFramePacketArrivalTimeMs = timePerformanceNow,
                        frameSubmittedTimeMs = timePerformanceNow,
                        frameDecodedTimeMs = timePerformanceNow,
                        frameRenderedTimeMs = 0,
                    });
                }
                else
                {
                    var frameDataBuffer = _frameQueue[frameId].frameData;
                    var bufferLength = (int)frameBufferData.Length;
                    Buffer.BlockCopy(frameBufferData, 0, frameDataBuffer, (int)frameData.frameOffset, bufferLength);

                    _frameQueue[frameId].bytesReceived += bufferLength;
                    _frameQueue[frameId].frameData = frameDataBuffer;
                }

                // Check if we have a full frame
                if (_frameQueue[frameId].bytesReceived == _frameQueue[frameId].frameSize)
                {
                    _frameQueue[frameId].fullFrame = true;
                    var renderedFrameId = _frameQueue[frameId].frameId;
                    var renderedFrameData = _frameQueue[frameId].frameData;

                    // Remove the frame from the queue
                    _frameQueue.Remove(frameId);
                    /*
                    postMessage(new
                    {
                        action = "doRender",
                        status = 200,
                        data = new { frameId = renderedFrameId, frameData = renderedFrameData },
                    });*/

                    return new RenderFrame(){ frameId = renderedFrameId, frameData = renderedFrameData };
                }
                else
                {
                    return new RenderFrame() { frameId = frameId };
                }
            });
        }

        public async Task OnMessage(WorkerMessage workerMessage)
        {
            switch (workerMessage.action)
            {
                case "endStream":
                    /*
                    self.destroy().then(() =>
                    {
                        postMessage(new
                        {
                            action = "endStream",
                            status = 200,
                        });
                    }).catch((error) =>
                    {
                        postMessage(new
                        {
                            action = "endStream",
                            status = 500,
                            message = error,
                        });
                    });
                    */
                    break;
                case "onPacket":
                    // Process incoming input
                    await OnPacket(workerMessage.data, workerMessage.timePerformanceNow).ContinueWith((task) =>
                                {
                                    if (task.IsFaulted)
                                    {
                                        Exception e = task.Exception;
                                        while (e.InnerException != null) e = e.InnerException;
                                        Console.WriteLine($"xCloudPlayer Worker/Video.cs - Failed onPacket(): {e.Message}");
                                    }
                                    else
                                    {
                                        RenderFrame result = task.Result;
                                        if (result != null)
                                        {
                                            // Packet succeeded
                                        }
                                    }
                                }, TaskScheduler.Default);                    
                    break;
                default:
                    Console.WriteLine("xCloudPlayer Worker/Video.cs - Unknown incoming worker message:", workerMessage.action, workerMessage.data);
                    break;
            }
        }

        

    }
    public class WorkerMessage
    {
        public string action { get; set; }
        public byte[] data { get; set; }
        public int timePerformanceNow { get; set; }
    }
    public class RenderFrame
    {
        public uint frameId { get; set; }
        public byte[] frameData { get; set; }
    }
    public class FrameData
    {
        public uint frameId { get; set; }
        public double timestamp { get; set; }
        public uint frameSize { get; set; }
        public uint frameOffset { get; set; }
        public uint serverDataKey { get; set; }
        public bool isKeyFrame { get; set; }
        public byte[] frameData { get; set; }
        public int bytesReceived { get; set; }
        public bool fullFrame { get; set; }
        public int firstFramePacketArrivalTimeMs { get; set; }
        public int frameSubmittedTimeMs { get; set; }
        public int frameDecodedTimeMs { get; set; }
        public int frameRenderedTimeMs { get; set; }

       
    }
}
