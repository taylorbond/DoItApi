using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DIA.Core.Models
{
    public class Comment
    {
        public string Id { get; set; }
        [Required]
        public string Text { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        public DateTimeOffset InsertedTime { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }
}