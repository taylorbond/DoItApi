using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIA.Core.Models
{
    [Table("Tasks")]
    public class DiaTask : BaseModel
    {
        [Required]
        public string TaskDescription { get; set; }
        [Required]
        public DateTimeOffset DueDateTime { get; set; } = DateTimeOffset.UtcNow.Date;
        public ICollection<Comment> Comments { get; set; }
        public ICollection<AlertTime> AlertTimes { get; set; }
        public DiaTask()
        {
            Comments = new List<Comment>();
            AlertTimes = new List<AlertTime>();
        }
    }
}
