using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace StudentTaskManagement.Models
{
    public class StudentTaskManagementContext : IdentityDbContext<L1Students>
    {
        public StudentTaskManagementContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Change AspNetUsers table name to L1Students
            modelBuilder.Entity<L1Students>().ToTable("L1Students");
            // Optionally, you can also rename other Identity tables if needed:
            modelBuilder.Entity<IdentityRole>().ToTable("L1Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("L1UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("L1UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("L1UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("L1RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("L1UserTokens");

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<L1Tasks>()
                .HasMany(t => t.L1SubTasks)
                .WithOne(s => s.L1Tasks)
                .HasForeignKey(s => s.L1TaskId);

            modelBuilder.Entity<L1NotificationPresets>()
                .Property(b => b.IsSystemDefault)
                .HasDefaultValue(false);

            modelBuilder.Entity<L1RecurringPatterns>()
                .Property(b => b.IsSystemDefault)
                .HasDefaultValue(false);

/*            // Configure the relationship
            modelBuilder.Entity<L1DiscussionForumLikes>()
                .HasOne(l => l.L1DiscussionForums)
                .WithMany(f => f.Likes)
                .HasForeignKey(l => l.L1DiscussionForumId);

            modelBuilder.Entity<L1DiscussionForumLikes>()
                .HasOne(l => l.CreatedByStudent)
                .WithMany()
                .HasForeignKey(l => l.CreatedByStudentId);*/
        }
        public DbSet<L1Tasks> L1Tasks { get; set; }
        public DbSet<L1SubTasks> L1SubTasks { get; set; }
        public DbSet<L1DiscussionForumComments> L1DiscussionForumComments { get; set; }
        public DbSet<L1DiscussionForums> L1DiscussionForums { get; set; }
        public DbSet<L1RecurringPatterns> L1RecurringPresets { get; set; }
        public DbSet<L1NotificationPresets> L1NotificationPresets { get; set; }
        public DbSet<L1Students> L1Students { get; set; }
        public DbSet<L1DiscussionForumLikes> L1DiscussionForumLikes { get; set; }
        public DbSet<NotificationQueues> NotificationQueues { get; set; }
        public DbSet<PushSubscriptions> PushSubscriptions { get; set; }

    }
}