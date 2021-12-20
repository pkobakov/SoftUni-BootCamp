namespace TheRecrutmentTool.Services
{
    using System.Threading.Tasks;
    using TheRecrutmentTool.Data.Models;

    public interface IRecruitersServices
    {
        Task<bool> IsRecruiterExistsAsync(string email);

        Task IncreaseRecruiterExperience(int recruiterId);

        Task<int> GetRecruiterIdAsync(string email);

        Task<Recruiter> GetRecruiterByIdAsync(int id);

        Task<int> CreateAsync(Recruiter recruiter);

        Task<int> CreateIfNotExistsAsync(Recruiter recruiter);
    }
}
