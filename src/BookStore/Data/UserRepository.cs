using AutoMapper;
using BookStore.Models;

namespace BookStore.Data
{
    public class UserRepository : GenericRepository<ApplicationUser>
    {
        public UserRepository(BookStoreContext ctx, IMapper mapper) : base(ctx, mapper)
        { }
    }
}