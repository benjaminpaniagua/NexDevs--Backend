using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        public string SkillName { get; set; }
    }
}
