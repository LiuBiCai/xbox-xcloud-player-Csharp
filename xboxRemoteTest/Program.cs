﻿using System;
using xboxRemoteTest;

//Remote remote=new Remote();
//var  result=remote.Consoles();
//Console.WriteLine(result.Content);
string userToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkFGNEZBQUY2LUU0NjMtNDk5OC05MjM1LUJCRDFCNTlEOUM0RCIsInR5cCI6IkpXVCJ9.eyJhcHBpZCI6IjEwMTY4OTg0MzkiLCJjb3VudHJ5IjoiMjAiLCJjb3VudHJ5Y29kZSI6IkNOIiwiZGV2aWNlaWQiOiJGNzAwQzI4QThCMTBGMkFGIiwiZGV2aWNldHlwZSI6IkFuZHJvaWQiLCJ1c2VyaWQiOiJCaXphdDE5ODgiLCJwdWlkIjoiODQ0NDI1MDAzNTg4ODkyIiwieHVpZCI6IjI1MzU0NTA0NjU0OTIwMzMiLCJ4dXNGbGlnaHRzIjoiW1wiNDYxMTY4NjAxODU0NzMzMjAwMFwiLFwiYjRlYzI1NGUtY2VjMC00OWRkLTgzMTMtZWY1ZDA1ZmJhZWY1XCIsXCJjMTYzNzMyYS1kZWI2LTQxNDAtYjljNS03Y2ZkM2QwYWI4NzFcIixcIjFjM2Y3OGFiLTlhYmEtNGQ1Yy04MzcwLTg2YTVlNzg5YTk3Y1wiXSIsInBhcnRuZXJpZCI6Ik1JQ1JPU09GVCIsIm9mZmVyaW5naWQiOiJYSE9NRSIsImluc3RhbmNlaWQiOiIwOTQ5Y2Q1OS1mNjVhLTRjNTYtOTI4MS0xMzk2MWM4NDFkNDMiLCJ0eXBlIjoiVXNlciIsInZlcnNpb24iOiIyLjAiLCJmbGlnaHRzIjoie30iLCJuYmYiOjE2ODk0MDQ4NjYsImV4cCI6MTY4OTQxOTI2NiwiaWF0IjoxNjg5NDA0ODY2LCJpc3MiOiJodHRwczovL3hob21lLWF1dGgtcHJvZC54Ym94bGl2ZS5jb20iLCJhdWQiOiJodHRwczovL3hob21lLXByb2QueGJveGxpdmUuY29tIn0.gz-zhquQBwbDfMBCGa0ZtpZlgko0QwATrZn_JdOcH6Cc3nk-9fn1zeFkrRh1IXs_lRclGOhpD0nMA5P9_GZrI2t_qhJ9kHUJpNWv7w57-_A1VKVBb2dwtbCSNL5U9C0PRd5PU2d-qv_HBGpkL6TNtWamMJS8EDjyIMWzqeUiM6w";
Server server=new Server();
server.userToken = userToken;
//await server.GetConsoles();
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

