﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniMesTrainApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        
        public ICollection<Order> Orders { get; set; } = null!;
    }
}
