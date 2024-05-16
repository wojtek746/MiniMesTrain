using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.html;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using Newtonsoft.Json.Linq;
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

        /*[HttpPost]
        [Route("selectBy")]
        public IActionResult selectBy([FromBody] ProcessSelectBy formData)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            try
            {
                //DateTimeFrom = HttpUtility.UrlDecode(DateTimeFrom);
                fromDate = DateTime.ParseExact(formData.DateTimeFrom, "MM/dd/yyyy", CultureInfo.InvariantCulture); //05%2F14%2F2024
            }
            catch (Exception ex) { }
            try
            {
                //DateTimeTo = HttpUtility.UrlDecode(DateTimeTo);
                toDate = DateTime.ParseExact(formData.DateTimeTo, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception ex) { }

            List<Process> process = _dbContext.Processes
                .Include(m => m.Order)
                .Include(m => m.ProcessParameters)
                .Where(m => m.Id == formData.ProcessId || formData.ProcessId <= 0)
                .Where(m => m.SerialNumber == formData.SerialNumber || formData.SerialNumber == "null")
                .Where(m => m.OrderId == formData.OrderId || formData.OrderId <= 0)
                .Where(m => m.Order.Code.Contains(formData.OrderCode) || formData.OrderCode == "null")
                .Where(m => m.Order.MachineId == formData.MachineId || formData.MachineId <= 0)
                .Where(m => m.Order.ProductId == formData.ProductId || formData.ProductId <= 0)
                .Where(m => m.Order.Quantity == formData.Quantity || formData.Quantity <= 0)
                .Where(m => m.ProcessParameters.Any(p => p.Id == formData.ProcessParameterId) || formData.ProcessParameterId <= 0)
                .Where(m => m.ProcessParameters.Any(p => p.ParameterId == formData.ParameterId) || formData.ParameterId <= 0)
                .Where(m => m.ProcessParameters.Any(p => p.Value.Contains(formData.ProcessParameterValue)) || formData.ProcessParameterValue == "null")
                .Where(m => m.DateTime >= fromDate)
                .Where(m => m.DateTime <= toDate)
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
        }*/

        [HttpPost]
        [Route("selectBy")]
        public IActionResult selectBy([FromBody] ProcessSelectBy formData)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            try
            {
                fromDate = DateTime.ParseExact(formData.DateTimeFrom, "MM/dd/yyyy", CultureInfo.InvariantCulture); 
            } catch (FormatException ex){}
            try
            {
                toDate = DateTime.ParseExact(formData.DateTimeTo, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            } catch (FormatException ex){}

            var process = new List<Process>();

            var optionsBuilder = new DbContextOptionsBuilder<MiniProductionDbContext>();
            optionsBuilder.UseSqlServer("Data Source = localhost\\SQLEXPRESS02; Initial Catalog = MiniProductionTrainDb; Persist Security Info=True; Integrated Security=SSPI; Connection Timeout = 15; TrustServerCertificate=True; ");

            using (var dbContext = new MiniProductionDbContext(optionsBuilder.Options))
            {
                var query = dbContext.Processes.Where(m => m.DateTime >= fromDate).Where(m => m.DateTime <= toDate).AsQueryable();
                if (formData.ProcessId > 0) query = query.Where(m => m.Id == formData.ProcessId);
                if (formData.SerialNumber != "null") query = query.Where(m => m.SerialNumber == formData.SerialNumber);
                if (formData.OrderId > 0) query = query.Where(m => m.OrderId == formData.OrderId);
                query = query.Include(m => m.Order);
                if (formData.OrderCode != "null") query = query.Where(m => m.Order.Code.Contains(formData.OrderCode));
                if (formData.MachineId > 0) query = query.Where(m => m.Order.MachineId == formData.MachineId);
                if (formData.ProductId > 0) query = query.Where(m => m.Order.ProductId == formData.ProductId);
                if (formData.Quantity > 0) query = query.Where(m => m.Order.Quantity == formData.Quantity);
                query = query.Include(m => m.ProcessParameters);
                if (formData.ProcessParameterId > 0) query = query.Where(m => m.ProcessParameters.Any(p => p.Id == formData.ProcessParameterId));
                if (formData.ProcessParameterValue != "null") query = query.Where(m => m.ProcessParameters.Any(p => p.Value.Contains(formData.ProcessParameterValue)));
                process = query.ToList();
            }

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
