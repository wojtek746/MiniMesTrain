using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniMesTrainApi.Repository
{
    public class ProcessParameterRepository : IRepository<ProcessParameterUpdate, ProcessParameter, ProcessParameterAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;
        private readonly ILogger<ProcessParameterRepository> _logger;

        public ProcessParameterRepository(MiniProductionDbContext dbContext, ILogger<ProcessParameterRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<ProcessParameter> SelectAll()
        {
            _logger.LogInformation($"Saw all ProcessParameters");
            return _dbContext.ProcessParameters.Include(m => m.Process).Include(m => m.Parameter).ToList();
        }
        
        public bool AddNew(ProcessParameterAddNew addNew)
        {
            var process = _dbContext.Processes.FirstOrDefault(m => m.Id == addNew.ProcessId);
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == addNew.ParameterId);

            if (process == null || parameter == null)
            {
                if (parameter == null)
                {
                    _logger.LogError($"Not found Parameter with Id {addNew.ParameterId}");
                }
                else
                {
                    _logger.LogError($"Not found Process with Id {addNew.ProcessId}");
                }
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

            _logger.LogInformation($"Successfully added ProcessParameter with Id {newProcessParameter.Id} with ProcessId: {newProcessParameter.ProcessId} and ParameterId: {newProcessParameter.ParameterId} and Value: {newProcessParameter.Value}");

            return true; 
        }

        public bool UpdateProcess(ProcessParameterUpdateProcess updateProcess)
        {
            var process = _dbContext.Processes.FirstOrDefault(m => m.Id == updateProcess.ProcessId);
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == updateProcess.ProcessParameterId);

            if (processParameter == null || process == null)
            {
                if (process == null)
                {
                    _logger.LogError($"Not found Process with Id {updateProcess.ProcessId}");
                }
                else
                {
                    _logger.LogError($"Not found ProcessParameter with Id {updateProcess.ProcessParameterId}");
                }
                return false;
            }

            var last = new
            {
                ProcessId = processParameter.ProcessId
            }; 

            processParameter.ProcessId = updateProcess.ProcessId;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully updated ProcessParameter with Id {processParameter.Id} from ProcessId: {last.ProcessId} to {processParameter.ProcessId}");

            return true;
        }

        public bool UpdateParameter(ProcessParameterUpdateParameter updateParameter)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == updateParameter.ParameterId);
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == updateParameter.ProcessParameterId);

            if (processParameter == null || parameter == null)
            {
                if (parameter == null)
                {
                    _logger.LogError($"Not found Parameter with Id {updateParameter.ParameterId}");
                }
                else
                {
                    _logger.LogError($"Not found ProcessParameter with Id {updateParameter.ProcessParameterId}");
                }
                return false;
            }

            var last = new
            {
                ParameterId = processParameter.ParameterId
            };

            processParameter.ParameterId = updateParameter.ParameterId;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully updated ProcessParameter with Id {processParameter.Id} from ParameterId: {last.ParameterId} to {processParameter.ParameterId}");

            return true;
        }

        public bool Update(ProcessParameterUpdate update)
        {
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == update.ProcessParameterId);
            var process = _dbContext.Processes.FirstOrDefault(m => m.Id == update.ProcessId);
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == update.ParameterId);

            if (process == null || parameter == null || processParameter == null)
            {
                if (process == null)
                {
                    _logger.LogError($"Not found Process with Id {update.ProcessId}");
                }
                else if (parameter == null)
                {
                    _logger.LogError($"Not found Parameter with Id {update.ParameterId}");
                }
                else
                {
                    _logger.LogError($"Not found ProcessParameter with Id {update.ProcessParameterId}");
                }
                return false;
            }

            var last = new
            {
                ProcessId = processParameter.ProcessId,
                ParameterId = processParameter.ParameterId,
                Value = processParameter.Value
            };

            processParameter.ProcessId = update.ProcessId;
            processParameter.ParameterId = update.ParameterId;
            processParameter.Value = update.Value;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully updated ProcessParameter with Id {processParameter.Id} from ProcessId: {last.ProcessId} to {processParameter.ProcessId} and from ParameterId: {last.ParameterId} to {processParameter.ParameterId} and from Value: {last.Value} to {processParameter.Value}");

            return true;
        }

        public bool Delete(int processParameterId)
        {
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == processParameterId);

            if (processParameter == null)
            {
                _logger.LogError($"Not found ProcessParameter with Id {processParameterId}");
                return false;
            }

            var last = new
            {
                Id = processParameter.Id,
                ProcessId = processParameter.ProcessId,
                ParameterId = processParameter.ParameterId,
                Value = processParameter.Value
            };

            _dbContext.ProcessParameters.Remove(processParameter);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully deleted ProcessParameter with Id {last.Id} with ProcessId: {last.ProcessId} and ParameterId: {last.ParameterId} and Value: {last.Value}");

            return true;
        }
    }
}
