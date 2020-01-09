using System;
using System.Collections.Generic;

namespace AlohaFriend.Server.Services
{
    public class SessionManagementService
    {
        private readonly Dictionary<string, int> _activeSessionUserMappings;
        public SessionManagementService()
        {
            _activeSessionUserMappings = new Dictionary<string, int>();
        }

        public string AddSession(int userId)
        {
            string sessionId = Guid.NewGuid().ToString();
            _activeSessionUserMappings[sessionId] = userId;

            return sessionId;
        }

        public int ? GetUserBySession(string sessionId)
        {
            if (_activeSessionUserMappings.ContainsKey(sessionId))
                return _activeSessionUserMappings[sessionId];

            else return null;         
        }
    }
}
