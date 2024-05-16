using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
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

        public ProcessParameterController(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        [Route("addNew/{processId}/{parameterId}/{value}")]
        public IActionResult AddNew([FromRoute] int processId, [FromRoute] int parameterId, [FromRoute] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);
                var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);

                if (process == null || parameter == null)
                {
                    return NotFound();
                }

                var newProcessParameter = new ProcessParameter
                {
                    ProcessId = processId,
                    ParameterId = parameterId,
                    Value = value,

                };

                _dbContext.ProcessParameters.Add(newProcessParameter);

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("changeProcess/{processParameterId}/{processId}")]
        public IActionResult ChangeOrder([FromRoute] int processParameterId, [FromRoute] int processId)
        {
            try
            {
                var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);
                var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == processParameterId);

                if (processParameter == null || process == null)
                {
                    return NotFound();
                }

                processParameter.ProcessId = processId;

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("changeParameter/{processParameterId}/{parameterId}")]
        public IActionResult ChangeParameter([FromRoute] int processParameterId, [FromRoute] int parameterId)
        {
            try
            {
                var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);
                var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == processParameterId);

                if (processParameter == null || parameter == null)
                {
                    return NotFound();
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
        [Route("change/{processParameterId}/{processId}/{parameterId}/{value}")]
        public IActionResult Change([FromRoute] int processParameterId, [FromRoute] int processId, [FromRoute] int parameterId, [FromRoute] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == processParameterId); 
                var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);
                var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);

                if (process == null || parameter == null || processParameter == null)
                {
                    return NotFound();
                }
                processParameter.ProcessId = processId;
                processParameter.ParameterId = parameterId;
                processParameter.Value = value;

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("delete/{processParameterId}")]
        public IActionResult Delete([FromRoute] int processParameterId)
        {
            try
            {
                var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == processParameterId);

                if (processParameter == null)
                {
                    return NotFound();
                }

                _dbContext.ProcessParameters.Remove(processParameter);

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
            List<ProcessParameter> processParameter = _dbContext.ProcessParameters.Include(m => m.Process).Include(m => m.Parameter).ToList();

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
