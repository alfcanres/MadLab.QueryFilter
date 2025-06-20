// See https://aka.ms/new-console-template for more information

using MadLab.QueryFilter.Domain;
using Microsoft.EntityFrameworkCore;


string connectionString = "Data Source=SimpleBlog.db";

using (var context = new DataBaseContext(connectionString))
{
   

    await CreateDummyData(context); 


    var list = await context.Posts
        .ToListAsync();

    foreach (var post in list)
    {
        Console.WriteLine($"Post ID: {post.Id}, Title: {post.Title}, Content: {post.Text}");
    }


    Console.ReadLine();
}


async Task CreateDummyData(DataBaseContext context)
{
    // Ensure database is created
    context.Database.EnsureCreated();



    bool createMoodTypes = !context.MoodTypes.Any();
    bool createPostTypes = !context.PostTypes.Any();

    if (createMoodTypes || createPostTypes)
    {
        List<string> moodTypes = new List<string> { "Happy", "Sad", "Angry", "Excited" };
        List<string> postTypes = new List<string> { "Text", "Image", "Video", "Link" };
        await DbInitializer.CreateMoodTypesAndPostTypes(context, createMoodTypes, moodTypes, createPostTypes, postTypes);
    }

    if (!context.Users.Any())
    {
        List<string> dummyNames = new List<string> { "John", "Jane", "Alice", "Bob" };
        List<string> dummyLastNames = new List<string> { "Doe", "Smith", "Johnson", "Brown" };
        await DbInitializer.CreateDummyUsers(context, 10, dummyNames, dummyLastNames, "yourStrong(!)Password");
    }

    if (!context.Posts.Any())
    {
        List<string> dummyPostTitles = new List<string> { "discover",
        "amazing",
        "journey",
        "future",
        "create",
        "ideas",
        "explore",
        "challenge",
        "dream",
        "inspire",
        "learn",
        "share",
        "moment",
        "grow",
        "together",
        "change",
        "vision",
        "success",
        "believe",
        "story" 
        };
        
        List<string> dummyPostContents = new List<string> {         
        "connect",
        "discoveries",
        "opportunity",
        "motivate",
        "focus",
        "energy",
        "progress",
        "community",
        "support",
        "challenge",
        "growth",
        "passion",
        "goal",
        "achievement",
        "collaborate",
        "imagine",
        "reflect",
        "advance",
        "encourage",
        "potential" };

        await DbInitializer.CreateDummyPosts(context, 250, dummyPostTitles, dummyPostContents);
    }
}