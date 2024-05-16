using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using Machine = MiniMesTrainApi.Models.Machine;

namespace MiniMesTrainApi.Controllers
{
    [Route("machine")]
    public class MachineController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;

        public MachineController(MiniProductionDbContext dbContext)
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
                var newMachine = new Machine
                {
                    Name = name,
                    Description = description
                };

                _dbContext.Machines.Add(newMachine);

                _dbContext.SaveChanges();

                return Ok("Machine added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("addOrder/{machineId}/{orderId}")]
        public IActionResult AddOrder([FromRoute] int machineId, [FromRoute] int orderId)
        {
            try
            {
                var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == machineId);
                if (machine == null)
                {
                    return NotFound($"Machine with ID {machineId} not found.");
                }

                var order = _dbContext.Orders.Find((long)orderId);
                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }

                order.MachineId = machineId;
                order.Machine = machine;
                machine.Orders.Add(order);

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("addParameter/{machineId}/{parameterId}")]
        public IActionResult AddParameter([FromRoute] int machineId, [FromRoute] int parameterId)
        {
            try
            {
                var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == machineId);
                if (machine == null)
                {
                    return NotFound($"Machine with ID {machineId} not found.");
                }

                var parameter = _dbContext.Parameters.Find(parameterId);
                if (parameter == null)
                {
                    return NotFound($"Order with ID {parameterId} not found.");
                }

                var machineParameter = new MachineParameter
                {
                    MachineId = machineId,
                    ParameterId = parameterId
                }; 

                _dbContext.MachineParameter.Add(machineParameter);

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("change/{machineId}/{name}/{description}")]
        public IActionResult Change([FromRoute] int machineId, [FromRoute] string name, [FromRoute] string description)
        {

            try
            {
                var machine = _dbContext.Machines.FirstOrDefault(m => m.Id == machineId);
                if (machine == null)
                {
                    return NotFound($"Machine with ID {machineId} not found.");
                }

                machine.Name = name;
                machine.Description = description;

                _dbContext.SaveChanges();

                return Ok("Change succesfully"); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("delete/{machineId}")]
        public IActionResult Delete([FromRoute] int machineId)
        {
            try
            {
                var machine = _dbContext.Machines.FirstOrDefault(m => m.Id == machineId);

                if (machine == null)
                {
                    return NotFound();
                }

                _dbContext.Machines.Remove(machine);

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
            List<Machine> machines = _dbContext.Machines.Include(m => m.Orders).Include(m => m.MachineParameter).ToList();

            var machinesWithExtraProperties = machines.Select(machine => new
            {
                machine.Id,
                machine.Name,
                machine.Description,
                machine.Orders,
                MachineParameter = machine.MachineParameter.Select(machineParameter => new
                {
                    Parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == machineParameter.ParameterId)
                })
            }); 

            return Ok(machinesWithExtraProperties);
        }
    }
}
