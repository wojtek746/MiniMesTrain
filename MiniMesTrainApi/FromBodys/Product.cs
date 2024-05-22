using Microsoft.AspNetCore.Mvc;

namespace MiniMesTrainApi.FromBodys.process
{
    public class ProductAddNew
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductAddOrder
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }

    public class ProductUpdate
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
