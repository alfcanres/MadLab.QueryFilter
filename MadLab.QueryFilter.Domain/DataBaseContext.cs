
using Microsoft.EntityFrameworkCore;
using System;


namespace MadLab.QueryFilter.Domain
{
    public class DataBaseContext : DbContext
    {

        /// <summary>
        /// This constructor is used to create a new instance of the DataBaseContext with a connection string.
        /// notice is using SQLite as the database provider, which is suitable for lightweight applications or testing purposes.
        /// and it is not recommended for production use. The database will be created in the local file system, that means your computer
        /// </summary>
        /// <param name="connectionString"></param>
        public DataBaseContext(string connectionString)
              : base(new DbContextOptionsBuilder<DataBaseContext>()
                  .UseSqlite(connectionString)
                  .Options)
        {
        }

        /// <summary>
        /// We are using this constructor so we can  use InMemory database for testing purposes.
        /// </summary>
        /// <param name="options"></param>
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            //We can actually do thise with Data Annotations, but I prefer to use Fluent API for more complex configurations.
            //this is not a so complex configuration, but just wanted to show you how to use Fluent API.
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
