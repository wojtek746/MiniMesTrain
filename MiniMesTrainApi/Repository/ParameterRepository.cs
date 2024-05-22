using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Parameter = MiniMesTrainApi.Models.Parameter;

namespace MiniMesTrainApi.Repository
{
    public class ParameterRepository : IRepository<ParameterUpdate, Parameter, ParameterAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;

        public ParameterRepository(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Parameter> SelectAll()
        {
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

            return true; 
        }

        public bool AddMachine(ParameterAddMachine addMachine)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == addMachine.ParameterId);
            var machine = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == addMachine.MachineId);

            if (parameter == null || machine == null)
            {
                return false;
            }

            var machineParameter = new MachineParameter
            {
                MachineId = addMachine.MachineId,
                ParameterId = addMachine.ParameterId
            };

            _dbContext.MachineParameter.Add(machineParameter);

            _dbContext.SaveChanges();

            _dbContext.SaveChanges();
            return true; 
        }

        public bool AddOrder(ParameterAddOrder addOrder)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == addOrder.ParameterId);
            var processParameter = _dbContext.ProcessParameters.FirstOrDefault(m => m.Id == addOrder.ProcessParameterId);
            if (processParameter == null || parameter == null)
            {
                return false;
            }

            processParameter.ParameterId = addOrder.ParameterId;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Update(ParameterUpdate update)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == update.ParameterId);

            if (parameter == null)
            {
                return false;
            }

            parameter.Name = update.Name;
            parameter.Unit = update.Unit;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Delete(int parameterId)
        {
            var parameter = _dbContext.Parameters.FirstOrDefault(m => m.Id == parameterId);

            if (parameter == null)
            {
                return false;
            }

            _dbContext.Parameters.Remove(parameter);

            _dbContext.SaveChanges();

            return true;
        }
    }
}
