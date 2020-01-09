using AlohaFriend.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AlohaFriend.Server.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ApplicationUserChatRoomJunction> ApplicationUserChatRooms { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        #region Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasKey(p => p.Id);

            modelBuilder.Entity<ApplicationUserChatRoomJunction>()
                .HasKey(p => new { p.ApplicationUserId, p.ChatRoomId });

            modelBuilder.Entity<ApplicationUserChatRoomJunction>()
                .HasOne(p => p.ApplicationUser)
                .WithMany(p => p.ChatRooms)
                .HasForeignKey(p => p.ApplicationUserId);

            modelBuilder.Entity<ApplicationUserChatRoomJunction>()
                .HasOne(p => p.ChatRoom)
                .WithMany(p => p.ApplicationUsers)
                .HasForeignKey(p => p.ChatRoomId);


            modelBuilder.Entity<ChatRoom>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<ChatRoom>()
                .HasMany(p => p.Messages)
                .WithOne(p => p.ChatRoom)
                .HasForeignKey(p => p.ChatRoomId);
        }
        #endregion
    }
}
