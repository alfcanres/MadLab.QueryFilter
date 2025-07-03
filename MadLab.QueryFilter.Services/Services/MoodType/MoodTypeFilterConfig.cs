using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadLab.QueryFilter.Services.Services
{
    public class MoodTypeFilterConfig
    {
        public bool? IsAvailable { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public bool OnlyWithPosts { get; set; } = false;
        public bool Paged { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
