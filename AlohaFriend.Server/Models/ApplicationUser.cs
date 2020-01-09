using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaFriend.Server.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<ApplicationUserChatRoomJunction> ChatRooms { get; set; }
    }
}
