using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniMesTrainApi.Repository
{
    public class ProcessParameterRepository : IRepository<ProcessParameterUpdate, ProcessParameter, ProcessParameterAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;

        public ProcessParameterRepository(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProcessParameter> SelectAll()
        {
            return _dbContext.ProcessParameters.Include(m => m.Process).Include(m => m.Parameter).ToList();
        }

        public bool AddNew(ProcessParameterAddNew addNew)
        {
            var process = _dbContext.Processes.FirstOrDefault(m => m.Id == addNew.ProcessId);
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == addNew.ParameterId);

            if (process == null || parameter == null)
            {
                return false;
            }

            var newProcessParameter = new ProcessParameter
            {
                ProcessId = addNew.ProcessId,
                ParameterId = addNew.ParameterId,
                Value = addNew.Value
            };

            _dbContext.ProcessParameters.Add(newProcessParameter);

            _dbContext.SaveChanges();

            return true; 
        }

        public bool UpdateProcess(ProcessParameterUpdateProcess updateProcess)
        {
            var process = _dbContext.Processes.FirstOrDefault(m => m.Id == updateProcess.ProcessId);
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == updateProcess.ProcessParameterId);

            if (processParameter == null || process == null)
            {
                return false;
            }

            processParameter.ProcessId = updateProcess.ProcessId;

            _dbContext.SaveChanges();

            return true;
        }

        public bool UpdateParameter(ProcessParameterUpdateParameter updateParameter)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == updateParameter.ParameterId);
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == updateParameter.ProcessParameterId);

            if (processParameter == null || parameter == null)
            {
                return false;
            }

            processParameter.ParameterId = updateParameter.ParameterId;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Update(ProcessParameterUpdate update)
        {
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == update.ProcessParameterId);
            var process = _dbContext.Processes.FirstOrDefault(m => m.Id == update.ProcessId);
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == update.ParameterId);

            if (process == null || parameter == null || processParameter == null)
            {
                return false;
            }
            processParameter.ProcessId = update.ProcessId;
            processParameter.ParameterId = update.ParameterId;
            processParameter.Value = update.Value;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Delete(int processParameterId)
        {
            var ProcessParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == processParameterId);

            if (ProcessParameter == null)
            {
                return false;
            }

            _dbContext.ProcessParameters.Remove(ProcessParameter);

            _dbContext.SaveChanges();

            return true;
        }
    }
}
