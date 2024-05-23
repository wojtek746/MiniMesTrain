using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using MiniMesTrainApi.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Process = MiniMesTrainApi.Models.Process;

namespace MiniMesTrainApi.Controllers
{
    [Route("processParameter")]
    public class ProcessParameterController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;
        private readonly ProcessParameterRepository _processParameterRepository;

        public ProcessParameterController(MiniProductionDbContext dbContext, ProcessParameterRepository processParameterRepository)
        {
            _dbContext = dbContext;
            _processParameterRepository = processParameterRepository; 
        }


        [HttpPost]
        [Route("addNew")]
        public IActionResult AddNew([FromBody] ProcessParameterAddNew addNew)
        {
            if (string.IsNullOrEmpty(addNew.Value))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_processParameterRepository.AddNew(addNew))
            {
                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("changeProcess")]
        public IActionResult UpdateProcess([FromBody] ProcessParameterUpdateProcess updateProcess)
        {
            if (_processParameterRepository.UpdateProcess(updateProcess))
            {
                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("changeParameter")]
        public IActionResult UpdateParameter([FromBody] ProcessParameterUpdateParameter updateParameter)
        {
            if (_processParameterRepository.UpdateParameter(updateParameter))
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
        public IActionResult Update([FromBody] ProcessParameterUpdate update)
        {
            if (string.IsNullOrEmpty(update.Value))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_processParameterRepository.Update(update))
            {
                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpDelete]
        [Route("delete/{processParameterId}")]
        public IActionResult Delete([FromRoute] int processParameterId)
        {
            if (_processParameterRepository.Delete(processParameterId))
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
            List<ProcessParameter> processParameter = _processParameterRepository.SelectAll();

            var processParameterWithExtraProperties = processParameter.Select(processParameter => new
            {
                processParameter.Id,
                processParameter.ProcessId,
                processParameter.ParameterId,
                processParameter.Value,
                Process = _dbContext.Processes.FirstOrDefault(m => m.Id == processParameter.ProcessId),
                Parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == processParameter.ParameterId)
            });
            return Ok(processParameterWithExtraProperties);
        }
    }
}
