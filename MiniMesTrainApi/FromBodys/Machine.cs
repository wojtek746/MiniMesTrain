using Microsoft.AspNetCore.Mvc;

namespace MiniMesTrainApi.FromBodys.process
{
    public class MachineAddNew
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MachineAddOrder
    {
        public int MachineId { get; set; }
        public int OrderId { get; set; }
    }

    public class MachineAddParameter
    {
        public int MachineId { get; set; }
        public int ParameterId { get; set; }
    }

    public class MachineUpdate
    {
        public int MachineId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
