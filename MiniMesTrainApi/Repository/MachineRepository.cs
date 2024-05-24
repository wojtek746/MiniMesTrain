using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Machine = MiniMesTrainApi.Models.Machine;

namespace MiniMesTrainApi.Repository
{
    public class MachineRepository : IRepository<MachineUpdate, Machine, MachineAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;
        private readonly ILogger<MachineRepository> _logger;

        public MachineRepository(MiniProductionDbContext dbContext, ILogger<MachineRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<Machine> SelectAll()
        {
            _logger.LogInformation($"Saw all Machines");
            return _dbContext.Machines.Include(m => m.Orders).Include(m => m.MachineParameter).ToList();
        }

        public bool AddNew(MachineAddNew addNew)
        {
            var newMachine = new Machine
            {
                Name = addNew.Name,
                Description = addNew.Description
            };

            _dbContext.Machines.Add(newMachine);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully added Machine on Id {newMachine.Id} with Name: {newMachine.Name} and Description: {newMachine.Description}");

            return true;
        }

        public bool AddOrder(MachineAddOrder addOrder)
        {
            var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == addOrder.MachineId);
            var order = _dbContext.Orders.Find((long)addOrder.OrderId);
            if (machine == null || order == null)
            {
                if (machine == null)
                {
                    _logger.LogError($"Not found Machine with Id {addOrder.MachineId}"); 
                }
                else
                {
                    _logger.LogError($"Not found Order with Id {addOrder.OrderId}");
                }
                return false;
            }

            order.MachineId = addOrder.MachineId;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully added Order with Id {order.Id} for Machine with Id {machine.Id}");

            return true;
        }

        public bool AddParameter(MachineAddParameter addParameter)
        {
            var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == addParameter.MachineId);
            var parameter = _dbContext.Parameters.Find(addParameter.ParameterId);
            if (parameter == null || machine == null)
            {
                if (machine == null)
                {
                    _logger.LogError($"Not found Machine with Id {addParameter.MachineId}");
                }
                else
                {
                    _logger.LogError($"Not found Parameter with Id {addParameter.ParameterId}");
                }
                return false;
            }

            var machineParameter = new MachineParameter
            {
                MachineId = addParameter.MachineId,
                ParameterId = addParameter.ParameterId
            };

            _dbContext.MachineParameter.Add(machineParameter);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully added Parameter with Id {parameter.Id} for Machine with Id {machine.Id}");

            return true;
        }

        public bool Update(MachineUpdate update)
        {
            var machine = _dbContext.Machines.FirstOrDefault(m => m.Id == update.MachineId);

            if (machine == null)
            {
                _logger.LogError($"Not found Machine with Id {update.MachineId}");
                return false;
            }

            var last = new
            {
                Name = machine.Name,
                Description = machine.Description,
            };

            machine.Name = update.Name;
            machine.Description = update.Description;

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully Updated Machine on Id {machine.Id} to Name: {machine.Name} and Description: {machine.Description} (last Name was Name: {last.Name} and Description: {last.Description})");

            return true;
        }

        public bool Delete(int machineId)
        {
            var machine = _dbContext.Machines.FirstOrDefault(m => m.Id == machineId);

            if (machine == null)
            {
                _logger.LogError($"Not found Machine with Id {machineId}");
                return false;
            }

            var last = new
            {
                Id = machine.Id,
                Name = machine.Name,
                Description = machine.Description,
            };

            _dbContext.Machines.Remove(machine);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully Deleted Machine on Id {last.Id} with Name: {last.Name} and Description: {last.Description}");

            return true;
        }
    }
}
