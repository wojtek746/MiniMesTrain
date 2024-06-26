﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using MiniMesTrainApi.Repository;
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
        private readonly ProcessRepository _processRepository;

        public ProcessController(MiniProductionDbContext dbContext, ProcessRepository processRepository)
        {
            _dbContext = dbContext;
            _processRepository = processRepository;
        }


        [HttpPost]
        [Route("addNew")]
        public IActionResult AddNew([FromBody] ProcessAddNew newProcessDto)
        {
            if (string.IsNullOrEmpty(newProcessDto.SerialNumber))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_processRepository.AddNew(newProcessDto))
            {
                return Ok("Process added successfully.");
            }
            else
            {
                return NotFound("Order not found.");
            }
        }

        [HttpPost]
        [Route("addParameter")]
        public IActionResult AddParameter([FromBody] ProcessAddParameter addParameter)
        {
            if (string.IsNullOrEmpty(addParameter.Value))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_processRepository.AddParameter(addParameter.ProcessId, addParameter.ParameterId, addParameter.Value))
            {
                return Ok("Parameter added to process successfully.\nNow go to selectAll");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("changeOrder")]
        public IActionResult UpdateOrder([FromBody] ProcessUpdateOrder updateOrder)
        {
            if (_processRepository.UpdateOrder(updateOrder.ProcessId, updateOrder.OrderId))
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
        public IActionResult Update([FromBody] ProcessUpdate update)
        {
            if (string.IsNullOrEmpty(update.SerialNumber))
            {
                return BadRequest("Name or description cannot be empty.");
            }

            if (_processRepository.Update(update))
            {
                return Ok("Machine added successfully.");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpDelete]
        [Route("delete/{processId}")]
        public IActionResult Delete([FromRoute] int processId)
        {
            if (_processRepository.Delete(processId))
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
            var process = _processRepository.SelectAll();

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
        [Route("selectBy")]
        public IActionResult selectBy([FromBody] ProcessSelectBy formData)
        {
            var processes = _processRepository.SelectBy(formData);

            var processWithExtraProperties = processes.Select(process => new
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
