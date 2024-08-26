

//console.log('xCloudPlayer Client - /api/start - Session is ready!', data)
 
using SingallingTest;

xCloudPlayer singalling = new xCloudPlayer();
//xss 4100 abagail231
string userToken = " eyJhbGciOiJSUzI1NiIsImtpZCI6IjEwQkI4OTgzLTI4NDktNEYyQi05MDczLTI2Rjg0NzVBMjM3QiIsInR5cCI6IkpXVCJ9.eyJhcHBpZCI6IjEwMTY4OTg0MzkiLCJjb3VudHJ5IjoiMTAzIiwiY291bnRyeWNvZGUiOiJVUyIsImRldmljZWlkIjoiRjcwMEUyMTYwN0VGNENCNCIsImRldmljZXR5cGUiOiJBbmRyb2lkIiwidXNlcmlkIjoiUm9ja01hcm1vdDM1MDQwIiwicHVpZCI6IjEwNTU1MTk5OTI2ODc2MjgiLCJ4dWlkIjoiMjUzNTQzNTQwNzUxNjY0MCIsInh1c0ZsaWdodHMiOiJbXCI0NjExNjg2MDE4NTQ3MzMyMDAwXCIsXCJiNGVjMjU0ZS1jZWMwLTQ5ZGQtODMxMy1lZjVkMDVmYmFlZjVcIixcImMxNjM3MzJhLWRlYjYtNDE0MC1iOWM1LTdjZmQzZDBhYjg3MVwiLFwiMWMzZjc4YWItOWFiYS00ZDVjLTgzNzAtODZhNWU3ODlhOTdjXCJdIiwicGFydG5lcmlkIjoiTUlDUk9TT0ZUIiwib2ZmZXJpbmdpZCI6IlhIT01FIiwiaW5zdGFuY2VpZCI6IjNkZmI2ZTgyLTliM2QtNDYwYi04MmQxLWRiNzBhZjg0MmQ0ZCIsInR5cGUiOiJVc2VyIiwidmVyc2lvbiI6IjIuMCIsImZsaWdodHMiOiJ7fSIsIm5iZiI6MTY5NDQyMjcwNywiZXhwIjoxNjk0NDM3MTA3LCJpYXQiOjE2OTQ0MjI3MDcsImlzcyI6Imh0dHBzOi8veGhvbWUtYXV0aC1wcm9kLnhib3hsaXZlLmNvbSIsImF1ZCI6Imh0dHBzOi8veGhvbWUtcHJvZC54Ym94bGl2ZS5jb20ifQ.JQUi9fHnnkUJCoBUfqDd-8V5NQoU1DT91zwjtxnazmo6NY6756UdNBGzFQAVxdSZKdZBCLKjSztndR5BvRdx4Jy8O04QmsgGCT3CvIDrV0sZRQrAPmaEc6_Iyw1t5jtMnWm7p4As6LFn_DD_n9Dr2dA-7qYHRv0r9W-Ry99qlkA";

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
await singalling.CreatOffer();
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
await singalling.SetRemoteOffer(sdp);

// Send ice config
await singalling.PostIce();

// ICE Has been set, lets do ICE
var ice = await singalling.GetIce();
/*
 * {"candidates":"[{\"candidate\":\"a=candidate:1 1 UDP 100 192.168.3.55 9002 typ host \",\"messageType\":\"iceCandidate\",\"sdpMLineIndex\":\"0\",\"sdpMid\":\"0\"},{\"candidate\":\"a=candidate:2 1 UDP 1 2001:0:2851:7ae4:3cca:d733:c20e:356a 9002 typ host \",\"messageType\":\"iceCandidate\",\"sdpMLineIndex\":\"0\",\"sdpMid\":\"0\"},{\"candidate\":\"a=end-of-candidates\",\"messageType\":\"iceCandidate\",\"sdpMLineIndex\":\"0\",\"sdpMid\":\"0\"}]"}
 * 
 */
await singalling.SetIceCandidates(ice);


// Ctrl-c will gracefully exit the call at any point.
ManualResetEvent exitMre = new ManualResetEvent(false);
Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
{
    e.Cancel = true;
    exitMre.Set();
};

// Wait for a signal saying the call failed, was cancelled with ctrl-c or completed.
exitMre.WaitOne();



