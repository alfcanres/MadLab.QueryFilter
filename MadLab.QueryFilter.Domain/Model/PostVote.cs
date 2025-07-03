using System.ComponentModel.DataAnnotations;

namespace MadLab.QueryFilter.Domain
{
    public class PostVote
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [Required]
        public Post Post { get; set; }
        public int UserId { set; get; }
        [Required]
        public User User { get; set; }
        [Required]
        public bool ILikedThis { get; set; }
        [Required]
        public DateTime VoteDate { get; set; }
    }
}
