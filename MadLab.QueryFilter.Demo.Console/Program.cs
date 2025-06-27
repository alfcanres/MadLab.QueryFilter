// See https://aka.ms/new-console-template for more information

using MadLab.QueryFilter.Domain;
using Microsoft.EntityFrameworkCore;


string connectionString = "Data Source=SimpleBlog.db";

using (var context = new DataBaseContext(connectionString))
{
    await DbInitializer.CreateDemoData(context);

    var posts = await context.Posts
        .Include(p => p.Author)
        .Include(p => p.PostType)
        .Include(p => p.MoodType)
        .OrderByDescending(p => p.CreationDate)
        .ToListAsync();

    // Table header
    Console.WriteLine(new string('=', 120));
    Console.WriteLine(
        $"| # | {"ID",3} | {"Title",-25} | {"Author",-15} | {"Type",-12} | {"Mood",-10} | {"Created",-16} | {"Pub",-3} | {"Content Preview",-25} |");
    Console.WriteLine(new string('=', 120));
    int number = 1;
    // Table rows
    foreach (var post in posts)
    {
        var authorName = post.Author?.UserName ?? "Unknown";
        var postType = post.PostType?.Description ?? "N/A";
        var moodType = post.MoodType?.Mood ?? "N/A";
        var preview = post.Text?.Length > 25 ? post.Text.Substring(0, 22) + "..." : post.Text;
        var created = post.CreationDate.ToString("yyyy-MM-dd HH:mm");
        var published = post.IsPublished ? "Yes" : "No";

        Console.WriteLine(
            $"| {number} | {post.Id,3} | {Truncate(post.Title, 25),-25} | {Truncate(authorName, 15),-15} | {Truncate(postType, 12),-12} | {Truncate(moodType, 10),-10} | {created,-16} | {published,3} | {Truncate(preview, 25),-25} |");

        number++;
    }

    Console.WriteLine(new string('=', 120));
    Console.WriteLine("End of posts. Press Enter to exit.");
    Console.ReadLine();
}

// Helper to truncate strings safely
static string Truncate(string value, int maxLength)
{
    if (string.IsNullOrEmpty(value)) return "";
    return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
}


