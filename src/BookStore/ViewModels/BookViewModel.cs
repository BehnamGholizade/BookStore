using AutoMapper;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; }

        [Required(ErrorMessage = "Publisher is required")]
        public int PublisherId { get; set; }

        [IgnoreMap]
        public Publisher Publisher { get; set; }

        [MaxLength(128)]
        public string UpTitle { get; set; }

        [MaxLength(128)]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [MaxLength(128)]
        public string SubTitle { get; set; }

        public string FullTitle
        {
            get { return (UpTitle + ' ' + Title + ' ' + SubTitle).Trim(); }
        }

        [Required(ErrorMessage = "Code ISBN is required")]
        public string Isbn { get; set; }

        [Required(ErrorMessage = "Pages is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Pages must be greater than zero.")]
        public int Pages { get; set; }

        public string Contents { get; set; }

        public string ShortDesc { get; set; }

        public string FullDesc { get; set; }

        public decimal? EbookPrice { get; set; }

        public decimal? PrintPrice { get; set; }

        [Required(ErrorMessage = "Publication Date is required")]
        public DateTime PublicationDate { get; set; }

        public string ImgCoverUrl { get; set; }

        public int[] SelectedBookAuthorIds { get; set; } = new int[] { };
    }
}
