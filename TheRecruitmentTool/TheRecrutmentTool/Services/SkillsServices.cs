namespace TheRecrutmentTool.Services
{
    using System.Threading.Tasks;
    using TheRecrutmentTool.Data;
    using Microsoft.EntityFrameworkCore;
    using TheRecrutmentTool.Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class SkillsServices : ISkillsServices
    {
        private readonly ApplicationDbContext dbContext;

        public SkillsServices(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> IsSkillExistsAsync(string name)
        {
            return (await this.dbContext.Skills.FirstOrDefaultAsync(x => x.Name == name)) != null;
        }

        public async Task<int> GetSkillIdAsync(string name)
        {
            return (await this.dbContext.Skills.FirstOrDefaultAsync(x => x.Name == name)).Id;
        }

        public async Task<Skill> GetSkillsByIdAsync(int skillId)
        {
            return await this.dbContext.Skills.FirstOrDefaultAsync(x => x.Id == skillId);
        }

        public async Task<ICollection<Skill>> GetSkillsByIdAsync(IEnumerable<int> skillIds)
        {
            ICollection<Skill> skills = new List<Skill>();

            foreach (var skillId in skillIds)
            {
                var skill = await this.dbContext.Skills.FirstOrDefaultAsync(x => x.Id == skillId);
                skills.Add(skill);
            }

            return skills;
        }

        public async Task<int> CreateAsync(string name)
        {
            var skill = new Skill()
            {
                Name = name,
            };

            var entity = await this.dbContext.Skills.AddAsync(skill);
            await this.dbContext.SaveChangesAsync();
            return entity.Entity.Id;
        }

        public async Task<int> CreateIfNotExistsAsync(string name)
        {
            var isSkillExists = await this.IsSkillExistsAsync(name);

            if (isSkillExists)
            {
                return await this.GetSkillIdAsync(name);
            }

            return await this.CreateAsync(name);
        }

        public async Task<ICollection<int>> CreateIfNotExistsAsync(IEnumerable<string> skillNames)
        {
            ICollection<int> skillIds = new List<int>();

            foreach (var skillName in skillNames)
            {
                var isSkillExists = await this.IsSkillExistsAsync(skillName);

                if (isSkillExists)
                {
                    skillIds.Add(await this.GetSkillIdAsync(skillName));
                }
                else
                {
                    skillIds.Add(await this.CreateAsync(skillName));
                }
            }

            return skillIds;
        }

        public async Task UpdateCandidateSkills(int candidateId, IEnumerable<CandidateSkill> newSkills)
        {
            var entity = await this.dbContext.Candidates.FirstOrDefaultAsync(x => x.Id == candidateId);
            // Remove all old skills and add all new ones if skills are changed.
            var entitySkillIds = await this.dbContext.CandidateSkills
                                                        .Where(x => x.CandidateId == candidateId)
                                                        .Select(x => x.SkillId)
                                                        .ToArrayAsync();
            var newSkillIds = newSkills.Select(x => x.SkillId).ToHashSet();

            var isSkillsChanged = !entitySkillIds.ToHashSet().SetEquals(newSkillIds);
            if (!isSkillsChanged)
            {
                return;
            }

            // Remove all old skills.
            var oldSkills = await this.dbContext.CandidateSkills.Where(x => x.CandidateId == candidateId).ToArrayAsync();
            this.dbContext.CandidateSkills.RemoveRange(oldSkills);
            await this.dbContext.SaveChangesAsync();

            // Add new skills.
            entity.Skills = newSkills.ToArray();

            await this.dbContext.SaveChangesAsync();
        }
    }
}
