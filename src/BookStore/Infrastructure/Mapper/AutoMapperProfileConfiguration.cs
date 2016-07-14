using AutoMapper;
using BookStore.Models;
using BookStore.ViewModels;

namespace BookStore.Infrastructure.Mapper
{
    //http://stackoverflow.com/a/35451804/4346513
    public class AutoMapperProfileConfiguration : Profile
    {
        protected override void Configure()
        {
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorViewModel, Author>();

            CreateMap<Book, Book>()
                .ForMember(x => x.BookAuthors, opt => opt.Ignore())
                .ForMember(x => x.BookTags, opt => opt.Ignore());
            CreateMap<Book, BookViewModel>();
            CreateMap<BookViewModel, Book>();

            CreateMap<Publisher, PublisherViewModel>();
            CreateMap<PublisherViewModel, Publisher>();

            CreateMap<Tag, TagViewModel>();
            CreateMap<TagViewModel, Tag>();

            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorViewModel, Author>();

            CreateMap<Address, AddressViewModel>();
            CreateMap<AddressViewModel, Address>();

            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserViewModel, ApplicationUser>();
        }
    }
}
