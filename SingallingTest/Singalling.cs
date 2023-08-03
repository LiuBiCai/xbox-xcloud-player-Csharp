using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Crmf;
using SIPSorcery.Net;
using SIPSorcery.SIP.App;

namespace SingallingTest
{
    public class Singalling
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

        static RTCConfiguration _webrtcConfiguration = new RTCConfiguration
        {
            iceServers = new List<RTCIceServer> { new RTCIceServer { urls = "stun:stun.l.google.com:19302" }, new RTCIceServer { urls = "stun:stun1.l.google.com:19302" } }
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
        public Singalling()
        {
            _webrtcClient = new RTCPeerConnection(_webrtcConfiguration);
            OpenDataChannels();
            _webrtcClient.onicecandidate += async (cand) =>
            {
                Console.WriteLine("cand");
                // Handle ICE Candidate messages
                //
            };
        }
        /*
         *  _openDataChannels(){
        for(const channel in this._webrtcDataChannelsConfig){
            this._openDataChannel(channel, this._webrtcDataChannelsConfig[channel])
        }
    }
         */
        public void OpenDataChannels()
        { 
            foreach(var channel in _webrtcDataChannelsConfig)
            {
                OpenDataChannel(channel.Key, channel.Value);
            }
        }
        Dictionary<string, RTCDataChannel> _webrtcDataChannels = new Dictionary<string, RTCDataChannel>();
        private void OpenDataChannel(string name, RTCDataChannelInit config)
        {
            //console.log('xCloudPlayer Library.ts - Creating data channel:', name, config)
            var createDataChannelResult = _webrtcClient.createDataChannel(name, config).Result;
            _webrtcDataChannels.Add(name, createDataChannelResult);
            /*
            switch (name)
            {
                case "video":
                    _webrtcChannelProcessors[name] = new VideoChannel("video", this);
                    break;
                case "audio":
                    _webrtcChannelProcessors[name] = new AudioChannel("audio", this);
                    break;
                case "input":
                    _webrtcChannelProcessors[name] = new InputChannel("input", this);
                    break;
                case "control":
                    _webrtcChannelProcessors[name] = new ControlChannel("control", this);
                    break;
                case "chat":
                    _webrtcChannelProcessors[name] = new DebugChannel("chat", this);
                    break;
                case "message":
                    _webrtcChannelProcessors[name] = new MessageChannel("message", this);
                    break;
            }
            */
        }



        RTCSessionDescriptionInit offer;
        //client.setRemoteOffer(sdpDetails.sdp)
        public void CreatOffer()
        {
            RTCOfferOptions rTCOfferOptions = new RTCOfferOptions();
            rTCOfferOptions.X_ExcludeIceCandidates = true;
            offer =_webrtcClient.createOffer(rTCOfferOptions);
            
            _webrtcClient.setLocalDescription(offer);
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
            Console.WriteLine(offer.sdp);
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
                while (waitCount++ <100);
            }
            return getConfigFailed;
        }

        public async Task<string> GetIce(object ice)
        {
            Console.WriteLine($"API - config-ice sessionID: {tempSessionID}");
            using (var client = new HttpClient())
            {
                int waitCount = 0;
                do
                {

                    var response = await client.GetAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/ice");
                    Console.WriteLine($"API - config-ice statuscode: {(int)response.StatusCode}");
                    if (response.StatusCode== HttpStatusCode.OK)
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
        public async Task<string> ConfigureSDP()
        {
            Console.WriteLine("API - config-sdp sessionID:" + tempSessionID);
            
            using (var client = new HttpClient())
            {
                int waitCount = 0;
                do
                {
                    client.BaseAddress = new Uri("https://uks.gssv-play-prodxhome.xboxlive.com");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("/v4/sessions/home/" + tempSessionID + "/sdp");
                    if(response.StatusCode== HttpStatusCode.OK)
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

        public async void SetRemoteOffer(string data)
        {



            SDP sdp =SDP.ParseSDPDescription(data);

            _webrtcClient.SetRemoteDescription(SdpType.answer, sdp);
        }
    }
    public class SessionStartResponse
    {
        public string SessionId { get; set; } = "";
        // Define additional properties for the session start response here
    }
    public class ExchangeResponse
    {
        string exchangeResponse { get; set; } = "";
    }
}
