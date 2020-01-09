using AlohaFriend.Server.Contexts;
using AlohaFriend.Server.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AlohaFriend.Server.Handlers
{

    public class GetUsersHandler : IHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SessionManagementService _sessionManagementService;

        public GetUsersHandler(ApplicationDbContext dbContext,
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
                var junctionModel = JObject.Parse(content);

                var sessionId = junctionModel["SessionId"].ToString();

                var userId = _sessionManagementService.GetUserBySession(sessionId);

                if (userId == null)
                {
                    response.StatusCode = 404;

                    response.OutputStream.Write(Encoding.UTF8.GetBytes("NOT FOUND!"));
                    response.Close();
                    return;
                }

                List<string> users = new List<string>();

                foreach (var user in _dbContext.ApplicationUsers)
                {
                    users.Add(user.PhoneNumber);
                }

                var jsonObject = JsonConvert.SerializeObject(users);


                response.OutputStream.Write(Encoding.UTF8.GetBytes(jsonObject));

                response.StatusCode = 200;
                response.Close();
            }
        }
    }
}
