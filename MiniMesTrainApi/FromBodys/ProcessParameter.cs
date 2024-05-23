using Microsoft.AspNetCore.Mvc;

namespace MiniMesTrainApi.FromBodys.process
{
    public class ProcessParameterAddNew
    {
        public int ProcessId { get; set; }
        public int ParameterId { get; set; }
        public string Value { get; set; }
    }

    public class ProcessParameterUpdateProcess
    {
        public int ProcessParameterId { get; set; }
        public int ProcessId { get; set; }
    }

    public class ProcessParameterUpdateParameter
    {
        public int ProcessParameterId { get; set; }
        public int ParameterId { get; set; }
    }

    public class ProcessParameterUpdate
    {
        public int ProcessParameterId { get; set; }
        public int ProcessId { get; set; }
        public int ParameterId { get; set; }
        public string Value { get; set; }
    }
}
