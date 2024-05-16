using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Collections.Generic;
using System.Linq;

namespace MiniMesTrainApi.Controllers
{
    [Route("product")]
    public class ProductController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;

        public ProductController(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        [Route("addNew/{name}/{description}")]
        public IActionResult AddNew([FromRoute] string name, [FromRoute] string description)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var newProduct = new Product
                {
                    Name = name,
                    Description = description
                };

                _dbContext.Products.Add(newProduct);
                _dbContext.SaveChanges();

                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding product: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("addOrder/{productId}/{orderId}")]
        public IActionResult AddOrder([FromRoute] int productId, [FromRoute] int orderId)
        {
            try
            {
                var product = _dbContext.Products.Include(m => m.Orders).FirstOrDefault(m => m.Id == productId);
                if (product == null)
                {
                    return NotFound($"Product with ID {productId} not found.");
                }

                var order = _dbContext.Orders.Find((long)orderId);
                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }

                order.ProductId = productId;
                order.Product = product;
                product.Orders.Add(order);

                _dbContext.SaveChanges();

                return Ok("Order added to product successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to product: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("change/{productId}/{name}/{description}")]
        public IActionResult Change([FromRoute] int productId, [FromRoute] string name, [FromRoute] string description)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var product = _dbContext.Products.FirstOrDefault(m => m.Id == productId);

                if (product == null)
                {
                    return NotFound();
                }
                product.Name = name;
                product.Description = description;

                _dbContext.SaveChanges();

                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding product: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("delete/{productId}")]
        public IActionResult Delete([FromRoute] int productId)
        {
            try
            {
                var product = _dbContext.Products.FirstOrDefault(m => m.Id == productId);

                if (product == null)
                {
                    return NotFound();
                }

                _dbContext.Products.Remove(product);

                _dbContext.SaveChanges();

                return Ok("deleted succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex}.");
            }
        }


        [HttpGet]
        [Route("selectAll")]
        public IActionResult SelectAll()
        {
            List<Product> products = _dbContext.Products.Include(m => m.Orders).ToList();
            return Ok(products);
        }
    }
}
