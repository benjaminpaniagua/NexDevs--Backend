using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexDevs.Models
{
    public class WorkCategory
    {
        [Key]
        public int Id { get; set; }

        public int WorkId { get; set; }

        public int CategoryId { get; set; }
    }
}