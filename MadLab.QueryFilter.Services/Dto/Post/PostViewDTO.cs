public class PostViewDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Author { get; set; }
    public string PostTypeName { get; set; }
    public string MoodTypeName { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? PublishDate { get; set; }
    public bool IsPublished { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
}