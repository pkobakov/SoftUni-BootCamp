namespace TheRecrutmentTool.Services
{
    using System.Threading.Tasks;
    using TheRecrutmentTool.Data;
    using TheRecrutmentTool.Constants;
    using Microsoft.EntityFrameworkCore;
    using TheRecrutmentTool.Data.Models;

    public class RecruitersServices : IRecruitersServices
    {
        private readonly ApplicationDbContext dbContext;

        public RecruitersServices(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> IsRecruiterExistsAsync(string email)
        {
            return (await this.dbContext.Recruiters.FirstOrDefaultAsync(x => x.Email == email)) != null;
        }

        public async Task<int> GetRecruiterIdAsync(string email)
        {
            return (await this.dbContext.Recruiters.FirstOrDefaultAsync(x => x.Email == email)).Id;
        }

        public async Task<Recruiter> GetRecruiterByIdAsync(int id)
        {
            return await this.dbContext.Recruiters.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> CreateAsync(Recruiter recruiter)
        {
            var entity = await this.dbContext.AddAsync(recruiter);
            await this.dbContext.SaveChangesAsync();

            return entity.Entity.Id;
        }

        public async Task<int> CreateIfNotExistsAsync(Recruiter recruiter)
        {
            var isRecruiterExists = await this.IsRecruiterExistsAsync(recruiter.Email);

            var recruiterId = 
                isRecruiterExists 
                ? await this.GetRecruiterIdAsync(recruiter.Email) 
                : await this.CreateAsync(recruiter);

            //await this.IncreaseRecruiterExperience(recruiterId);
            return recruiterId;
        }

        public async Task IncreaseRecruiterExperience(int recruiterId)
        {
            var recruiter = await this.dbContext.Recruiters.FirstOrDefaultAsync(x => x.Id == recruiterId);
            recruiter.ExperienceLevel += 1;
            await this.dbContext.SaveChangesAsync();
        }

        private async Task<bool> HasRecruiterFreeSlots(int recruiterId)
        {
            var recruiter = await this.dbContext.Recruiters.FirstOrDefaultAsync(x => x.Id == recruiterId);
            return recruiter.InterviewSlots < RecruiterConstants.recruiterSlotsMaxCount;
        }
    }
}
