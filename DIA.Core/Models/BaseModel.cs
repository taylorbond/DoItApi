using System;
using Newtonsoft.Json;

namespace DIA.Core.Models
{
    public class BaseModel
    {
        public string Id { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}