﻿using FlexNetCore;
using FlexNetCore.Models.Auth;
using Newtonsoft.Json;
using PocFlex.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main(string[] args)
    {
        var id1 = new Random().Next(100, 999).ToString();
        var id2 = Guid.NewGuid().ToString("N").Substring(0, 8);

        string url = $"wss://event-nguc.weblink.se/{id1}/{id2}/websocket";

        using (var ws = new ClientWebSocket())
        {
            Uri serverUri = new Uri(url);
            
            await ws.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connection established.");

            await ReceiveMessages(ws);
        }
    }

    static async Task ReceiveMessages(ClientWebSocket ws)
    {
        var buffer = new byte[1024 * 4];
        var segment = new ArraySegment<byte>(buffer);

        while (ws.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await ws.ReceiveAsync(segment, CancellationToken.None);
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (message == "o")
            {
                var flexClient = new FlexClient(new FlexAuthorization
                {
                    ClientId = "",
                    GrantType = "",
                    Username = "",
                    Password = ""
                });

                var token = await flexClient.Authenticate();

                var authMessage = $"[\"[\\\"authenticate\\\",{{\\\"authorization\\\":\\\"Bearer {token.AccessToken}\\\"}}]\"]\r\n";
                Console.WriteLine($"AuthMessage: {authMessage} ");

                var authBuffer = Encoding.UTF8.GetBytes(authMessage);
                var authSegment = new ArraySegment<byte>(authBuffer);
                await ws.SendAsync(authSegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (message.StartsWith("c["))
            {
                Console.WriteLine("Authentication failed.");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Authentication failed", CancellationToken.None);
            }
            else if (message == "h")
            {
                Console.WriteLine("Heartbeat received.");
            }
            else if (message.StartsWith("a["))
            {
                Console.WriteLine(message);

                try
                {
                    string pattern = @"\{[^{}]*\}";

                    Match match = Regex.Match(message, pattern);
                    string cleanedInput = match.Value.Replace("\\", "");

                    var userUpdate = JsonConvert.DeserializeObject<UserUpdate>(cleanedInput);

                    string? profileId = GetProfileId(userUpdate);

                    if (String.IsNullOrEmpty(profileId))
                    {
                        Console.WriteLine("Profile invalid");
                    }
                    else
                    {
                        Console.WriteLine($"ProfileId: {profileId}");
                    }

                    Console.WriteLine(JsonConvert.SerializeObject(userUpdate));
                }
                catch (Exception e) 
                {
                    Console.WriteLine("Error: ", e.Message);
                }
            }
        }
        Console.WriteLine("Connection was closed.");
    }

    private static string? GetProfileId(UserUpdate response)
    {
        if(response.Type == UserUpdateType.Forced)
        {
            return response.ForcePbxProfileId;
        }
        else if(response.Type == UserUpdateType.Calendar)
        {
            return response.SchedulePbxProfileId;
        }
        else if(response.Type == UserUpdateType.Override)
        {
            return response.DefaultPbxProfileId;
        }

        return null;
    }
}