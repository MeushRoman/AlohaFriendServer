using System.Net;

namespace AlohaFriend.Server.Handlers
{
    public interface IHandler
    {
        public void Handle(HttpListenerRequest request, HttpListenerResponse response);
    }
}
