using AlohaFriend.Server.Contexts;
using AlohaFriend.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using AlohaFriend.Server.Services;
using AlohaFriend.Server.Handlers;
using Fleck;
using Newtonsoft.Json.Linq;

namespace AlohaFriend.Server
{
    class Program
    {
        static async Task RunHttpServer(ServiceProvider serviceProvider)
        {
            var worker = Task.Run(() =>
            {
                var bindingAddress = "http://localhost:15100/";//http://127.0.0.1:15100/
                var httpServer = new HttpListener();
                httpServer.Prefixes.Add(bindingAddress);
                httpServer.Start();

                while (true)
                {
                    var connectionContext = httpServer.GetContext();
                    var request = connectionContext.Request;

                    var routingService = serviceProvider.GetService<RoutingService>();
                    var handler = routingService.GetHandlerByRoute(request.RawUrl);

                    handler.Handle(connectionContext.Request, connectionContext.Response);
                }
            });

            await worker;
        }

        static async Task OnConnectionOpened(
            IWebSocketConnection connection,
            ICollection<IWebSocketConnection> templeConnections)
        {
            templeConnections.Add(connection);

            await connection.Send("OK");
        }

        static async Task OnMessage(
            IWebSocketConnection connection,
            SessionManagementService sessionManagement,
            WebSocketConnectionManager webSocketConnectionManager,
            ApplicationDbContext applicationDbContext,
            string message)
        {
            var json = JObject.Parse(message);
            var sessionId = json["sessionId"].ToString();

            var userId = sessionManagement.GetUserBySession(sessionId);
            var user = applicationDbContext.ApplicationUsers.Find(userId);

            webSocketConnectionManager.AddSocketConnection(sessionId, connection);

            foreach (var activeConnection in webSocketConnectionManager.GetAllActiveConnections())
            {
                await activeConnection.Send($"Пользователь {user.PhoneNumber} онлайн!");
            }

            await connection.Send("OK");
        }

        static async Task Main(string[] args)
        {
            var connectionString = "Server=SQLNCLI11; Data Source=LENOVO-PC; Integrated Security=SSPI;Database=AlohaFriendsDB";
            
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString))
                .AddSingleton(typeof(RoutingService))
                .AddTransient(typeof(SignUpHandler))
                .AddTransient(typeof(SignInHandler))
                .AddTransient(typeof(NewChatRoomHandler))
                .AddTransient(typeof(GetUsersHandler))
                .AddTransient(typeof(JuncrionHandler))
                .AddSingleton(typeof(SessionManagementService))
                .AddSingleton(typeof(WebSocketConnectionManager))
                .BuildServiceProvider();

            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();           

            var templeConnections = new List<IWebSocketConnection>();
            var bindingAddress = "ws://localhost:15200";
            var sessionManagement = serviceProvider.GetService<SessionManagementService>();
            var webSocketConnectionManager = serviceProvider.GetService<WebSocketConnectionManager>();

            var wrapper = new WebSocketServer(bindingAddress);

            wrapper.Start(socket =>
            {
                socket.OnOpen = async () => await OnConnectionOpened(socket, templeConnections);
                socket.OnClose = () => Console.WriteLine("Close!");
                socket.OnMessage = async message => await OnMessage(socket, sessionManagement, webSocketConnectionManager, dbContext, message);
            });

            var httpServerWorker = RunHttpServer(serviceProvider);
            await httpServerWorker;
            Console.ReadLine();

            wrapper.Dispose();
        }
    }
}
