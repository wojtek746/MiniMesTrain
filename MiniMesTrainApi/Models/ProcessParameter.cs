using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniMesTrainApi.Models
{
    public class ProcessParameter
    {
        [Key]
        public long Id { get; set; }
        public long ProcessId { get; set; }
        public int ParameterId { get; set; }
        public string Value { get; set; } = null!;

        [JsonIgnore]
        [ForeignKey("ProcessId")]
        public Process Process { get; set; } = null!;
        [JsonIgnore]
        [ForeignKey("ParameterId")]
        public Parameter Parameter { get; set; } = null!;
    }
}
