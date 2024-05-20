using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Data.Common;
using System.Globalization;

public class ProcessRepository : IRepository
{
    private readonly MiniProductionDbContext _dbContext;

    public ProcessRepository(MiniProductionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Process> SelectBy(SelectBy formData)
    {
        DateTime fromDate = DateTime.MinValue;
        DateTime toDate = DateTime.MaxValue;

        try
        {
            fromDate = DateTime.ParseExact(formData.DateTimeFrom, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
        catch (FormatException) { }

        try
        {
            toDate = DateTime.ParseExact(formData.DateTimeTo, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
        catch (FormatException) { }

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

        return query.ToList();
    }

    public List<Process> SelectAll()
    {
        return _dbContext.Processes.Include(m => m.Order).Include(m => m.ProcessParameters).ToList();
    }

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

    public bool AddNew(string serialNumber, int orderId)
    {

        var order = _dbContext.Orders.FirstOrDefault(m => m.Id == orderId);

        if (order == null)
        {
            return false; 
        }

        var newProcess = new Process
        {
            SerialNumber = serialNumber,
            OrderId = orderId,
            DateTime = DateTime.Now
        };

        _dbContext.Processes.Add(newProcess);
        _dbContext.SaveChanges();

        return true; 
    }

    public bool ChangeOrder(int processId, int orderId)
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

    public bool Update(Change change)
    {
        var process = _dbContext.Processes.FirstOrDefault(m => m.Id == change.ProcessId);
        var order = _dbContext.Orders.FirstOrDefault(m => m.Id == change.OrderId);

        if (order == null || process == null)
        {
            return false; 
        }

        process.SerialNumber = change.SerialNumber;
        process.OrderId = change.OrderId;

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
