using System;
using System.Collections.Generic;
using System.Text;

namespace AlohaFriend.Server.Models
{
    public enum ApplicationUserInChatStatus
    {
        Active, Banned
    }

    public class ApplicationUserChatRoomJunction
    {
        public int ChatRoomId { get; set; }
        public int ApplicationUserId { get; set; }
        public ChatRoom ChatRoom { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ApplicationUserInChatStatus ApplicationUserStatus { get; set; }
    }
}
