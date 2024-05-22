using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.Migrations;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using MiniMesTrainApi.Repository;
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
        private readonly ParameterRepository _parameterRepository;

        public ParameterController(MiniProductionDbContext dbContext, ParameterRepository parameterRepository)
        {
            _dbContext = dbContext;
            _parameterRepository = parameterRepository; 
        }


        [HttpPost]
        [Route("addNew")]
        public IActionResult AddNew([FromBody] ParameterAddNew addNew)
        {
            if (string.IsNullOrEmpty(addNew.Name) || string.IsNullOrEmpty(addNew.Unit))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            _parameterRepository.AddNew(addNew);
            return Ok("Order added to machine successfully.\nNow go to selectAll");
        }

        [HttpPost]
        [Route("addMachine")]
        public IActionResult AddMachine([FromBody] ParameterAddMachine addMachine)
        {
            if (_parameterRepository.AddMachine(addMachine))
            {
                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("addOrder")]
        public IActionResult AddOrder([FromBody] ParameterAddOrder addOrder)
        {
            if (_parameterRepository.AddOrder(addOrder))
            {
                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound(); 
            }
        }


        [HttpPost]
        [Route("change")]
        public IActionResult Update([FromBody] ParameterUpdate update)
        {
            if (string.IsNullOrEmpty(update.Name) || string.IsNullOrEmpty(update.Unit))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_parameterRepository.Update(update))
            {
                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpDelete]
        [Route("delete/{parameterId}")]
        public IActionResult Delete([FromRoute] int parameterId)
        {
            if (_parameterRepository.Delete(parameterId))
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
            List<Parameter> parameters = _parameterRepository.SelectAll();

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
