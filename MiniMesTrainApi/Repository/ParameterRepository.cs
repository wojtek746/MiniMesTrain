using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Parameter = MiniMesTrainApi.Models.Parameter;

namespace MiniMesTrainApi.Repository
{
    public class ParameterRepository : IRepository<ParameterUpdate, Parameter, ParameterAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;
        private readonly ILogger<ParameterRepository> _logger;

        public ParameterRepository(MiniProductionDbContext dbContext, ILogger<ParameterRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<Parameter> SelectAll()
        {
            _logger.LogInformation($"Saw all Parameters");
            return _dbContext.Parameters.Include(m => m.ProcessParameters).Include(m => m.MachineParameter).ToList();
        }

        public bool AddNew(ParameterAddNew addNew)
        {
            var newParameter = new Parameter
            {
                Name = addNew.Name,
                Unit = addNew.Unit
            };

            _dbContext.Parameters.Add(newParameter);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully added Parameter on Id {newParameter.Id} with Name: {newParameter.Name} and Unit: {newParameter.Unit}");

            return true; 
        }

        public bool AddMachine(ParameterAddMachine addMachine)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == addMachine.ParameterId);
            var machine = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == addMachine.MachineId);

            if (parameter == null || machine == null)
            {
                if (machine == null)
                {
                    _logger.LogError($"Not found Machine with Id {addMachine.MachineId}");
                }
                else
                {
                    _logger.LogError($"Not found Order with Id {addMachine.ParameterId}");
                }
                return false;
            }

            var machineParameter = new MachineParameter
            {
                MachineId = addMachine.MachineId,
                ParameterId = addMachine.ParameterId
            };

            _dbContext.MachineParameter.Add(machineParameter);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully added Machine with Id {machine.Id} for Parameter with Id {parameter.Id}");

            return true; 
        }

        public bool AddOrder(ParameterAddOrder addOrder)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == addOrder.ParameterId);
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == addOrder.ProcessParameterId);
            if (processParameter == null || parameter == null)
            {
                if (processParameter == null)
                {
                    _logger.LogError($"Not found ProcessParameter with Id {addOrder.ProcessParameterId}");
                }
                else
                {
                    _logger.LogError($"Not found Parameter with Id {addOrder.ParameterId}");
                }
                return false;
            }

            processParameter.ParameterId = addOrder.ParameterId;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully added ProcessParameter with Id {processParameter.Id} for Parameter with Id {parameter.Id}");

            return true;
        }

        public bool Update(ParameterUpdate update)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == update.ParameterId);

            if (parameter == null)
            {
                _logger.LogError($"Not found Parameter with Id {update.ParameterId}");
                return false;
            }

            var last = new
            {
                Name = parameter.Name,
                Unit = parameter.Unit
            }; 

            parameter.Name = update.Name;
            parameter.Unit = update.Unit;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully updated Parameter with Id {parameter.Id} from Name: {last.Name} and Unit: {last.Unit} for Name: {parameter.Name} and Unit: {parameter.Unit}");

            return true;
        }

        public bool Delete(int parameterId)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);

            if (parameter == null)
            {
                _logger.LogError($"Not found Parameter with Id {parameterId}");
                return false;
            }

            var last = new
            {
                Id = parameter.Id,
                Name = parameter.Name,
                Unit = parameter.Unit
            };

            _dbContext.Parameters.Remove(parameter);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully deleted Parameter with Id {last.Id} with Name: {last.Name} and Unit: {last.Unit}");

            return true;
        }
    }
}
