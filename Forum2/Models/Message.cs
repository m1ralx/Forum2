using System;
using System.ComponentModel.DataAnnotations;
using Forum.Models;

namespace Forum2.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        [StringLength(4000, ErrorMessage = "Message must be less than 4000 characters")]
        public string Content { get; set; }
        public DateTime PublicationTime { get; set; }
        public virtual ForumThread Thread { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}