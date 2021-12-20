using System.ComponentModel.DataAnnotations;

namespace TheRecrutmentTool.ViewModels.Skill
{
    public class SkillCreateModel
    {
        [Required(ErrorMessage = "Skill Name should not be empty.")]
        public string Name { get; set; }
    }
}
