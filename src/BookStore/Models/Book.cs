using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; }

        public int PublisherId { get; set; }
        [IgnoreMap]
        public Publisher Publisher { get; set; }

        public ICollection<BookTag> BookTags { get; set; }

        [MaxLength(128)]
        public string UpTitle { get; set; }

        [MaxLength(128)]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [MaxLength(128)]
        public string SubTitle { get; set; }

        [MaxLength(386)]
        public string FullTitle
        {
            get
            {
                return (UpTitle + ' ' + Title + ' ' + SubTitle).Trim();
            }
            set { }
        }

        [Required(ErrorMessage = "Code ISBN is required")]
        public string Isbn { get; set; }

        public int Pages { get; set; }

        public string Contents { get; set; }

        public string ShortDesc { get; set; }

        public string FullDesc { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal EbookPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrintPrice { get; set; }

        public DateTime? PublicationDate { get; set; }

        public bool IsVisible { get; set; }

        public string ImgCoverUrl { get; set; }

    }
}