using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using Process = MiniMesTrainApi.Models.Process;

namespace MiniMesTrainApi.Controllers
{
    [Route("process")]
    public class ProcessController : Controller
    {
        private readonly MiniProductionDbContext _dbContext;

        public ProcessController(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        [Route("addNew/{serialNumber}/{orderId}")]
        public IActionResult AddNew([FromRoute] string serialNumber, [FromRoute] int orderId)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var order = _dbContext.Orders.FirstOrDefault(m => m.Id == orderId); 

                if (order == null)
                {
                    return NotFound();
                }

                var newProcess = new Process
                {
                    SerialNumber = serialNumber,
                    OrderId = orderId,
                    DateTime = DateTime.Now
                };

                _dbContext.Processes.Add(newProcess);
                _dbContext.SaveChanges();

                return Ok("Machine added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding machine: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("addParameter/{processId}/{parameterId}/{value}")]
        public IActionResult AddParameter([FromRoute] int processId, [FromRoute] int parameterId, [FromRoute] string value)
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
        [Route("changeOrder/{processId}/{orderId}")]
        public IActionResult ChangeOrder([FromRoute] int processId, [FromRoute] int orderId)
        {
            try
            {
                var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);
                var order = _dbContext.Orders.FirstOrDefault(m => m.Id == orderId);

                if (order == null || process == null)
                {
                    return NotFound();
                }

                process.OrderId = orderId;

                _dbContext.SaveChanges();

                return Ok("Order added to machine successfully.\nNow go to selectAll");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding order to machine: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("change/{processId}/{serialNumber}/{orderId}")]
        public IActionResult Change([FromRoute] int processId, [FromRoute] string serialNumber, [FromRoute] int orderId)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            try
            {
                var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId); 
                var order = _dbContext.Orders.FirstOrDefault(m => m.Id == orderId);

                if (order == null || process == null)
                {
                    return NotFound();
                }

                process.SerialNumber = serialNumber;
                process.OrderId = orderId;

                _dbContext.SaveChanges();

                return Ok("Machine added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding machine: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("delete/{processId}")]
        public IActionResult Delete([FromRoute] int processId)
        {
            try
            {
                var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);

                if (process == null)
                {
                    return NotFound();
                }

                _dbContext.Processes.Remove(process);

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
            List<Process> process = _dbContext.Processes.Include(m => m.Order).Include(m => m.ProcessParameters).ToList();

            var processWithExtraProperties = process.Select(process => new
            {
                process.Id,
                process.SerialNumber,
                process.OrderId,
                process.Status,
                DateTime = process.DateTime.ToString("dddd, dd MMMM yyyy HH: mm:ss"),
                Order = _dbContext.Orders.FirstOrDefault(m => m.Id == process.OrderId),
                process.ProcessParameters
            });
            return Ok(processWithExtraProperties);
        }

        [HttpPost]
        [Route("selectBy/{processId}/{serialNumber}/{orderId}/{orderCode}/{machineId}/{productId}/{quantity}/{processParameterId}/{ParameterId}/{processParameterValue}/{DateTimeFrom}/{DateTimeTo}")]
        public IActionResult selectBy([FromRoute] int processId, [FromRoute] string serialNumber, 
            [FromRoute] int orderId, [FromRoute] string orderCode, [FromRoute] int machineId, [FromRoute] int productId, [FromRoute] int quantity,
            [FromRoute] int processParameterId, [FromRoute] int ParameterId, [FromRoute] string processParameterValue,
            [FromRoute] string DateTimeFrom, [FromRoute] string DateTimeTo)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (DateTimeFrom != "null")
            {
                DateTimeFrom = HttpUtility.UrlDecode(DateTimeFrom);
                fromDate = DateTime.ParseExact(DateTimeFrom, "MM/dd/yyyy", CultureInfo.InvariantCulture); //05%2F14%2F2024
            }
            if (DateTimeTo != "null")
            {
                DateTimeTo = HttpUtility.UrlDecode(DateTimeTo);
                toDate = DateTime.ParseExact(DateTimeTo, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            List<Process> process = _dbContext.Processes
                .Include(m => m.Order)
                .Include(m => m.ProcessParameters)
                .Where(m => m.Id == processId || processId <= 0)
                .Where(m => m.SerialNumber == serialNumber || serialNumber == "null")
                .Where (m => m.OrderId == orderId || orderId <= 0)
                .Where(m => m.Order.Code.Contains(orderCode) || orderCode == "null")
                .Where(m => m.Order.MachineId == machineId || machineId <= 0)
                .Where(m => m.Order.ProductId == productId || productId <= 0)
                .Where(m => m.Order.Quantity == quantity || quantity <= 0)
                .Where(m => m.ProcessParameters.Any(p => p.Id == processParameterId) || processParameterId <= 0)
                .Where(m => m.ProcessParameters.Any(p => p.ParameterId == ParameterId) || ParameterId <= 0)
                .Where(m => m.ProcessParameters.Any(p => p.Value.Contains(processParameterValue)) || processParameterValue == "null")
                .Where(m => m.DateTime >= fromDate || DateTimeFrom == "null")
                .Where(m => m.DateTime <= toDate || DateTimeTo == "null")
                .ToList();

            var processWithExtraProperties = process.Select(process => new
            {
                process.Id,
                process.SerialNumber,
                process.OrderId,
                process.Status,
                DateTime = process.DateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                process.Order,
                process.ProcessParameters
            });
            return Ok(processWithExtraProperties);
        }
    }
}
