using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.html;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Globalization;

public interface IProcessRepository
{
    List<Process> SelectBy(ProcessSelectBy formData);
    List<Process> SelectAll();
}

public class ProcessRepository : IProcessRepository
{
    private readonly MiniProductionDbContext _dbContext;

    public ProcessRepository(MiniProductionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Process> SelectBy(ProcessSelectBy formData)
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
}
