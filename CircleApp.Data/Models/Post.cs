using System.ComponentModel.DataAnnotations;

namespace CircleApp.Data.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public int NrOfReports { get; set; }
        public DateTime DataCreated { get; set; }
        public DateTime DataUpdated { get; set; }

        // Foreign key to the User who created the post
        public int UserId { get; set; }
        //Navigation property 
        public User User { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
