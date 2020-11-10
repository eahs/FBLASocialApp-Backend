using ADSBackend.Models;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Odbc;

namespace ADSBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ConfigurationItem> ConfigurationItem { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<ChatSession> ChatSession { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Reaction> Reaction { get; set; }
        public DbSet<Wall> Wall { get; set; }
        public DbSet<WallPost> WallPost { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<MemberFriend>()
                .HasKey(t => new { t.MemberId, t.FriendId });

            builder.Entity<MemberFriend>()
                .HasOne(mf => mf.Member)
                .WithMany(m => m.Friends)
                .HasForeignKey(cm => cm.MemberId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostComment>()
                .HasKey(t => new { t.PostId, t.CommentId });

            builder.Entity<PostComment>()
                .HasOne(pc => pc.Post)
                .WithMany(cm => cm.Comments)
                .HasForeignKey(cm => cm.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostReaction>()
                    .HasKey(t => new { t.PostId, t.ReactionId });

            builder.Entity<PostReaction>()
                .HasOne(pr => pr.Post)
                .WithMany(react => react.Reactions)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<WallPost>()
                .HasKey(t => new { t.WallId, t.PostId });

            builder.Entity<WallPost>()
                .HasOne(pr => pr.Wall)
                .WithMany(pr => pr.Posts)
                .HasForeignKey(wall => wall.WallId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<ChatSessionMember>()
                .HasKey(t => new { t.ChatSessionId, t.MemberId });

            builder.Entity<ChatSessionMember>()
                .HasOne(c => c.ChatSession)
                .WithMany(cm => cm.ChatMembers)
                .HasForeignKey(c => c.ChatSessionId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<ChatSessionMember>()
                .HasOne(c => c.Member)
                .WithMany(m => m.ChatSessions)
                .HasForeignKey(m => m.MemberId);           

            builder.Entity<PostPhoto>()
                .HasKey(t => new { t.PostId, t.PhotoId });

            builder.Entity<PostPhoto>()
                .HasOne(p => p.Post)
                .WithMany(p => p.Images)
                .HasForeignKey(p => p.PostId);

            builder.Entity<Member>()
                .HasOne(m => m.ProfilePhoto)
                .WithOne()
                .HasForeignKey<Member>(m => m.ProfilePhotoPhotoId);
        }
    }
}
