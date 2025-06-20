
using Microsoft.EntityFrameworkCore;


namespace MadLab.QueryFilter.Domain
{
    public class DataBaseContext : DbContext
    {

        public DataBaseContext(string connectionString)
              : base(new DbContextOptionsBuilder<DataBaseContext>()
                  .UseSqlite(connectionString)
                  .Options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Post>()
                .HasMany(t => t.Comments)
                .WithOne(t => t.Post)
                .HasForeignKey(t => t.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Post>()
                .HasMany(t => t.Votes)
                .WithOne(t => t.Post)
                .HasForeignKey(t => t.PostId)
                .OnDelete(DeleteBehavior.NoAction);



            base.OnModelCreating(builder);

        }
  
        public DbSet<MoodType> MoodTypes => Set<MoodType>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostType> PostTypes => Set<PostType>();
        public DbSet<PostVote> PostVotes => Set<PostVote>();
        public DbSet<PostComment> PostComments => Set<PostComment>();
        public DbSet<User> Users => Set<User>();
    }
}
