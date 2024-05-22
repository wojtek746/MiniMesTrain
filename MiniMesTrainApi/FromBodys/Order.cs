using Microsoft.AspNetCore.Mvc;

namespace MiniMesTrainApi.FromBodys.process
{
    public class OrderAddNew
    {
        public string Code { get; set; }
        public int MachineId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderUpdateMachine
    {
        public int OrderId { get; set; }
        public int MachineId { get; set; }
    }

    public class OrderUpdateProduct
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }

    public class OrderUpdate
    {
        public int OrderId { get; set; }
        public string Code { get; set; }
        public int MachineId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
