using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.Models;

namespace MiniMesTrainApi.IRepository
{
    public interface IRepository
    {
        //List<Process> SelectBy(SelectBy formData);
        List<Process> SelectAll();
        //bool AddParameter(int processId, int parameterId, string value);
        bool AddNew(string serialNumber, int orderId);
        //bool ChangeOrder(int processId, int orderId);
        bool Update<T>(T data) where T : class;
        bool Delete(int Id);
    }
}
