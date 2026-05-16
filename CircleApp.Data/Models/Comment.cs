using System;
using System.Collections.Generic;
using System.Text;

namespace CircleApp.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdate { get; set; }
        
        //Foreign keys
        public int postId { get; set; }
        public int userId { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public User User { get; set; }
    }
}
