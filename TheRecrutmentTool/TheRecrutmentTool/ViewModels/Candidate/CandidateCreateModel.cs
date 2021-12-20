namespace TheRecrutmentTool.ViewModels.Candidate
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TheRecrutmentTool.ViewModels.Recruiter;
    using TheRecrutmentTool.ViewModels.Skill;

    public class CandidateCreateModel : IValidatableObject
    {
        [Required(ErrorMessage = "Candidate FirstName should not be empty.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Candidate LastName should not be empty.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Candidate Email should not be empty.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Candidate Biography should not be empty.")]
        public string Bio { get; set; }

        [Required(ErrorMessage = "Candidate BirthDate should be in valid format and couldn't be empty.")]
        public string BirthDate { get; set; }


        [Required(ErrorMessage = "The candidate should have at least one skill")]
        public ICollection<SkillCreateModel> Skills { get; set; }

        [Required(ErrorMessage = "The candidate should have a recruiter")]
        public RecruiterCreateModel Recruiter { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Skills.Count == 0)
            {
                yield return new ValidationResult("The candidate should have at least one skill");
            }
        }
    }
}
