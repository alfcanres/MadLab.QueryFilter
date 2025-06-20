using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadLab.QueryFilter.Domain
{
    public class MoodType
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Mood must have a description")]
        public string Mood { get; set; }
        [Required]
        public bool IsAvailable { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
