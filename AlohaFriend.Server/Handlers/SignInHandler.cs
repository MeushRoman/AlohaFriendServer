using AlohaFriend.Server.Contexts;
using AlohaFriend.Server.Services;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AlohaFriend.Server.Handlers
{
    public class SignInHandler : IHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SessionManagementService _sessionManagementService;

        public SignInHandler(ApplicationDbContext dbContext, 
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
                var registrationModel = JObject.Parse(content);

                var phoneNumber = registrationModel["PhoneNumber"].ToString();
                var password = registrationModel["Password"].ToString();

                var user = _dbContext.ApplicationUsers
                    .SingleOrDefault(p => p.PhoneNumber == phoneNumber && p.PasswordHash == password);

                if (user == null)
                {
                    response.StatusCode = 404;

                    response.OutputStream.Write(Encoding.UTF8.GetBytes("NOT FOUND!"));
                    response.Close();
                    return;
                }

                var session = _sessionManagementService.AddSession(user.Id);
                response.StatusCode = 200;

                response.OutputStream.Write(Encoding.UTF8.GetBytes(session));
                response.Close();
            }
        }
    }
}
