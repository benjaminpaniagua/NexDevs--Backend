using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class WorkCategorie
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryImageUrl { get; set; }
    }
}

