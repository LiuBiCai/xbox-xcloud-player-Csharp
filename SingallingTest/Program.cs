

//console.log('xCloudPlayer Client - /api/start - Session is ready!', data)

using SingallingTest;
Singalling singalling = new Singalling();
//xss 4100
string userToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjgxQjg5MDA3LTI1REQtNEMwMC1BNjcyLTUyQzI0QzQ1OUY5RCIsInR5cCI6IkpXVCJ9.eyJhcHBpZCI6IjEwMTY4OTg0MzkiLCJjb3VudHJ5IjoiMjAiLCJjb3VudHJ5Y29kZSI6IkNOIiwiZGV2aWNlaWQiOiJGNzAwRTRDMUM1OTI3NjYyIiwiZGV2aWNldHlwZSI6IkFuZHJvaWQiLCJ1c2VyaWQiOiJMYWRlbkdvbGYzMTI4IiwicHVpZCI6Ijg0NDQyNjU4MTQ3NzM0NyIsInh1aWQiOiIyNTM1NDI4MjY2MDkyMjEzIiwieHVzRmxpZ2h0cyI6IltcIjQ2MTE2ODYwMTg1NDczMzIwMDBcIixcImI0ZWMyNTRlLWNlYzAtNDlkZC04MzEzLWVmNWQwNWZiYWVmNVwiLFwiYzE2MzczMmEtZGViNi00MTQwLWI5YzUtN2NmZDNkMGFiODcxXCIsXCIxYzNmNzhhYi05YWJhLTRkNWMtODM3MC04NmE1ZTc4OWE5N2NcIl0iLCJwYXJ0bmVyaWQiOiJNSUNST1NPRlQiLCJvZmZlcmluZ2lkIjoiWEhPTUUiLCJpbnN0YW5jZWlkIjoiMWJhZjNhNDEtZjU0Zi00MTA4LWEyYWItNDI3NWQ2MzI2NTcxIiwidHlwZSI6IlVzZXIiLCJ2ZXJzaW9uIjoiMi4wIiwiZmxpZ2h0cyI6Int9IiwibmJmIjoxNjkwOTU3MTM2LCJleHAiOjE2OTA5NzE1MzYsImlhdCI6MTY5MDk1NzEzNiwiaXNzIjoiaHR0cHM6Ly94aG9tZS1hdXRoLXByb2QueGJveGxpdmUuY29tIiwiYXVkIjoiaHR0cHM6Ly94aG9tZS1wcm9kLnhib3hsaXZlLmNvbSJ9.Dq5c_Gx09QSjIRpi-2rEJtC3ohrCllMBK9iATF3ZQidwHHZ93wMaXaDACpadEg2mTb1w-x3jn_Y3pUE0uundIFX5yAMfVHnBAbKxOOA6tM5rnnI8lk4eGxX7AJaAXh-zHRKF6sphKsoXee4Wy8ewDsC3ZVcMFW0rDKWyt-GsO74";

singalling.userToken = userToken;
//await server.GetConsoles();
string serverId = "F4001E14C17E9AA5";
var startSession = await singalling.Start(serverId);
Console.WriteLine("xCloudPlayer Client - /api/start - ok, got:", startSession);

//server.tempSessionID = "F06CB311-246D-415B-86F1-0ED060A27A3F";
var result2 = await singalling.IsSessionsReady();
Console.WriteLine("xCloudPlayer Client - /api/start - Session is ready!", result2, "Waiting...");


// Fetch SDP Offer
// client.createOffer().then((offer) => {
singalling.CreatOffer();
// console.log('xCloudPlayer Client - Got offer data:', offer)

/*
 *   fetch('/api/config/sdp', {
                                    method: 'POST',
                                    headers: {
                                        'Content-Type': 'application/json'
                                    },
                                    body: JSON.stringify({
                                        sdp: offer.sdp
                                    })
                                })
 */
var configSdp = await singalling.PostConfigSdp();
Console.WriteLine(configSdp);
//this.isExchangeReady('/api/config').then((data) => {
var config = await singalling.GetConfig();
if(config==singalling.getConfigFailed)
{
    Console.WriteLine($"{config}");
}
//this.isExchangeReady('/api/config/sdp').then((data) => {
var sdp = await singalling.ConfigureSDP();
if(sdp==singalling.getConfigSDPFailed)
{
    Console.WriteLine($"{sdp}");
}
Console.WriteLine("sdp="+sdp);
//console.log('xCloudPlayer Client - SDP Server response:', data) 
/*{"exchangeResponse":"{\"audio\":1,\"chat\":1,\"chatConfiguration\":{\"format\":{\"codec\":\"opus\",\"container\":\"webm\"}},\"control\":2,\"input\":4,\"message\":1,\"messageType\":\"answer\",\"sdp\":\"v=0\\r\\no=- 9186087405738891 2 IN IP4 127.0.0.1\\r\\ns=-\\r\\nt=0 0\\r\\na=group:BUNDLE 0\\r\\na=msid-semantic: WMS\\r\\nm=application 9 UDP/DTLS/SCTP webrtc-datachannel\\r\\nc=IN IP4 0.0.0.0\\r\\na=ice-ufrag:2M2A3QcLSA\\r\\na=ice-pwd:3emnHC0X/zEq339SBdBAxeli\\r\\na=fingerprint:sha-256 7D:DC:E0:83:96:66:DE:01:2A:D4:26:8E:E1:11:F4:A3:47:B2:7B:74:00:DC:63:59:7E:38:9D:86:CF:D9:BA:46\\r\\na=setup:active\\r\\na=mid:0\\r\\na=sctp-port:18499\\r\\n\",\"sdpType\":\"answer\",\"status\":\"success\",\"supportedFecProtocols\":[\"raptorq\"],\"video\":2}","errorDetails":{"code":null,"message":null}}
 */


// Do ICE Handshake
//var sdpDetails = JSON.parse(data.exchangeResponse)



//client.setRemoteOffer(sdpDetails.sdp)









