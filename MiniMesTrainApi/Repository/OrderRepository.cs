using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;

namespace MiniMesTrainApi.Repository
{
    public interface IOrderRepository
    {
        Order GetOrderById(int orderId);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly MiniProductionDbContext _dbContext;

        public OrderRepository(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Order GetOrderById(int orderId)
        {
            return _dbContext.Orders.FirstOrDefault(m => m.Id == orderId);
        }
    }
}
