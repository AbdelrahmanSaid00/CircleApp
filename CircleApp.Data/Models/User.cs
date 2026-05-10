using System;
using System.Collections.Generic;
using System.Text;

namespace CircleApp.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }

        //Navigation property for the posts created by the user
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
