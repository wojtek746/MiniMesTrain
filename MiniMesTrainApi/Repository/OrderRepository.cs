using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniMesTrainApi.Repository
{
    public class OrderRepository : IRepository<OrderUpdate, Order, OrderAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;
        private readonly ILogger<MachineRepository> _logger;

        public OrderRepository(MiniProductionDbContext dbContext, ILogger<MachineRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger; 
        }

        public List<Order> SelectAll()
        {
            _logger.LogInformation($"Saw all Orders");
            return _dbContext.Orders.Include(o => o.Machine).Include(o => o.Product).ToList();
        }

        public bool AddNew(OrderAddNew addNew)
        {
            var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == addNew.MachineId);
            var product = _dbContext.Products.Include(m => m.Orders).FirstOrDefault(m => m.Id == addNew.ProductId);

            if (machine == null || product == null)
            {
                if (machine == null)
                {
                    _logger.LogError($"Not found Machine with Id {addNew.MachineId}");
                }
                else
                {
                    _logger.LogError($"Not found Product with Id {addNew.ProductId}");
                }
                return false; 
            }

            var newOrder = new Order
            {
                Code = addNew.Code,
                MachineId = addNew.MachineId,
                ProductId = addNew.ProductId,
                Quantity = addNew.Quantity
            };

            _dbContext.Orders.Add(newOrder);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully added Order on Id {newOrder.Id} with MachineId: {newOrder.MachineId} and ProductId: {newOrder.ProductId} and Quantity: {newOrder.Quantity}");

            return true; 
        }

        public bool UpdateMachine(OrderUpdateMachine updateMachine)
        {
            var order = _dbContext.Orders.Include(m => m.Product).Include(m => m.Machine).FirstOrDefault(m => m.Id == updateMachine.OrderId);
            var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == updateMachine.MachineId);

            if (order == null || machine == null)
            {
                if (machine == null)
                {
                    _logger.LogError($"Not found Machine with Id {updateMachine.MachineId}");
                }
                else
                {
                    _logger.LogError($"Not found Order with Id {updateMachine.OrderId}");
                }
                return false;
            }

            var last = new
            {
                MachineId = order.MachineId
            }; 

            order.MachineId = updateMachine.MachineId;
            order.Machine = machine;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully update Machine from Id {last.MachineId} for {order.MachineId} in Order on Id {order.Id}");

            return true;
        }

        public bool UpdateProduct(OrderUpdateProduct updateProduct)
        {
            var order = _dbContext.Orders.Include(m => m.Product).Include(m => m.Machine).FirstOrDefault(m => m.Id == updateProduct.OrderId);
            var product = _dbContext.Products.Include(m => m.Orders).FirstOrDefault(m => m.Id == updateProduct.ProductId);
            if (order == null || product == null)
            {
                return false;
            }

            order.ProductId = updateProduct.ProductId;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Update(OrderUpdate update)
        {
            var order = _dbContext.Orders.Include(m => m.Product).Include(m => m.Machine).FirstOrDefault(m => m.Id == update.OrderId);
            var product = _dbContext.Products.FirstOrDefault(m => m.Id == update.ProductId);
            var machine = _dbContext.Machines.FirstOrDefault(m => m.Id == update.MachineId);
            if (order == null || product == null || machine == null)
            {
                return false;
            }

            order.Code = update.Code;
            order.MachineId = update.MachineId;
            order.ProductId = update.ProductId;
            order.Quantity = update.Quantity;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Delete(int orderId)
        {
            var order = _dbContext.Orders.FirstOrDefault(m => m.Id == orderId);

            if (order == null)
            {
                return false;
            }

            _dbContext.Orders.Remove(order);

            _dbContext.SaveChanges();

            return true;
        }
    }
}
