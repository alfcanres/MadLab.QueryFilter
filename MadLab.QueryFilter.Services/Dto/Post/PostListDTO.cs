public class PostListDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int AuthorId { get; set; }
    public string PostTypeName { get; set; }
    public int PostTypeId { get; set; }
    public string MoodTypeName { get; set; }
    public int MoodTypeId { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsPublished { get; set; }
}