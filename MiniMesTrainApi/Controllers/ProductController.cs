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
    [Route("product")]
    public class ProductController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;
        private readonly ProductRepository _productRepository;

        public ProductController(MiniProductionDbContext dbContext, ProductRepository productRepository)
        {
            _dbContext = dbContext;
            _productRepository = productRepository; 
        }


        [HttpPost]
        [Route("addNew")]
        public IActionResult AddNew([FromBody] ProductAddNew addNew)
        {
            if (string.IsNullOrEmpty(addNew.Name) || string.IsNullOrEmpty(addNew.Description))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            _productRepository.AddNew(addNew);

            return Ok("Product added successfully.");
        }

        [HttpPost]
        [Route("addOrder")]
        public IActionResult AddOrder([FromBody] ProductAddOrder addOrder)
        {

            if (_productRepository.AddOrder(addOrder))
            {
                return Ok("Order added to product successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("change")]
        public IActionResult Update([FromBody] ProductUpdate update)
        {
            if (string.IsNullOrEmpty(update.Name) || string.IsNullOrEmpty(update.Description))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_productRepository.Update(update))
            {
                return Ok("Product changed successfully.");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpDelete]
        [Route("delete/{productId}")]
        public IActionResult Delete([FromRoute] int productId)
        {
            if (_productRepository.Delete(productId))
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
            List<Product> products = _productRepository.SelectAll();
            return Ok(products);
        }
    }
}
