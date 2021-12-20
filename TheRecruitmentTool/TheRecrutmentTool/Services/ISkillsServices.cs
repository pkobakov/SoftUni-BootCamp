namespace TheRecrutmentTool.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TheRecrutmentTool.Data.Models;

    public interface ISkillsServices
    {
        Task<bool> IsSkillExistsAsync(string name);

        Task<int> GetSkillIdAsync(string name);

        Task<int> CreateAsync(string name);

        Task<int> CreateIfNotExistsAsync(string name);

        Task<ICollection<int>> CreateIfNotExistsAsync(IEnumerable<string> skillNames);

        Task<Skill> GetSkillsByIdAsync(int skillId);

        Task<ICollection<Skill>> GetSkillsByIdAsync(IEnumerable<int> skillIds);

        Task UpdateCandidateSkills(int candidateId, IEnumerable<CandidateSkill> newSkills);
    }
}
