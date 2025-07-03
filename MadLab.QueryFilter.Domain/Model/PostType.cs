using System.ComponentModel.DataAnnotations;

namespace MadLab.QueryFilter.Domain
{
    public class PostType
    {
        public int Id { get; set; }

        [MaxLength(255)]
        [Required(ErrorMessage ="Type of post must have a description")]
        public string Description { get; set; }
        [Required]
        public bool IsAvailable { get; set; }

        public ICollection<Post> Posts { get; set;}
    }
}
