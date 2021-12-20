namespace TheRecrutmentTool.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using TheRecrutmentTool.Data.Models.BaseModels;

    public class JobSkill : BaseDeletableModel, IDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
