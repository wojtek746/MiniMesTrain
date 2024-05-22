using MiniMesTrainApi.FromBodys.process;
using MiniMesTrainApi.Models;

namespace MiniMesTrainApi.IRepository
{
    public interface IRepository<UpdateDto, SelectDto, AddDto> where UpdateDto : class where SelectDto : class where AddDto : class
    {
        //List<Process> SelectBy(SelectBy formData);
        List<SelectDto> SelectAll();
        //bool AddParameter(int processId, int parameterId, string value);
        bool AddNew(AddDto data);
        //bool ChangeOrder(int processId, int orderId);
        bool Update(UpdateDto data); 
        bool Delete(int Id);
    }
}
