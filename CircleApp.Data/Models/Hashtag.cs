using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CircleApp.Data.Models
{
    public class Hashtag
    {
        [Key]
        public int Id { get; set; }
        public string Tag { get; set; } = string.Empty;
        public int UsageCount { get; set; } = 0;

        // Many-to-many relationship with Post
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
