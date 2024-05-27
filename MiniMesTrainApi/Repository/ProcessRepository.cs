using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniMesTrainApi.FromBodys;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using MiniMesTrainApi.Repository;
using System.Data.Common;
using System.Globalization;

public class ProcessRepository : IRepository<ProcessUpdate, Process, ProcessAddNew>
{
    private readonly MiniProductionDbContext _dbContext;
    private readonly ILogger<ProcessRepository> _logger;

    public ProcessRepository(MiniProductionDbContext dbContext, ILogger<ProcessRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public List<Process> SelectBy(ProcessSelectBy formData)
    {
        DateTime fromDate = DateTime.MinValue;
        DateTime toDate = DateTime.MaxValue;

        try
        {
            fromDate = DateTime.ParseExact(formData.DateTimeFrom, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            toDate = DateTime.ParseExact(formData.DateTimeTo, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            _logger.LogInformation($"error with parse text to date while seeing Process"); 
        }

        var query = _dbContext.Processes.Where(m => m.DateTime >= fromDate).Where(m => m.DateTime <= toDate).AsQueryable();
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

        _logger.LogInformation($"Saw Process with ");

        return query.ToList();
    }

    public List<Process> SelectAll()
    {
        _logger.LogInformation($"Saw all Process");
        return _dbContext.Processes.Include(m => m.Order).Include(m => m.ProcessParameters).ToList();
    }

    //do zmiany w środę, teraz (w poniedziałek po 15) zauważyłem, że tego nie zmieniłem po skopiowaniu z ProcessParameterRepository
    public bool AddParameter(int processId, int parameterId, string value)
    {
        var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);
        var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);

        if (process == null || parameter == null)
        {
            return false; 
        }

        var newProcessParameter = new ProcessParameter
        {
            ProcessId = processId,
            ParameterId = parameterId,
            Value = value
        };

        _dbContext.ProcessParameters.Add(newProcessParameter);

        _dbContext.SaveChanges();

        return true; 
    }

    public bool AddNew(ProcessAddNew addNew)
    {

        var order = _dbContext.Orders.FirstOrDefault(m => m.Id == addNew.OrderId);

        if (order == null)
        {
            return false; 
        }

        var newProcess = new Process
        {
            SerialNumber = addNew.SerialNumber,
            OrderId = addNew.OrderId,
            DateTime = DateTime.Now
        };

        _dbContext.Processes.Add(newProcess);
        _dbContext.SaveChanges();

        return true; 
    }

    public bool UpdateOrder(int processId, int orderId)
    {
        var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);
        var order = _dbContext.Orders.FirstOrDefault(m => m.Id == orderId);

        if (order == null || process == null)
        {
            return false; 
        }

        process.OrderId = orderId;

        _dbContext.SaveChanges();
        return true; 
    }

    public bool Update(ProcessUpdate update)
    {
        var process = _dbContext.Processes.FirstOrDefault(m => m.Id == update.ProcessId);
        var order = _dbContext.Orders.FirstOrDefault(m => m.Id == update.OrderId);

        if (order == null || process == null)
        {
            return false; 
        }

        process.SerialNumber = update.SerialNumber;
        process.OrderId = update.OrderId;

        _dbContext.SaveChanges();
        return true; 
    }

    public bool Delete(int processId)
    {
        var process = _dbContext.Processes.FirstOrDefault(m => m.Id == processId);

        if (process == null)
        {
            return false; 
        }

        _dbContext.Processes.Remove(process);

        _dbContext.SaveChanges();

        return true; 
    }
}
