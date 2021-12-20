namespace TheRecrutmentTool.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TheRecrutmentTool.Data.Models.BaseModels;

    public class Recruiter : BaseDeletableModel, IDeletableEntity
    {
        public Recruiter()
        {
            this.Interviews = new HashSet<Interview>();
            this.Candidates = new HashSet<Candidate>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "LastName should not be empty.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email should not be empty.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Country should not be empty.")]
        public string Country { get; set; }
        public int InterviewSlots { get; set; }
        public int ExperienceLevel { get; set; } = 1;

        public ICollection<Interview> Interviews { get; set; } // this count = InterviewSlots
        public ICollection<Candidate> Candidates { get; set; } // this count = ExperienceLevel
    }
}
