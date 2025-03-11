using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WPFTest.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
        public ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
} 