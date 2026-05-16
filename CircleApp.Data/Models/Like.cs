using System;
using System.Collections.Generic;
using System.Text;

namespace CircleApp.Data.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int postId { get; set; }
        public int userId { get; set; }
        
        // Navigation properties
        public Post Post { get; set; }
        public User User { get; set; }

    }
}
