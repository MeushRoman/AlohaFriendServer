using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlohaFriend.Server.Services
{
    public class WebSocketConnectionManager
    {
        private readonly Dictionary<string, IWebSocketConnection> _webSocketConnection;

        public WebSocketConnectionManager()
        {
            _webSocketConnection = new Dictionary<string, IWebSocketConnection>();
        }

        public void AddSocketConnection(string sessionId, IWebSocketConnection connection)
        {
            _webSocketConnection[sessionId] = connection;
        }
        public IWebSocketConnection GetConnectionBySession(string sessionId)
        {
            if (_webSocketConnection.ContainsKey(sessionId))
                return _webSocketConnection[sessionId];

            else return null;
        }

        public IEnumerable<IWebSocketConnection> GetAllActiveConnections()
        {
            return _webSocketConnection.Select(p => p.Value);
        }

    }
}
