
using Microsoft.EntityFrameworkCore;

namespace MadLab.QueryFilter.Domain
{
    public static class DbInitializer
    {

        public static async Task CreateMoodTypesAndPostTypes(
            DataBaseContext appDbContext,
            bool createmoodTypes,
            IEnumerable<string> moodTypes,
            bool createPostTypes,
            IEnumerable<string> postTypes)
        {

            if (createPostTypes && moodTypes == null)
                throw new ArgumentNullException(nameof(moodTypes), "Mood types cannot be null when creating post types.");

            if (createmoodTypes && postTypes == null)
                throw new ArgumentNullException(nameof(postTypes), "Post types cannot be null when creating mood types.");

            if (createmoodTypes)
                foreach (var mood in moodTypes)
                {
                    if (!appDbContext.MoodTypes.Any(pt => pt.Mood == mood))
                    {
                        appDbContext.MoodTypes.Add(new MoodType
                        {
                            Mood = mood,
                            IsAvailable = true
                        });
                    }
                }

            if (createPostTypes)
                foreach (var postType in postTypes)
                {
                    if (!appDbContext.PostTypes.Any(pt => pt.Description == postType))
                    {
                        appDbContext.PostTypes.Add(new PostType
                        {
                            Description = postType,
                            IsAvailable = true
                        });
                    }
                }
            await appDbContext.SaveChangesAsync();
        }

        public static async Task CreateDummyUsers(
            DataBaseContext appDbContext,
            int dummyUserCount,
            IEnumerable<string> dummyNames,
            IEnumerable<string> dummyLastNames,
            string defaultPasswordForDummyUsers
            )
        {


            for (int i = 0; i < dummyUserCount; i++)
            {
                var firstName = GetRandomStringFromList(dummyNames);
                var lastName = GetRandomStringFromList(dummyLastNames);

                string userEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";
                string userPassword = defaultPasswordForDummyUsers;

                if (!appDbContext.Users.Any(u => u.UserName == firstName && u.LastName == lastName))
                {
                    appDbContext.Users.Add(new User
                    {
                        UserName = userEmail,
                        LastName = lastName,
                        FirstName = firstName,
                        IsActive = true
                    });
                }

                await appDbContext.SaveChangesAsync();

            }

        }

        public static async Task CreateDummyPosts(
            DataBaseContext appDbContext,
            int dummyPostCount,
            IEnumerable<string> dummyPostTitles,
            IEnumerable<string> dummyPostContents)
        {
            //Moving this validation to InitialDataOptionsValidator class
            if (dummyPostCount <= 0 || (dummyPostTitles == null && dummyPostContents == null))
                throw new ArgumentException("Dummy post count must be greater than zero and titles/contents cannot be null.");

            var users = await appDbContext.Users.ToListAsync();

            if (!users.Any())
                throw new InvalidOperationException("No users found to create posts for.");

            var moodTypes = await appDbContext.MoodTypes.ToListAsync();
            if (!moodTypes.Any())
                throw new InvalidOperationException("No mood types found to assign to posts.");

            var postTypes = await appDbContext.PostTypes.ToListAsync();
            if (!postTypes.Any())
                throw new InvalidOperationException("No post types found to assign to posts.");


            for (int i = 0; i < dummyPostCount; i++)
            {
                var randomUser = users[new Random().Next(users.Count)];
                var postTitle = GetRandomSentenceFromList(dummyPostTitles, 5, 25);
                var postContent = GetRandomSentenceFromList(dummyPostContents, 2, 25);
                var moodType = moodTypes[new Random().Next(moodTypes.Count)];
                var postType = postTypes[new Random().Next(postTypes.Count)];
                var randomDate = GetRandomDateFromYearStartToNow();


                appDbContext.Posts.Add(new Post
                {
                    Title = postTitle,
                    ApplicationUserInfoId = randomUser.Id,
                    ApplicationUserInfo = randomUser,
                    Text = postContent,
                    CreationDate = randomDate,
                    IsPublished = true,
                    PublishDate = randomDate,
                    PostType = postType,
                    PostTypeId = postType.Id,
                    MoodType = moodType,
                    MoodTypeId = moodType.Id
                });
            }

            await appDbContext.SaveChangesAsync();
        }


        internal static string GetRandomStringFromList(IEnumerable<string> list)
        {
            if (list == null || !list.Any())
                throw new ArgumentException("List cannot be null or empty.", nameof(list));

            var array = list as string[] ?? list.ToArray();
            var random = new Random();
            int index = random.Next(array.Length);
            return array[index];
        }

        internal static string GetRandomSentenceFromList(IEnumerable<string> list, int minWords, int maxWords)
        {
            if (list == null || !list.Any())
                throw new ArgumentException("List cannot be null or empty.", nameof(list));
            if (minWords < 1)
                throw new ArgumentOutOfRangeException(nameof(minWords), "Minimum words must be at least 1.");
            if (maxWords < minWords)
                throw new ArgumentOutOfRangeException(nameof(maxWords), "Maximum words must be greater than or equal to minimum words.");

            var array = list as string[] ?? list.ToArray();
            int available = array.Length;
            int actualMax = Math.Min(maxWords, available);
            int actualMin = Math.Min(minWords, actualMax);

            var random = new Random();
            int wordCount = random.Next(actualMin, actualMax + 1);

            // Shuffle and take wordCount words (no repeats)
            var sentenceWords = array.OrderBy(_ => random.Next()).Take(wordCount).ToArray();

            // Capitalize first word, join with spaces, and add period
            if (sentenceWords.Length == 0)
                return string.Empty;

            sentenceWords[0] = char.ToUpper(sentenceWords[0][0]) + sentenceWords[0].Substring(1);
            var sentence = string.Join(" ", sentenceWords) + ".";

            return sentence;
        }
        internal static DateTime GetRandomDateFromYearStartToNow()
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, 1, 1);
            var range = (now - start).Days;

            if (range <= 0)
                return start;

            var random = new Random();
            int daysToAdd = random.Next(0, range + 1);
            return start.AddDays(daysToAdd)
                        .AddHours(random.Next(0, 24))
                        .AddMinutes(random.Next(0, 60))
                        .AddSeconds(random.Next(0, 60));
        }
    }
}
