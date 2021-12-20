namespace TheRecrutmentTool.ViewModels.Candidate
{
    using System.Collections.Generic;
    using TheRecrutmentTool.ViewModels.Recruiter;
    using TheRecrutmentTool.ViewModels.Skill;

    public class CandidateViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string BirthDate { get; set; }

        public RecruiterViewModel Recruiter { get; set; }

        public ICollection<SkillViewModel> Skills { get; set; }
    }
}
