// See https://aka.ms/new-console-template for more information

using MadLab.QueryFilter.Domain;

using MadLab.QueryFilter.Domain.Repository;
using MadLab.QueryFilter.Services.Services;


string connectionString = "Data Source=SimpleBlog.db";

using (var context = new DataBaseContext(connectionString))
{
    await DbInitializer.CreateDemoData(context);

    // Set up repository and service
    var postRepository = new RepositoryBase<Post>(context);
    var postService = new PostService(postRepository);

    // Use the service to get all posts paged (first page, 100 posts)
    var posts = await postService.GetAllPaged(1, 100);

    // Table header
    Console.WriteLine(new string('=', 120));
    Console.WriteLine(
        $"| # | {"ID",3} | {"Title",-25} | {"Author",-15} | {"Type",-12} | {"Mood",-10} | {"Created",-16} | {"Pub",-3} |");
    Console.WriteLine(new string('=', 120));
    int number = 1;
    // Table rows
    foreach (var post in posts)
    {
        var authorName = post.Author ?? "Unknown";
        var postType = post.PostTypeName ?? "N/A";
        var moodType = post.MoodTypeName ?? "N/A";
        var created = post.CreationDate.ToString("yyyy-MM-dd HH:mm");
        var published = post.IsPublished ? "Yes" : "No";

        Console.WriteLine(
            $"| {number} | {post.Id,3} | {Truncate(post.Title, 25),-25} | {Truncate(authorName, 15),-15} | {Truncate(postType, 12),-12} | {Truncate(moodType, 10),-10} | {created,-16} | {published,3} |");

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