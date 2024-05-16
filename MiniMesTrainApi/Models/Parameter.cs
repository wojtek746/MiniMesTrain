using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMesTrainApi.Models
{
    public class Parameter
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Unit { get; set; } = null!;

        public ICollection<ProcessParameter> ProcessParameters { get; set; } = null!;
        public ICollection<MachineParameter> MachineParameter { get; set; } = null!;
    }
}
