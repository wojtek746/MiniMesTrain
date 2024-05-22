using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.IRepository;
using MiniMesTrainApi.Models;
using MiniMesTrainApi.Persistance;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MiniMesTrainApi.Repository
{
    public class MachineRepository : IRepository<MachineUpdate, Machine, MachineAddNew>
    {
        private readonly MiniProductionDbContext _dbContext;

        public MachineRepository(MiniProductionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Machine> SelectAll()
        {
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

            return true;
        }

        public bool AddOrder(MachineAddOrder addOrder)
        {
            var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == addOrder.MachineId);
            var order = _dbContext.Orders.Find((long)addOrder.OrderId);
            if (machine == null || order == null)
            {
                return false;
            }

            order.MachineId = addOrder.MachineId;

            _dbContext.SaveChanges();
            return true;
        }

        public bool AddParameter(MachineAddParameter addParameter)
        {
            var machine = _dbContext.Machines.Include(m => m.Orders).FirstOrDefault(m => m.Id == addParameter.MachineId);
            var parameter = _dbContext.Parameters.Find(addParameter.ParameterId);
            if (parameter == null || machine == null)
            {
                return false;
            }

            var machineParameter = new MachineParameter
            {
                MachineId = addParameter.MachineId,
                ParameterId = addParameter.ParameterId
            };

            _dbContext.MachineParameter.Add(machineParameter);

            _dbContext.SaveChanges();
            return true;
        }

        public bool Update(MachineUpdate update)
        {
            var machine = _dbContext.Machines.FirstOrDefault(m => m.Id == update.MachineId);
            if (machine == null)
            {
                return false;
            }

            machine.Name = update.Name;
            machine.Description = update.Description;

            _dbContext.SaveChanges();

            return true;
        }

        public bool Delete(int MachineId)
        {
            var Machine = _dbContext.Machines.FirstOrDefault(m => m.Id == MachineId);

            if (Machine == null)
            {
                return false;
            }

            _dbContext.Machines.Remove(Machine);

            _dbContext.SaveChanges();

            return true;
        }
    }
}
