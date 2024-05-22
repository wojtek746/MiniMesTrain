using Microsoft.AspNetCore.Mvc;

namespace MiniMesTrainApi.FromBodys.process
{
    public class ProcessAddNew
    {
        public string SerialNumber { get; set; }
        public int OrderId { get; set; }
    }

    public class ProcessAddParameter
    {
        public int ProcessId { get; set; }
        public int ParameterId { get; set; }
        public string Value { get; set; }
    }

    public class ProcessUpdateOrder
    {
        public int ProcessId { get; set; }
        public int OrderId { get; set; }
    }

    public class ProcessUpdate
    {
        public int ProcessId { get; set; }
        public string SerialNumber { get; set; }
        public int OrderId { get; set; }
    }

    public class ProcessSelectBy
    {
        public int ProcessId { get; set; }
        public string SerialNumber { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public int MachineId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ProcessParameterId { get; set; }
        public int ParameterId { get; set; }
        public string ProcessParameterValue { get; set; }
        public string DateTimeFrom { get; set; }
        public string DateTimeTo { get; set; }
    }
}
