using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DoItApi.Models
{
    public class Task
    {
        public string Id { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        [Required]
        public string TaskDescription { get; set; }
        [Required]
        public DateTimeOffset DueDateTime { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<AlertTime> AlertTimes { get; set; }

        public Task()
        {
            Comments = new List<Comment>();
            AlertTimes = new List<AlertTime>();
        }
    }
}
