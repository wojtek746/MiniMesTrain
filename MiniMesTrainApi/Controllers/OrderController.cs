using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using MiniMesTrainApi.Repository;
using System.Collections.Generic;
using System.Linq;

namespace MiniMesTrainApi.Controllers
{
    [Route("order")]
    public class OrderController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;
        private readonly OrderRepository _orderRepository;

        public OrderController(MiniProductionDbContext dbContext, OrderRepository orderRepository)
        {
            _dbContext = dbContext;
            _orderRepository = orderRepository; 
        }


        [HttpPost]
        [Route("addNew")]
        public IActionResult AddNew([FromBody] OrderAddNew addNew)
        {
            if (string.IsNullOrEmpty(addNew.Code))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_orderRepository.AddNew(addNew))
            {
                return Ok("Product added successfully.");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("changeMachine")]
        public IActionResult UpdateMachine([FromBody] OrderUpdateMachine updateMachine)
        {
            if (_orderRepository.UpdateMachine(updateMachine))
            {
                return Ok("Product added successfully.");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("changeProduct")]
        public IActionResult UpdateProduct([FromBody] OrderUpdateProduct updateProduct)
        {
            if (_orderRepository.UpdateProduct(updateProduct))
            {
                return Ok("Product added successfully.");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("change")]
        public IActionResult Update([FromBody] OrderUpdate update)
        {
            if (_orderRepository.Update(update))
            {
                return Ok("Product added successfully.");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpDelete]
        [Route("delete/{orderId}")]
        public IActionResult Delete([FromRoute] int orderId)
        {
            if (_orderRepository.Delete(orderId))
            {
                return Ok("deleted succesfully");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet]
        [Route("selectAll")]
        public IActionResult SelectAll()
        {
            var orders = _orderRepository.SelectAll(); 

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
