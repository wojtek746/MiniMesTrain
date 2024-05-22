using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniMesTrainApi.Repository
{
    public class ProductRepository : IRepository<ProductUpdate, Product, ProductAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;

        public ProductRepository(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Product> SelectAll()
        {
            return _dbContext.Products.Include(m => m.Orders).ToList();
        }

        public bool AddNew(ProductAddNew addNew)
        {
            var newProduct = new Product
            {
                Name = addNew.Name,
                Description = addNew.Description
            };

            _dbContext.Products.Add(newProduct);
            _dbContext.SaveChanges();

            return true;
        }

        public bool AddOrder(ProductAddOrder addOrder)
        {
            var product = _dbContext.Products.Include(m => m.Orders).FirstOrDefault(m => m.Id == addOrder.ProductId);
            var order = _dbContext.Orders.Find((long)addOrder.OrderId);

            if (order == null || product == null)
            {
                return false;
            }

            order.ProductId = addOrder.ProductId;

            _dbContext.SaveChanges();
            return true;
        }

        public bool Update(ProductUpdate update)
        {
            var product = _dbContext.Products.FirstOrDefault(m => m.Id == update.ProductId);

            if (product == null)
            {
                return false;
            }
            product.Name = update.Name;
            product.Description = update.Description;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Delete(int ProductId)
        {
            var Product = _dbContext.Products.FirstOrDefault(m => m.Id == ProductId);

            if (Product == null)
            {
                return false;
            }

            _dbContext.Products.Remove(Product);

            _dbContext.SaveChanges();

            return true;
        }
    }
}
