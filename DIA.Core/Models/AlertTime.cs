using System;
using System.ComponentModel.DataAnnotations;

namespace DIA.Core.Models
{
    public class AlertTime
    {
        public string Id { get; set; }
        [Required]
        public DateTimeOffset Time { get; set; }
    }
}