using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniMesTrainApi.Models
{
    public class Process
    {
        [Key]
        public long Id { get; set; }
        public string SerialNumber { get; set; } = null!;
        public long OrderId { get; set; }
        public ProcessStatus Status { get; set; }
        public DateTime DateTime { get; set; }

        [JsonIgnore]
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;
        
        public ICollection<ProcessParameter> ProcessParameters { get; set; } = null!;
    }
}
