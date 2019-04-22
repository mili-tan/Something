using System;
using System.Collections.Generic;
using System.Linq;
using Fleck;

namespace mWsket
{
    static class Program
    {
        static void Main()
        {
            var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://0.0.0.0:23333");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    var cPort = socket.ConnectionInfo.ClientPort;
                    Console.WriteLine(cPort + ": " + "Open!");
                    socket.Send("服务器: " + "PORT: " + cPort);
                    allSockets.ToList().ForEach(s => s.Send(cPort + "上线了 " + allSockets.Count + "人当前在线"));
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    var cPort = socket.ConnectionInfo.ClientPort;
                    Console.WriteLine(cPort + ": " + "Close!");
                    allSockets.ToList().ForEach(s => s.Send(cPort + "下线了 " + allSockets.Count + "人当前在线"));
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    var cPort = socket.ConnectionInfo.ClientPort;
                    Console.WriteLine(cPort + ": " + message);
                    allSockets.ToList().ForEach(s => s.Send(cPort + ": " + message));
                };
            });


            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send("服务器: " + input);
                }
                input = Console.ReadLine();
            }
        }
    }
}
