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
    public class JuncrionHandler : IHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SessionManagementService _sessionManagementService;

        public JuncrionHandler(ApplicationDbContext dbContext,
            SessionManagementService sessionManagementService)
        {
            _dbContext = dbContext;
            _sessionManagementService = sessionManagementService;
        }

        public ApplicationUser GetUserByPhoneNumber(string phoneNumber)
        {
            var user = _dbContext.ApplicationUsers.SingleOrDefault(p => p.PhoneNumber == phoneNumber);

            if (user == null)
                return null;
            else
                return user;
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var ms = new MemoryStream())
            {
                request.InputStream.CopyTo(ms);
                var content = Encoding.UTF8.GetString(ms.ToArray());
                var junctionModel = JObject.Parse(content);

                var sessionId = junctionModel["SessionId"].ToString();
                var chatRoomId = junctionModel["ChatRoomId"].ToString();
                var userPhoneNumber = junctionModel["UserPhoneNumber"].ToString();

                var user = GetUserByPhoneNumber(userPhoneNumber);

                var sender = _sessionManagementService.GetUserBySession(sessionId);

                if (sender == null || chatRoomId == null || user == null)
                {
                    response.StatusCode = 404;

                    response.OutputStream.Write(Encoding.UTF8.GetBytes("NOT FOUND!"));
                    response.Close();
                    return;
                }

                var junction = new ApplicationUserChatRoomJunction()
                {
                    ApplicationUserId = user.Id,
                    ChatRoomId = Int32.Parse(chatRoomId),
                    ApplicationUserStatus = ApplicationUserInChatStatus.Active
                };

                _dbContext.ApplicationUserChatRooms.Add(junction);
                _dbContext.SaveChanges();

                response.StatusCode = 200;
                response.Close();
            }
        }
    }
}
