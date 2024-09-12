using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class WorkSkill
    {
        [Key]
        public int WorkSkillId { get; set; }

        public int WorkId { get; set; }

        public int SkillId { get; set; }
    }
}
