using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Process = MiniMesTrainApi.Models.Process;

namespace MiniMesTrainApi.Controllers
{
    [Route("parameter")]
    public class ParameterController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;

        public ParameterController(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        [Route("addNew/{name}/{unit}")]
        public IActionResult AddNew([FromRoute] string name, [FromRoute] string unit)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(unit))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var newParameter = new Parameter
                {
                    Name = name,
                    Unit = unit
                };

                _dbContext.Parameters.Add(newParameter);

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("addMachine/{parameterId}/{machineId}")]
        public IActionResult AddMachine([FromRoute] int parameterId, [FromRoute] int machineId)
        {
            try
            {
                var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);
                if (parameter == null)
                {
                    return NotFound($"Machine with ID {parameterId} not found.");
                }

                var machine = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == machineId);
                if (machine == null)
                {
                    return NotFound($"Order with ID {machineId} not found.");
                }

                var machineParameter = new MachineParameter
                {
                    MachineId = machineId,
                    ParameterId = parameterId
                };

                _dbContext.MachineParameter.Add(machineParameter);

                _dbContext.SaveChanges();

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("addOrder/{parameterId}/{processParameterId}")]
        public IActionResult AddOrder([FromRoute] int parameterId, [FromRoute] int processParameterId)
        {
            try
            {
                var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);
                if (parameter == null)
                {
                    return NotFound($"Machine with ID {parameterId} not found.");
                }

                var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == processParameterId);
                if (processParameter == null)
                {
                    return NotFound($"Order with ID {processParameterId} not found.");
                }

                processParameter.ParameterId = parameterId;

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("change/{parameterId}/{name}/{unit}")]
        public IActionResult Change([FromRoute] int parameterId, [FromRoute] string name, [FromRoute] string unit)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(unit))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);

                if (parameter == null)
                {
                    return NotFound();
                }

                parameter.Name = name;
                parameter.Unit = unit;

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("delete/{parameterId}")]
        public IActionResult Delete([FromRoute] int parameterId)
        {
            try
            {
                var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);

                if (parameter == null)
                {
                    return NotFound();
                }

                _dbContext.Parameters.Remove(parameter);

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
            List<Parameter> parameters = _dbContext.Parameters.Include(m => m.ProcessParameters).Include(m => m.MachineParameter).ToList();

            var parametersWithExtraProperties = parameters.Select(parameter => new
            {
                parameter.Id, 
                parameter.Name,
                parameter.Unit,
                parameter.ProcessParameters, 
                MachineParameter = parameter.MachineParameter.Select(machine => new
                {
                    Machine = _dbContext.Machines.FirstOrDefault(m =>m.Id == machine.MachineId)
                })
            }); 

            return Ok(parametersWithExtraProperties);
        }
    }
}
