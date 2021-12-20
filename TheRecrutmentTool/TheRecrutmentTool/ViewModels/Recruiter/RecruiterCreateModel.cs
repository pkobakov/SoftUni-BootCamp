using System.ComponentModel.DataAnnotations;

namespace TheRecrutmentTool.ViewModels.Recruiter
{
    public class RecruiterCreateModel
    {
        [Required(ErrorMessage = "Recruiter LastName should not be empty.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Recruiter Email should not be empty.")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Recruiter Country should not be empty.")]
        public string Country { get; set; }
    }
}
