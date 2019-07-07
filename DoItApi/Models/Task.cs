using System;
using System.Collections.Generic;

namespace DoItApi.Models
{
    public class Task
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string TaskDescription { get; set; }
        public DateTimeOffset DueDateTime { get; set; }
        public ICollection<string> Comments { get; set; }
        public ICollection<DateTimeOffset> AlertTimes { get; set; }
    }
}
