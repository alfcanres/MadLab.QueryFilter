using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadLab.QueryFilter.Domain
{
    public class Post
    {
        public int Id { get; set; }
        public int ApplicationUserInfoId { set; get; }
        public User ApplicationUserInfo { get; set; }

        public int PostTypeId { get; set; }
        [Required]
        public PostType PostType { get; set; }

        public int MoodTypeId { get; set; }
        [Required]
        public MoodType MoodType { get; set; }

        [MaxLength(255, ErrorMessage = "Max chars 255")]
        [Required(ErrorMessage = "Please type a title for your post")]
        public string Title { set; get; }

        [MaxLength(1024, ErrorMessage = "Avoid TDL!!! Max chars 1024")]
        [Required(ErrorMessage = "Need some text here!")]
        public string Text { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }

        public Nullable<DateTime> PublishDate { get; set; }
        [Required]
        public bool IsPublished { get; set; }

        public ICollection<PostVote> Votes { set; get; }

        public ICollection<PostComment> Comments { set; get; }

    }
}
