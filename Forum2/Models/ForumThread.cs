using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Forum.Models;

namespace Forum2.Models
{
    public class ForumThread
    {
        public int Id { get; set; }
        [Required]
        [StringLength(80, ErrorMessage = "Thread name must be less than 80 characters", MinimumLength = 6)]
        [Display(Name = "Thread name")]
        public string Name { get; set; }
        [StringLength(2000, ErrorMessage = "Short description of thread must be less than 2000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public virtual Board Board { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}