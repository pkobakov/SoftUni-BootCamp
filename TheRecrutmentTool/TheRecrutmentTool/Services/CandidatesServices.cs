namespace TheRecrutmentTool.Services
{
    using System.Linq;
    using TheRecrutmentTool.Data;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TheRecrutmentTool.Data.Models;
    using System;

    public class CandidatesServices : ICandidatesServices
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IRecruitersServices recruitersServices;
        private readonly ISkillsServices skillsServices;

        public CandidatesServices(
            ApplicationDbContext dbContext,
            IRecruitersServices recruitersServices,
            ISkillsServices skillsServices)
        {
            this.dbContext = dbContext;
            this.recruitersServices = recruitersServices;
            this.skillsServices = skillsServices;
        }

        public async Task<bool> IsCandidateExistsAsync(int id)
        {
            return (await this.dbContext.Candidates.FirstOrDefaultAsync(x => x.Id == id)) != null;
        }

        public async Task<bool> IsCandidateEmailExistsAsync(string email)
        {
            return (await this.dbContext.Candidates.FirstOrDefaultAsync(x => x.Email == email)) != null;
        }

        public async Task<int> CreateAsync(Candidate candidate)
        {
            var entity = await this.dbContext.Candidates.AddAsync(candidate);
            await this.dbContext.SaveChangesAsync();

            return entity.Entity.Id;
        }

        public async Task<Candidate> GetByIdAsync(int id)
        {
            return await this.dbContext.Candidates.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> IsCandidateOwnsEmail(int candidateId, string email)
        {
            var candidateByEmail = await this.dbContext.Candidates.FirstOrDefaultAsync(x => x.Email == email);

            if (candidateByEmail == null)
            {
                return true;
            }

            var candidateById = await this.dbContext.Candidates.FirstOrDefaultAsync(x => x.Id == candidateId);

            return candidateByEmail.Id == candidateById.Id;
        }

        public async Task<Recruiter> GetCandidateRecruiter(int candidateId)
        {
            var candidate = await this.GetByIdAsync(candidateId);
            return await this.recruitersServices.GetRecruiterByIdAsync(candidate.RecruiterId);
        }

        public async Task<int> UpdateAsync(int candidateId, Candidate candidate)
        {
            var entity = await this.dbContext.Candidates.FirstOrDefaultAsync(x => x.Id == candidateId);

            entity.Email = candidate.Email;
            entity.FirstName = candidate.FirstName;
            entity.LastName = candidate.LastName;
            entity.Bio = candidate.Bio;
            entity.BirthDate = candidate.BirthDate;
            entity.RecruiterId = candidate.RecruiterId;
            await this.dbContext.SaveChangesAsync();

            await this.skillsServices.UpdateCandidateSkills(candidateId, candidate.Skills);

            return entity.Id;
        }

        public async Task<int> DeleteAsync(int candidateId)
        {
            var entity = await this.GetByIdAsync(candidateId);
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.Now;
            await this.dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> DeleteHardAsync(int candidateId)
        {
            var entity = await this.GetByIdAsync(candidateId);
            var removedEntity = this.dbContext.Candidates.Remove(entity);
            await this.dbContext.SaveChangesAsync();
            return removedEntity.Entity.Id;
        }
    }
}
