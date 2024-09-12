using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public int WorkId { get; set; }

        public string ContentNotification { get; set; }

        public DateTime CreateAt { get; set; }

        public char Status { get; set; }
    }
}
