using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniMesTrainApi.Models
{
    public class Order
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; } = "";

        public int MachineId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        [ForeignKey("MachineId")]
        public Machine Machine { get; set; } = null!;
        [JsonIgnore]
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
        
        public ICollection<Process> Processes { get; set; } = null!;
    }
}
