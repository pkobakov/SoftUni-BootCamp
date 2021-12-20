namespace TheRecrutmentTool.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TheRecrutmentTool.Data.Models.BaseModels;

    public class Job : BaseDeletableModel, IDeletableEntity
    {
        public Job()
        {
            this.Skills = new HashSet<JobSkill>();
            this.Interviews = new HashSet<Interview>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title should not be empty.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description should not be empty.")]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public double Salary { get; set; }
        public virtual ICollection<JobSkill> Skills { get; set; }

        public virtual ICollection<Interview> Interviews { get; set; }
    }
}
