using AlohaFriend.Server.Contexts;
using AlohaFriend.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace AlohaFriend.Server.Handlers
{
    public class SignUpHandler : IHandler
    {
        private readonly ApplicationDbContext _dbContext;
        public SignUpHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            var rand = new Random();

            using (var ms = new MemoryStream())
            {
                request.InputStream.CopyTo(ms);
                var content = Encoding.UTF8.GetString(ms.ToArray());
                var registrationModel = JObject.Parse(content);

                var phoneNumber = registrationModel["PhoneNumber"].ToString();
                var password = rand.Next(1000, 9999).ToString();

                Console.WriteLine(phoneNumber + " : " + password);

                var user = _dbContext.ApplicationUsers.Add(new ApplicationUser()
                {
                    PhoneNumber = phoneNumber,
                    PasswordHash = password
                });

                _dbContext.SaveChanges();

                response.StatusCode = 201;
                response.OutputStream.Write(Encoding.UTF8.GetBytes("OK!"));
                response.Close();
            }
        }
    }
}
