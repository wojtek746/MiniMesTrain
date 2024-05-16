using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Collections.Generic;
using System.Linq;

namespace MiniMesTrainApi.Controllers
{
    [Route("order")]
    public class OrderController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;

        public OrderController(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        [Route("addNew/{code}/{machineId}/{productId}/{quantity}")]
        public IActionResult AddNew([FromRoute] string code, [FromRoute] int machineId, [FromRoute] int productId, [FromRoute] int quantity)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == machineId);
                var product = _dbContext.Products.Include(m => m.Orders).FirstOrDefault(m => m.Id == productId);

                if (machine == null || product == null)
                {
                    return NotFound();
                }

                var newOrder = new Order
                {
                    Code = code,
                    MachineId = machineId,
                    ProductId = productId,
                    Quantity = quantity
                };

                _dbContext.Orders.Add(newOrder);

                _dbContext.SaveChanges();

                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding product: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("changeMachine/{orderId}/{machineId}")]
        public IActionResult ChangeMachine([FromRoute] int orderId, [FromRoute] int machineId)
        {
            try
            {
                var order = _dbContext.Orders.Include(m => m.Product).Include(m => m.Machine).FirstOrDefault(m => m.Id == orderId);
                var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == machineId);

                if (order == null || machine == null)
                {
                    return NotFound();
                }

                order.MachineId = machineId;
                order.Machine = machine;

                _dbContext.SaveChanges();

                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding product: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("changeProduct/{orderId}/{productId}")]
        public IActionResult ChangeProduct([FromRoute] int orderId, [FromRoute] int productId)
        {
            try
            {
                var order = _dbContext.Orders.Include(m => m.Product).Include(m => m.Machine).FirstOrDefault(m => m.Id == orderId);
                var product = _dbContext.Products.Include(m => m.Orders).FirstOrDefault(m => m.Id == productId);
                if (order == null || product == null)
                {
                    return NotFound();
                }

                order.ProductId = productId;

                _dbContext.SaveChanges();

                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding product: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("change/{orderId}/{code}/{machineId}/{productId}/{quantity}")]
        public IActionResult Change([FromRoute] int orderId, [FromRoute] string code, [FromRoute] int machineId, [FromRoute] int productId, [FromRoute] int quantity)
        {
            try
            {
                var order = _dbContext.Orders.Include(m => m.Product).Include(m => m.Machine).FirstOrDefault(m => m.Id == orderId);
                var product = _dbContext.Products.FirstOrDefault(m => m.Id == productId);
                var machine = _dbContext.Machines.FirstOrDefault(m => m.Id == machineId);
                if (order == null || product == null || machine == null)
                {
                    return NotFound();
                }

                order.Code = code;
                order.MachineId = machineId;
                order.ProductId = productId;
                order.Quantity = quantity;

                _dbContext.SaveChanges();

                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding product: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("delete/{orderId}")]
        public IActionResult Delete([FromRoute] int orderId)
        {
            try
            {
                var order = _dbContext.Orders.FirstOrDefault(m => m.Id == orderId);

                if (order == null)
                {
                    return NotFound();
                }

                _dbContext.Orders.Remove(order);

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
            List<Order> orders = _dbContext.Orders.Include(o => o.Machine).Include(o => o.Product).ToList();

            var ordersWithExtraProperties = orders.Select(order => new
            {
                order.Id,
                order.Code,
                order.MachineId,
                order.ProductId,
                order.Quantity,
                Machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == order.MachineId),
                Product = _dbContext.Products.Include(m => m.Orders).FirstOrDefault(m => m.Id == order.ProductId)
            });

            return Ok(ordersWithExtraProperties);
        }
    }
}
