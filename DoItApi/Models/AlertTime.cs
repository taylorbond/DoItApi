using System;
using System.ComponentModel.DataAnnotations;

namespace DoItApi.Models
{
    public class AlertTime
    {
        public string Id { get; set; }
        [Required]
        public DateTimeOffset Time { get; set; }
    }
}