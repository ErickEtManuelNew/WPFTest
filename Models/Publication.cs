using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WPFTest.Models
{
    public class Publication
    {
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        [Required]
        public required string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? PublishedAt { get; set; }
        public PublicationStatus Status { get; set; }
        public int AuthorId { get; set; }
        public required UserAccount Author { get; set; }
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }

    public enum PublicationStatus
    {
        Draft = 0,
        UnderReview = 1,
        Published = 2,
        Archived = 3
    }
} 