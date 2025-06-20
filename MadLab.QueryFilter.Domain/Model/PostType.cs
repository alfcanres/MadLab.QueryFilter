using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
