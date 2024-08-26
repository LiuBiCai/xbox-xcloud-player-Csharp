using System;
using xboxRemoteTest;

//Remote remote=new Remote();
//var  result=remote.Consoles();
//Console.WriteLine(result.Content);
string userToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjRERTUwODIwLUMyQkItNEY1My05QTQ1LUUwNUI1NENFNENCRiIsInR5cCI6IkpXVCJ9.eyJhcHBpZCI6IjEwMTY4OTg0MzkiLCJjb3VudHJ5IjoiMTAzIiwiY291bnRyeWNvZGUiOiJVUyIsImRldmljZWlkIjoiRjcwMEU2MjY5NUVCRDgyQiIsImRldmljZXR5cGUiOiJBbmRyb2lkIiwidXNlcmlkIjoiQWdhdGVDaGFpbjM0ODc1IiwicHVpZCI6IjEwNTU1MjAwMDQ2MDYyOTkiLCJ4dWlkIjoiMjUzNTQ2Njg1MjY3NzAzMCIsInh1c0ZsaWdodHMiOiJbXCI0NjExNjg2MDE4NTQ3MzMyMDAwXCIsXCJiNGVjMjU0ZS1jZWMwLTQ5ZGQtODMxMy1lZjVkMDVmYmFlZjVcIixcImMxNjM3MzJhLWRlYjYtNDE0MC1iOWM1LTdjZmQzZDBhYjg3MVwiLFwiMWMzZjc4YWItOWFiYS00ZDVjLTgzNzAtODZhNWU3ODlhOTdjXCJdIiwicGFydG5lcmlkIjoiTUlDUk9TT0ZUIiwib2ZmZXJpbmdpZCI6IlhIT01FIiwiaW5zdGFuY2VpZCI6Ijc2NTQ2YTQ3LWZlZDUtNDMzOS1iMDQwLTBlY2I0NGU5MzFjYiIsInR5cGUiOiJVc2VyIiwidmVyc2lvbiI6IjIuMCIsImZsaWdodHMiOiJ7fSIsIm5iZiI6MTY5NDQ5ODAzMywiZXhwIjoxNjk0NTEyNDMzLCJpYXQiOjE2OTQ0OTgwMzMsImlzcyI6Imh0dHBzOi8veGhvbWUtYXV0aC1wcm9kLnhib3hsaXZlLmNvbSIsImF1ZCI6Imh0dHBzOi8veGhvbWUtcHJvZC54Ym94bGl2ZS5jb20ifQ.znhj2-K3-TNgTa83fuGoPQte-GYYzktTjLzMXVfE1Xr5EkuTVvYXojMYXNM_2hqbI20EeLBzbI8-Ikyio4L5OTonhtI3Lj0U9knkjH945RH1BynOc5ZStx55uRaOsnG8Wp7vLIaK4mY0OiAnY12785EVTEtSq_H5KFKIpx1nAlU";
Server server=new Server();
server.userToken = userToken;
await server.GetConsoles();
string serverId = "F4001CC3A528E632";
var startSession=await server.Start(serverId);
Console.WriteLine(startSession.SessionId);
Console.WriteLine("xCloudPlayer Client - /api/start - ok, got:", startSession.SessionId);

//server.tempSessionID = "F06CB311-246D-415B-86F1-0ED060A27A3F";
var result2=await server.IsSessionsReady();
Console.WriteLine("xCloudPlayer Client - /api/start - Session is ready!", result2, "Waiting...");
/*
// Create a new WebRTC connection
var webRtcConnection = new WebRtcConnection();

// Handle new messages
webRtcConnection.OnIceCandidate += (string candidate) =>
{
    // Handle ICE Candidate messages
};

webRtcConnection.OnSdpMessage += (string sdp) =>
{
    // Handle SDP signaling messages
};

*/

