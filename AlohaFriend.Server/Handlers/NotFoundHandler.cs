using System.Net;

namespace AlohaFriend.Server.Handlers
{
    public class NotFoundHandler : IHandler
    {
        public void Handle(
            HttpListenerRequest request, 
            HttpListenerResponse response)
        {
            response.StatusCode = 404;
        }
    }
}
