using System;
using System.ComponentModel.DataAnnotations;

namespace CircleApp.Data.Models
{
    public class Story
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Foreign key to User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
