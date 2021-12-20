namespace TheRecrutmentTool.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TheRecrutmentTool.Data.Models.BaseModels;

    public class Skill : BaseDeletableModel, IDeletableEntity
    {
        public Skill()
        {
            this.Candidates = new HashSet<CandidateSkill>();
            this.Jobs = new HashSet<JobSkill>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Skill name should not be empty.")]
        public string Name { get; set; }

        public virtual ICollection<CandidateSkill> Candidates { get; set; }
        public virtual ICollection<JobSkill> Jobs { get; set; }
    }
}
