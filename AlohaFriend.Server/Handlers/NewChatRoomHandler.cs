using AlohaFriend.Server.Contexts;
using AlohaFriend.Server.Models;
using AlohaFriend.Server.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AlohaFriend.Server.Handlers
{
    public class NewChatRoomHandler : IHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SessionManagementService _sessionManagementService;

        public NewChatRoomHandler(ApplicationDbContext dbContext,
            SessionManagementService sessionManagementService)
        {
            _dbContext = dbContext;
            _sessionManagementService = sessionManagementService;
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var ms = new MemoryStream())
            {
                request.InputStream.CopyTo(ms);
                var content = Encoding.UTF8.GetString(ms.ToArray());
                var chatRoomModel = JObject.Parse(content);

                var title = chatRoomModel["Title"].ToString();                
                var type = JsonConvert.DeserializeObject<ChatRoomType>(chatRoomModel["Type"].ToString());
                var sessionId = chatRoomModel["SessionId"].ToString();

                var userId = _sessionManagementService.GetUserBySession(sessionId);

                if (userId == null)
                {
                    response.StatusCode = 404;

                    response.OutputStream.Write(Encoding.UTF8.GetBytes("NOT FOUND!"));
                    response.Close();
                    return;
                }

                var mainChat = new ChatRoom()
                {
                    Title = title,
                    ChatRoomType = type
                };

                _dbContext.ChatRooms.Add(mainChat);

                _dbContext.SaveChanges();

                response.OutputStream.Write(Encoding.UTF8.GetBytes(mainChat.Id.ToString()));
                response.StatusCode = 200;
                response.Close();
            }
        }
    }
}
