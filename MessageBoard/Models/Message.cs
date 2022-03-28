using System;
using System.ComponentModel.DataAnnotations;

namespace MessageBoard.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Message title must be between 5 and 50 characters long", MinimumLength = 5)]
        public string Title { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "Message body must be between 5 and 50 characters long", MinimumLength = 5)]
        public string Body { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Group { get; set; }

    }
}