using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DIA.Core.Models
{
    [Table("Tasks")]
    public class DiaTask
    {
        public string Id { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        [Required]
        public string TaskDescription { get; set; }
        [Required]
        public DateTimeOffset DueDateTime { get; set; } = DateTimeOffset.UtcNow.Date;
        public ICollection<Comment> Comments { get; set; }
        public ICollection<AlertTime> AlertTimes { get; set; }

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DiaTask()
        {
            Comments = new List<Comment>();
            AlertTimes = new List<AlertTime>();
        }
    }
}
