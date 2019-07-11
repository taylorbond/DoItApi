using System;
using System.ComponentModel.DataAnnotations;

namespace DIA.Core.Models
{
    public class AlertTime : BaseModel
    {
        [Required]
        public DateTimeOffset Time { get; set; }
    }
}