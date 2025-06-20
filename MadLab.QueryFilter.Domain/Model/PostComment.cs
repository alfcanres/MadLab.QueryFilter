using System.ComponentModel.DataAnnotations;

namespace MadLab.QueryFilter.Domain
{
    public class PostComment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [Required]
        public Post Post { get; set; }
        public int ApplicationUserInfoId { set; get; }
        [Required]
        public User ApplicationUserInfo { get; set; }
        [Required]
        public DateTime CommentDate { set; get; }

        [Required]
        [MaxLength(255)]
        public string CommentText { set; get; }
    }
}
