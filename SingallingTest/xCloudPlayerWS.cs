using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Crmf;
using SingallingTest.Channel;
using SIPSorcery.Media;
using SIPSorcery.Net;
using SIPSorcery.SIP.App;
using SIPSorceryMedia.Abstractions;
using SIPSorceryMedia.Encoders;
using System.Runtime.Intrinsics.Arm;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp.Server;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Extensions.Logging;

namespace SingallingTest
{
    public class WebRtcClient : WebSocketBehavior
    {
        public RTCPeerConnection pc;

        public event Func<WebSocketContext, Task<RTCPeerConnection>> WebSocketOpened;
        public event Func<WebSocketContext, RTCPeerConnection, string, Task> OnMessageReceived;

        public WebRtcClient()
        { }

        protected override void OnMessage(MessageEventArgs e)
        {
            OnMessageReceived(this.Context, pc, e.Data);
        }

        protected override async void OnOpen()
        {
            base.OnOpen();
            pc = await WebSocketOpened(this.Context);
        }
    }
    public class xCloudPlayerWS
    {
        /*
 *  _webrtcConfiguration = {
        iceServers: [{
            urls: 'stun:stun.l.google.com:19302',
        }, {
            urls: 'stun:stun1.l.google.com:19302',
        }],
    }
 */

        private const string LOCALHOST_CERTIFICATE_PATH = "certs/localhost.pfx";
        static RTCConfiguration _webrtcConfiguration = new RTCConfiguration
        {
            iceServers = new List<RTCIceServer> { new RTCIceServer { urls = "stun:stun.l.google.com:19302" }, new RTCIceServer { urls = "stun:stun1.l.google.com:19302" } },
            certificates = new List<RTCCertificate> { new RTCCertificate{Certificate= new X509Certificate2(LOCALHOST_CERTIFICATE_PATH, "", X509KeyStorageFlags.Exportable)
            }
            }


        };
        RTCPeerConnection _webrtcClient;

        Dictionary<string, RTCDataChannelInit> _webrtcDataChannelsConfig = new Dictionary<string, RTCDataChannelInit>
{
    {"video", new RTCDataChannelInit(){id=1,ordered=true, protocol="1.0" } },
    {"audio", new RTCDataChannelInit(){ id=2,ordered=true,maxRetransmits= 0 ,protocol="audioV1"} },
    {"input", new RTCDataChannelInit(){id=3,ordered=true, protocol="1.0" } },
    {"control", new RTCDataChannelInit(){ id=4 , protocol="controlV1" } },
    {"message", new RTCDataChannelInit(){ id = 5 ,protocol = "messageV1" } },
    {"chat", new RTCDataChannelInit(){ id = 6 , protocol = "chatV1" } }
};

        Dictionary<string, string> _webrtcStates = new Dictionary<string, string> { { "iceGathering", "open" }, { "iceConnection", "open" }, { "iceCandidates", "[]" }, { "streamConnection", "open" }, };

        public string getConfigFailed = "Get config failed";
        public string getIceFailed = "Get ICE failed";
        public string getConfigSDPFailed = "Get Config SDP Failed";

        Stack<RTCIceCandidate> _iceCandidates = new Stack<RTCIceCandidate>();
        private static uint _rtpEventSsrc = 0;

       
        private const int WEBSOCKET_PORT = 8081;
        private const string FFPLAY_DEFAULT_SDP_PATH = "ffplay.sdp";
        private const string FFPLAY_DEFAULT_COMMAND = "ffplay -probesize 32 -protocol_whitelist \"file,rtp,udp\" -i {0}";
        private const int FFPLAY_DEFAULT_AUDIO_PORT = 5016;
        private const int FFPLAY_DEFAULT_VIDEO_PORT = 5018;

        private static Microsoft.Extensions.Logging.ILogger logger = NullLogger.Instance;

        private static WebSocketServer _webSocketServer;
        private static RTCPeerConnection _activePeerConnection;
        public xCloudPlayerWS()
        {
            _webrtcClient = new RTCPeerConnection(_webrtcConfiguration);


            CancellationTokenSource exitCts = new CancellationTokenSource();

            
            // Start web socket.
            Console.WriteLine("Starting web socket server...");
            _webSocketServer = new WebSocketServer(IPAddress.Any, WEBSOCKET_PORT);
            //_webSocketServer = new WebSocketServer(IPAddress.Any, WEBSOCKET_PORT, true);
            _webSocketServer.SslConfiguration.ServerCertificate = new X509Certificate2(LOCALHOST_CERTIFICATE_PATH);
            _webSocketServer.SslConfiguration.CheckCertificateRevocation = false;
            _webSocketServer.Log.Level = WebSocketSharp.LogLevel.Debug;
            _webSocketServer.AddWebSocketService<WebRtcClient>("/", (client) =>
            {
                //client.WebSocketOpened += SendOffer;
                client.OnMessageReceived += WebSocketMessageReceived;
            });
            _webSocketServer.Start();

        }
        private static async Task WebSocketMessageReceived(WebSocketContext context, RTCPeerConnection pc, string message)
        {
            Console.WriteLine($"WebSocket message received {message}.");
        }
       
        /*
         *  _openDataChannels(){
        for(const channel in this._webrtcDataChannelsConfig){
            this._openDataChannel(channel, this._webrtcDataChannelsConfig[channel])
        }
    }
         */
        public async Task OpenDataChannels()
        {
            _webrtcDataChannels = new Dictionary<string, RTCDataChannel>();
            _webrtcChannelProcessors = new Dictionary<string, BaseChannel>();
            foreach (var channel in _webrtcDataChannelsConfig)
            {
                await OpenDataChannel(channel.Key, channel.Value);
            }
        }
        Dictionary<string, RTCDataChannel> _webrtcDataChannels = new Dictionary<string, RTCDataChannel>();
        Dictionary<string, BaseChannel> _webrtcChannelProcessors = new Dictionary<string, BaseChannel>();
        private async Task OpenDataChannel(string name, RTCDataChannelInit config)
        {
            //console.log('xCloudPlayer Library.ts - Creating data channel:', name, config)
            var createDataChannelResult = await _webrtcClient.createDataChannel(name, config);
            _webrtcDataChannels.Add(name, createDataChannelResult);

            switch (name)
            {
                case "video":
                    _webrtcChannelProcessors.Add(name, new VideoChannel("video"));
                    break;
                case "audio":
                    _webrtcChannelProcessors.Add(name, new AudioChannel("audio"));
                    break;
                case "input":
                    _webrtcChannelProcessors.Add(name, new InputChannel("input"));
                    break;
                case "control":
                    _webrtcChannelProcessors.Add(name, new ControlChannel("control"));
                    break;
                case "chat":
                    _webrtcChannelProcessors.Add(name, new DebugChannel("chat"));
                    break;
                case "message":
                    _webrtcChannelProcessors.Add(name, new MessageChannel("message"));
                    break;
            }
            _webrtcDataChannels[name].onopen += () =>
            {
                Console.WriteLine($"xCloudPlayer Library.ts - Data channel {name} opened");
                _webrtcChannelProcessors[name].OnOpen(_webrtcDataChannels[name]);
            };
            _webrtcClient.ondatachannel += (channel) =>
            {
                Console.WriteLine($"xCloudPlayer Library.ts - Data channel {name} opened");
                _webrtcChannelProcessors[name].OnOpen(_webrtcDataChannels[name]);
            };

            _webrtcDataChannels[name].onclose += () =>
            {
                Console.WriteLine($"xCloudPlayer Library.ts - Data channel {name} closed");
                _webrtcChannelProcessors[name].OnClose(_webrtcDataChannels[name]);
            };
            _webrtcDataChannels[name].onerror += (err) =>
            {
                Console.WriteLine($"xCloudPlayer Library.ts - Data channel {name} error: {err}");
                _webrtcChannelProcessors[name].OnError(_webrtcDataChannels[name], err);
            };
            _webrtcDataChannels[name].onmessage += (msg, protocol, data) =>
            {
                Console.WriteLine($"xCloudPlayer Library.ts - Data channel {name} message: {msg}");
                _webrtcChannelProcessors[name].OnMessage(_webrtcDataChannels[name], msg, protocol, data);
            };




        }



        RTCSessionDescriptionInit offer;
        //client.setRemoteOffer(sdpDetails.sdp)
        public async Task CreatOffer()
        {
            await OpenDataChannels();

            var videoSource = new VideoTestPatternSource(new VpxVideoEncoder());
            var videoSink = new VideoEncoderEndPoint();
            MediaStreamTrack videoTrack = new MediaStreamTrack(videoSink.GetVideoSourceFormats(), MediaStreamStatusEnum.RecvOnly);
            //_webrtcClient.addTrack(videoTrack);
            //_webrtcClient.OnVideoFrameReceived += videoSink.GotVideoFrame;
            /*
             _webrtcClient.OnVideoFormatsNegotiated += (formats) =>
             {
                 Console.WriteLine($"OnVideoFormatsNegotiated");
                 videoSink.SetVideoSourceFormat(formats.First());
                 videoSource.SetVideoSourceFormat(formats.First());
             };
            */
            _webrtcClient.OnTimeout += (mediaType) => Console.WriteLine($"Peer connection timeout on media {mediaType}.");
            _webrtcClient.oniceconnectionstatechange += (state) => Console.WriteLine($"ICE connection state changed to {state}.");

            _webrtcClient.onconnectionstatechange += async (state) =>
            {
                Console.WriteLine($"Peer connection connected changed to {state}.");

                if (state == RTCPeerConnectionState.closed || state == RTCPeerConnectionState.failed)
                {
                    await videoSource.CloseVideo().ConfigureAwait(false);
                    videoSource.Dispose();
                }
            };

            videoSink.OnVideoSinkDecodedSample += (byte[] bmp, uint width, uint height, int stride, VideoPixelFormatsEnum pixelFormat) =>
            {
                Console.WriteLine($"OnVideoSinkDecodedSample.");
                unsafe
                {
                    fixed (byte* s = bmp)
                    {
                        Bitmap bmpImage = new Bitmap((int)width, (int)height, (int)(bmp.Length / height), PixelFormat.Format24bppRgb, (IntPtr)s);
                        bmpImage.Save(DateTime.UtcNow.Ticks.ToString() + ".bmp", ImageFormat.Bmp);
                        //remoteVideoPicBox.Image = bmpImage;
                    }
                }
            };

            RTCOfferOptions rTCOfferOptions = new RTCOfferOptions();
            rTCOfferOptions.X_ExcludeIceCandidates = true;
            offer = _webrtcClient.createOffer(rTCOfferOptions);
            offer.sdp += "a=extmap-allow-mixed\r\n";
            offer.sdp += "a=msid-semantic: WMS\r\n";
            offer.sdp = offer.sdp.Replace("ice2,trickle", "trickle");
            SDP sDP = SDP.ParseSDPDescription(offer.sdp);

            sDP.SessionName = "-";
            sDP.AnnouncementVersion = 2;


            offer.sdp = sDP.ToString();


            Console.WriteLine($"offer.sdp: {offer.sdp}");
            await _webrtcClient.setLocalDescription(offer).ConfigureAwait(false);
            //await videoSource.StartVideo().ConfigureAwait(false);
            _webrtcClient.onicecandidate += async (cand) =>
            {
                Console.WriteLine(cand.ToString());
                if (cand.candidate != null)
                {
                    _iceCandidates.Push(cand);
                }

                Console.WriteLine("cand");
                // Handle ICE Candidate messages
                //
            };
            _webrtcClient.onicecandidateerror += (candidate, error) => Console.WriteLine($"Error adding remote ICE candidate. {error} {candidate}");
            _webrtcClient.OnRtcpBye += (reason) => Console.WriteLine($"RTCP BYE receive, reason: {(string.IsNullOrWhiteSpace(reason) ? "<none>" : reason)}.");
            // Peer ICE connection state changes are for ICE events such as the STUN checks completing.
            _webrtcClient.oniceconnectionstatechange += (state) => Console.WriteLine($"ICE connection state change to {state}.");

            _webrtcClient.ondatachannel += (dc) =>
            {
                Console.WriteLine($"Data channel opened by remote peer, label {dc.label}, stream ID {dc.id}.");
                dc.onmessage += (dc, protocol, data) =>
                {
                    if (protocol == DataChannelPayloadProtocols.WebRTC_String ||
                        protocol == DataChannelPayloadProtocols.WebRTC_String_Partial)
                    {
                        Console.WriteLine($"data channel ({dc.label}:{dc.id}): {Encoding.UTF8.GetString(data)}.");
                        dc.send($"echo: {Encoding.UTF8.GetString(data)}");
                    }
                    else
                    {
                        Console.WriteLine($"data channel ({dc.label}:{dc.id}): received {dc.protocol} message, length {data?.Length} bytes.");
                    }
                };
            };
            _webrtcClient.onsignalingstatechange += () =>
            {
                if (_webrtcClient.signalingState == RTCSignalingState.have_remote_offer
                    || _webrtcClient.signalingState == RTCSignalingState.stable)
                {
                    Console.WriteLine("Remote SDP:");
                    Console.WriteLine(_webrtcClient.remoteDescription.sdp.ToString());
                }
                else if (_webrtcClient.signalingState == RTCSignalingState.have_local_offer)
                {
                    Console.WriteLine("Local SDP:");
                    Console.WriteLine(_webrtcClient.localDescription.sdp.ToString());
                }
            };


            _webrtcClient.OnRtpEvent += (ep, ev, hdr) =>
            {
                if (_rtpEventSsrc == 0)
                {
                    if (ev.EndOfEvent && hdr.MarkerBit == 1)
                    {
                        Console.WriteLine($"RTP event echo received: {ev.EventID}.");
                    }
                    else if (!ev.EndOfEvent)
                    {
                        _rtpEventSsrc = hdr.SyncSource;
                        Console.WriteLine($"RTP event echo received: {ev.EventID}.");
                    }
                }

                if (_rtpEventSsrc != 0 && ev.EndOfEvent)
                {
                    _rtpEventSsrc = 0;
                }
            };
        }

        public string userToken { get; set; } = "";
        public string tempSessionID { get; set; } = "";

        public async Task<bool> PostConfigSdp()
        {
            System.Console.WriteLine($"API - POST - config-sdp sessionID: {tempSessionID}");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, $"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/sdp");
            postRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            postRequest.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                messageType = "offer",
                sdp = offer.sdp,
                configuration = new
                {
                    containerizeAudio = false,
                    chatConfiguration = new
                    {
                        bytesPerSample = 2,
                        expectedClipDurationMs = 100,
                        format = new
                        {
                            codec = "opus",
                            container = "webm"
                        },
                        numChannels = 1,
                        sampleFrequencyHz = 24000
                    },
                    audio = new
                    {
                        minVersion = 1,
                        maxVersion = 1
                    },
                    chat = new
                    {
                        minVersion = 1,
                        maxVersion = 1
                    },
                    control = new
                    {
                        minVersion = 1,
                        maxVersion = 2
                    },
                    input = new
                    {
                        minVersion = 1,
                        maxVersion = 4
                    },
                    message = new
                    {
                        minVersion = 1,
                        maxVersion = 1
                    },
                    video = new
                    {
                        minVersion = 1,
                        maxVersion = 2
                    }
                }
            }), Encoding.UTF8, "application/json");
            //Console.WriteLine(offer.sdp);



            await _webrtcClient.Start();
            using var client = new HttpClient();
            var response = await client.SendAsync(postRequest);
            //Console.WriteLine(response.Content.ToString());
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<object> IsExchangeReady(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"xCloudPlayer Client - {url} - Waiting...");
                    await Task.Delay(1000);
                    return await IsExchangeReady(url);
                }
                else
                {
                    var data = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"xCloudPlayer Client - {url} - Ready! Got data: {data}");
                    return JsonConvert.DeserializeObject(data);
                }
            }
        }

        public async Task<string> GetConfig()
        {
            System.Console.WriteLine($"API - config sessionID: {tempSessionID}");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userToken}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                int waitCount = 0;
                do
                {



                    var response = await client.GetAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/configuration");

                    System.Console.WriteLine($"API - config statuscode: {response.StatusCode}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        return responseData;
                    }


                    await Task.Delay(1000);
                }
                while (waitCount++ < 100);
            }
            return getConfigFailed;
        }


        public async Task<string> ConfigureSDP()
        {
            Console.WriteLine("API - config-sdp sessionID:" + tempSessionID);

            using (var client = new HttpClient())
            {
                int waitCount = 0;
                client.BaseAddress = new Uri("https://uks.gssv-play-prodxhome.xboxlive.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                do
                {


                    HttpResponseMessage response = await client.GetAsync("/v4/sessions/home/" + tempSessionID + "/sdp");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();


                        Console.WriteLine(responseBody);
                        return responseBody;


                    }
                    await Task.Delay(1000);
                }
                while (waitCount++ < 100);
                return getConfigSDPFailed;

            }
        }

        public async Task<string> Start(string serverId)
        {
            HttpClient _httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/play");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            var requestBody = new
            {
                titleId = "",
                systemUpdateGroup = "",
                settings = new
                {
                    nanoVersion = "V3;RtcdcTransport.dll",
                    enableTextToSpeech = false,
                    highContrast = 0,
                    locale = "en-US",
                    useIceConnection = false,
                    timezoneOffsetMinutes = 120,
                    sdkType = "web",
                    osName = "windows",
                },
                serverId = serverId,
                fallbackRegionNames = new string[] { },
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            Console.WriteLine("API - start statuscode: " + (int)response.StatusCode);
            var responseData = JsonConvert.DeserializeObject<SessionStartResponse>(await response.Content.ReadAsStringAsync());
            tempSessionID = responseData.SessionId;
            Console.WriteLine("API - start set sessionID: " + tempSessionID);
            return tempSessionID;
        }

        public async Task<string> IsSessionsReady()
        {
            Console.WriteLine("API - session sessionID: " + tempSessionID);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/state");
            var data = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API - session statuscode: " + response.StatusCode);
            return data;
        }

        public async Task<bool> SetRemoteOffer(string data)
        {
            try
            {
                //反序列化data
                //"{\"exchangeResponse\":\"{\\\"audio\\\":1,\\\"chat\\\":1,\\\"chatConfiguration\\\":{\\\"format\\\":{\\\"codec\\\":\\\"opus\\\",\\\"container\\\":\\\"webm\\\"}},\\\"control\\\":2,\\\"input\\\":4,\\\"message\\\":1,\\\"messageType\\\":\\\"answer\\\",\\\"sdp\\\":\\\"v=0\\\\r\\\\no=- 4649125511298530 2 IN IP4 127.0.0.1\\\\r\\\\ns=-\\\\r\\\\nt=0 0\\\\r\\\\na=group:BUNDLE 0\\\\r\\\\na=msid-semantic: WMS\\\\r\\\\nm=application 9 UDP/DTLS/SCTP webrtc-datachannel\\\\r\\\\nc=IN IP4 0.0.0.0\\\\r\\\\na=ice-ufrag:HiBsFfw4Nf\\\\r\\\\na=ice-pwd:ctoAxwOKKy29bp9WCWWmquTk\\\\r\\\\na=fingerprint:sha-256 78:D6:C4:80:14:2F:F0:A3:11:38:21:98:F4:38:0A:37:2F:4A:51:C1:0A:37:CF:72:B5:E5:B2:BE:47:E0:B2:14\\\\r\\\\na=setup:active\\\\r\\\\na=mid:0\\\\r\\\\na=sctp-port:20284\\\\r\\\\n\\\",\\\"sdpType\\\":\\\"answer\\\",\\\"status\\\":\\\"success\\\",\\\"supportedFecProtocols\\\":[\\\"raptorq\\\"],\\\"video\\\":2}\",\"errorDetails\":{\"code\":null,\"message\":null}}"
                string sdpPattern = "\\\\\"sdp\\\\\":\\\\\"(.*?)\\\\\"";
                Match sdpMatch = Regex.Match(data, sdpPattern);
                string sdp = sdpMatch.Groups[1].Value;

                sdp = sdp.Replace("\\\\", "\\");
                sdp = Regex.Replace(sdp, @"\\r\\n", "\r\n");
                sdp = Regex.Replace(sdp, @"\\n", "\n");
                sdp = Regex.Replace(sdp, @"\\r", "\r");

                SDP des = SDP.ParseSDPDescription(sdp);
                _webrtcClient.SetRemoteDescription(SdpType.answer, des);
                

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetRemoteOffer");
                return false;

            }


        }

        public async Task<bool> PostIce()
        {
            var iceCandidate = new IceCandidate
            {
                candidate = _iceCandidates.Peek().candidate,
                sdpMid = "0",
                sdpMLineIndex = 0
            };
            var payload = new IcePayload { ice = iceCandidate };
            var ice = JsonConvert.SerializeObject(payload.ice);


            Console.WriteLine($"API - POST - config-ice sessionID: {tempSessionID}");
            Console.WriteLine(ice);

            string postData = "{\"messageType\": \"iceCandidate\",\"candidate\":" + ice + "}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/ice", httpContent);

            Console.WriteLine($"API - start statuscode: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetIce()
        {
            Console.WriteLine($"API - config-ice sessionID: {tempSessionID}");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                int waitCount = 0;
                do
                {

                    var response = await client.GetAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/ice");
                    Console.WriteLine($"API - config-ice statuscode: {(int)response.StatusCode}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        return data;
                    }

                    await Task.Delay(1000);
                }
                while (waitCount++ < 100);
                return getIceFailed;

            }

        }

        public async Task<string> SetIceCandidates(string data)
        {
            string[] array = data.Split("\\\"", StringSplitOptions.RemoveEmptyEntries);
            foreach (string text in array)
            {
                if (text.Contains("end-of-candidates"))
                    break;
                if (text.StartsWith("a="))
                {
                    _webrtcClient.addIceCandidate(new RTCIceCandidateInit()
                    {
                        candidate = text,
                        sdpMid = "0",
                        sdpMLineIndex = 0
                    });
                }

            }


            Console.WriteLine(data);
            // iceCandidates = JsonConvert.DeserializeObject<IcePayloads>(data);


            /*
            foreach (var iceCandidate in iceCandidates)
            {
                //pc.addIceCandidate({ candidate: evt.data, sdpMid: "0", sdpMLineIndex: 0 });
                if (iceCandidate.candidate.Contains("end-of-candidates"))
                    break;
                _webrtcClient.addIceCandidate(new RTCIceCandidateInit()
                {
                    candidate=iceCandidate.candidate,
                    sdpMid=iceCandidate.sdpMid.ToString(),
                    sdpMLineIndex=(ushort)iceCandidate.sdpMLineIndex
                });
            }
            */

            return "success";
        }

        public RTCDataChannel GetChannel(string name)
        {
            return _webrtcDataChannels[name];
        }
    }
   


}

