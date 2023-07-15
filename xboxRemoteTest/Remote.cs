using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace xboxRemoteTest
{
    public class Remote
    {
        public HttpResponseMessage Consoles()
        {
            string userToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkFGNEZBQUY2LUU0NjMtNDk5OC05MjM1LUJCRDFCNTlEOUM0RCIsInR5cCI6IkpXVCJ9.eyJhcHBpZCI6IjEwMTY4OTg0MzkiLCJjb3VudHJ5IjoiMjAiLCJjb3VudHJ5Y29kZSI6IkNOIiwiZGV2aWNlaWQiOiJGNzAwQzI4QThCMTBGMkFGIiwiZGV2aWNldHlwZSI6IkFuZHJvaWQiLCJ1c2VyaWQiOiJCaXphdDE5ODgiLCJwdWlkIjoiODQ0NDI1MDAzNTg4ODkyIiwieHVpZCI6IjI1MzU0NTA0NjU0OTIwMzMiLCJ4dXNGbGlnaHRzIjoiW1wiNDYxMTY4NjAxODU0NzMzMjAwMFwiLFwiYjRlYzI1NGUtY2VjMC00OWRkLTgzMTMtZWY1ZDA1ZmJhZWY1XCIsXCJjMTYzNzMyYS1kZWI2LTQxNDAtYjljNS03Y2ZkM2QwYWI4NzFcIixcIjFjM2Y3OGFiLTlhYmEtNGQ1Yy04MzcwLTg2YTVlNzg5YTk3Y1wiXSIsInBhcnRuZXJpZCI6Ik1JQ1JPU09GVCIsIm9mZmVyaW5naWQiOiJYSE9NRSIsImluc3RhbmNlaWQiOiJjOTg3MDEzOS0yZGJkLTQ3MzYtYjZiYy02ZTA2YzE1OWJhYzAiLCJ0eXBlIjoiVXNlciIsInZlcnNpb24iOiIyLjAiLCJmbGlnaHRzIjoie30iLCJuYmYiOjE2ODkzOTk3MzEsImV4cCI6MTY4OTQxNDEzMSwiaWF0IjoxNjg5Mzk5NzMxLCJpc3MiOiJodHRwczovL3hob21lLWF1dGgtcHJvZC54Ym94bGl2ZS5jb20iLCJhdWQiOiJodHRwczovL3hob21lLXByb2QueGJveGxpdmUuY29tIn0.RjiYNfTryRPWsc-qjD5eUByzw9QCkzfYRzE45AwRaKvx8L34Q6FPMv8lJFcQgkcM3pB_dXnTWLaCCbGx26pynNf9kjBVZ_WtR0up_kwQvBQaZp_S4MFPyWCa-viXfZx3NV7AfQYPOVmq8dRH0Xr4g0iPRvX_7ZZ3IiixI68Yjts";
            Console.WriteLine("API - consoles request");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://uks.gssv-play-prodxhome.xboxlive.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync("/v6/servers/home").Result;
                var data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonConvert.DeserializeObject(data);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(responseData.ToString(), System.Text.Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    Console.WriteLine("Error fetching consoles. Status: {0} Body: {1}", response.StatusCode, data);
                    return new HttpResponseMessage(response.StatusCode)
                    {
                        Content = new StringContent(data, System.Text.Encoding.UTF8, "application/json")
                    };
                }
            }
        }
    }
}
