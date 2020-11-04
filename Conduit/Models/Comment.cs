using System;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Required]
        public string Body { get; set; }
        public Author Author { get; set; }
    }
}
