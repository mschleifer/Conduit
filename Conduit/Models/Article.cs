using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Models
{
    public class Article
    {
        public string Slug { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Body { get; set; }
        public List<string> TagList { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Favorited { get; set; }
        public int FavoritesCount { get; set; }
        public Author Author { get; set; }
    }
}
