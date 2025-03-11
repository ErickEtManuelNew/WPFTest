using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WPFTest.Models
{
    public enum CategoryType
    {
        Article,
        Publication
    }

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        public CategoryType Type { get; set; }

        // Navigation properties
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
} 