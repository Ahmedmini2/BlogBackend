using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Seed 10,000 blog posts with varied content
        var posts = new List<BlogPost>();
        var random = new Random();
        for (int i = 1; i <= 10000; i++)
        {
            posts.Add(new BlogPost
            {
                Id = i,
                Title = $"Sample Post {i}",
                Content = $"This is the content of sample post {i}.",
                Author = $"Author {random.Next(1, 100)}",
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 365))
            });
        }
        modelBuilder.Entity<BlogPost>().HasData(posts);
    }
}