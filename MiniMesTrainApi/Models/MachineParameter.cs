using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniMesTrainApi.Models
{
    public class MachineParameter
    {
        [Key]
        public int Id { get; set; }
        public int MachineId { get; set; }
        public int ParameterId { get; set; }

        [JsonIgnore]
        [ForeignKey("MachineId")]
        public Machine Machine { get; set; } = null!;
        [JsonIgnore]
        [ForeignKey("ParameterId")]
        public Parameter Parameter { get; set; } = null!;
    }
}
