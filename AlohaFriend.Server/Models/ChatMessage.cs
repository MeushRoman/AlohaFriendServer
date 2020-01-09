using System;
using System.Collections.Generic;
using System.Text;

namespace AlohaFriend.Server.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public int SenderApplicationUserId { get; set; }
        public ApplicationUser SenderApplicationUser { get; set; }
        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }
    }
}
