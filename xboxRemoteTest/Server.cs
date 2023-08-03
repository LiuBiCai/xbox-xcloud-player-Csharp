using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace xboxRemoteTest
{
    public class Server
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public string userToken { get; set; } = "";
        public string tempSessionID { get; set; } = "";

        public async Task<Tuple<bool, string>> GetConsoles()
        {
            Console.WriteLine("API - consoles request");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _httpClient.GetAsync("https://uks.gssv-play-prodxhome.xboxlive.com/v6/servers/home");
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {

                Console.WriteLine(responseBody);
                return new Tuple<bool, string>(true, responseBody);
            }
            else
            {
                Console.WriteLine("Error fetching consoles. Status: {0} Body: {1}", response.StatusCode, responseBody);
                return new Tuple<bool, string>(false, responseBody);
            }
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

        public async Task<SessionStartResponse> Start(string serverId)
        {
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
            return responseData;
        }

        private class SessionState
        {
            // Define properties for the session state response here
        }

        public class SessionStartResponse
        {
            public string SessionId { get; set; } = "";
            // Define additional properties for the session start response here
        }

        public async Task<string> GetConfig()
        {
            System.Console.WriteLine($"API - config sessionID: {tempSessionID}");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userToken}");
                client.DefaultRequestHeaders.Add("Content-Type", "application/json; charset=utf-8");

                var response = await client.GetAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/configuration");

                System.Console.WriteLine($"API - config statuscode: {response.StatusCode}");

                var responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
        }

        public async Task<bool> PostConfigSdp(SdpRequest request)
        {
            System.Console.WriteLine($"API - POST - config-sdp sessionID: {tempSessionID}");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, $"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/sdp");
            postRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            postRequest.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                messageType = "offer",
                sdp = request.Sdp,
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

            using var client = new HttpClient();
            var response = await client.SendAsync(postRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public class SdpRequest
        {
            public string Sdp { get; set; } = "";
        }
        public async Task<bool> PostIce(string ice)
        {
            Console.WriteLine($"API - POST - config-ice sessionID: {tempSessionID}");
            Console.WriteLine(ice);

            string postData = System.Text.Json.JsonSerializer.Serialize(new
            {
                messageType = "iceCandidate",
                candidate = ice
            });

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/ice", httpContent);

            Console.WriteLine($"API - start statuscode: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }


        public async Task<Tuple<bool,string>> ConfigureSDP()
        {
            Console.WriteLine("API - config-sdp sessionID:" + tempSessionID);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://uks.gssv-play-prodxhome.xboxlive.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/v4/sessions/home/" + tempSessionID + "/sdp");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {

                    Console.WriteLine(responseBody);
                    return new Tuple<bool, string>(true, responseBody);
                }
                else
                {
                    Console.WriteLine("Error fetching consoles. Status: {0} Body: {1}", response.StatusCode, responseBody);
                    return new Tuple<bool, string>(false, responseBody);
                }
            }
        }
        public async Task<bool> GetIce(object ice)
        {
            Console.WriteLine($"API - config-ice sessionID: {tempSessionID}");

            var response = await _httpClient.GetAsync($"https://uks.gssv-play-prodxhome.xboxlive.com/v4/sessions/home/{tempSessionID}/ice");

            var data = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"API - config-ice statuscode: {(int)response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
       


    }
}

