public class PostUpdateDTO
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int PostTypeId { get; set; }
    public int MoodTypeId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public bool IsPublished { get; set; }
}