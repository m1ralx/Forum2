using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum2.Models
{
    public class Board
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Board name must be less than 100 characters", MinimumLength = 6)]
        [Display(Name = "Board name")]
        public string Name { get; set; }
        [StringLength(2000, ErrorMessage = "Short description of Board theme must be less than 2000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual ICollection<ForumThread> Threads { get; set; }
    }
}