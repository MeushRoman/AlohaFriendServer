using System;
using System.Collections.Generic;
using System.Text;

namespace AlohaFriend.Server.Models
{
    public enum ChatRoomType
    {
        UserToUserChat, 
        GroupChat
    }

    public class ChatRoom
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ChatRoomType ChatRoomType { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
        public ICollection<ApplicationUserChatRoomJunction> ApplicationUsers { get; set; }
    }
}
