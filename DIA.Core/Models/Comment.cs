using System.ComponentModel.DataAnnotations;

namespace DIA.Core.Models
{
    public class Comment : BaseModel
    {
        [Required]
        public string Text { get; set; }
    }
}