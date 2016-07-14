using AutoMapper;
using BookStore.Models;
using BookStore.ViewModels;
using System.Linq;

namespace BookStore.Data
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository(BookStoreContext ctx, IMapper mapper) : base(ctx, mapper)
        { }

        public IQueryable<OrderLine> GetOrderLines(int orderId)
        {
            return _ctx.OrderLines.Where(o => o.OrderId == orderId);
        }

        public IQueryable<Order> GetOrdersByUserId(string userId)
        {
            return _ctx.Orders.Where(o => o.UserId == userId);
        }

        public override void Insert(Order entity)
        {
            base.Insert(entity);
        }
    }
}