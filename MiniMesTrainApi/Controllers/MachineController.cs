using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using MiniMesTrainApi.Repository;
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
        private readonly MachineRepository _machineRepository;

        public MachineController(MiniProductionDbContext dbContext, MachineRepository machineRepository)
        {
            _dbContext = dbContext;
            _machineRepository = machineRepository; 
        }


        [HttpPost]
        [Route("addNew")]
        public IActionResult AddNew([FromBody] MachineAddNew addNew)
        {
            if (string.IsNullOrEmpty(addNew.Name) || string.IsNullOrEmpty(addNew.Description))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            _machineRepository.AddNew(addNew);
            return Ok("Machine added successfully.");
        }

        [HttpPost]
        [Route("addOrder")]
        public IActionResult AddOrder([FromBody] MachineAddOrder addOrder)
        {
            _machineRepository.AddOrder(addOrder);

            return Ok("Order added to machine successfully.\nNow go to selectAll");
        }

        [HttpPost]
        [Route("addParameter")]
        public IActionResult AddParameter([FromBody] MachineAddParameter addParameter)
        {
            if (_machineRepository.AddParameter(addParameter))
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
        public IActionResult Update([FromBody] MachineUpdate update)
        {
            if (_machineRepository.Update(update))
            {
                return Ok("Change succesfully");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("delete/{machineId}")]
        public IActionResult Delete([FromRoute] int machineId)
        {
            if (_machineRepository.Delete(machineId))
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
            List<Machine> machines = _machineRepository.SelectAll();

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
