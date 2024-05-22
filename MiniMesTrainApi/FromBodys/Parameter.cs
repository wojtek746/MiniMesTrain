using Microsoft.AspNetCore.Mvc;

namespace MiniMesTrainApi.FromBodys.process
{
    public class ParameterAddNew
    {
        public string Name { get; set; }
        public string Unit { get; set; }
    }

    public class ParameterAddMachine
    {
        public int ParameterId { get; set; }
        public int MachineId { get; set; }
    }

    public class ParameterAddOrder
    {
        public int ParameterId { get; set; }
        public int ProcessParameterId { get; set; }
    }

    public class ParameterUpdate
    {
        public int ParameterId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }
}
