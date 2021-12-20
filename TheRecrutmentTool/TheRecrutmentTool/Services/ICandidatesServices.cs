
namespace TheRecrutmentTool.Services
{
    using System.Threading.Tasks;
    using TheRecrutmentTool.Data.Models;

    public interface ICandidatesServices
    {
        Task<bool> IsCandidateExistsAsync(int id);

        Task<bool> IsCandidateEmailExistsAsync(string email);

        Task<int> CreateAsync(Candidate candidate);

        Task<Candidate> GetByIdAsync(int id);

        Task<bool> IsCandidateOwnsEmail(int candidateId, string email);

        Task<int> UpdateAsync(int candidateId, Candidate candidate);

        Task<Recruiter> GetCandidateRecruiter(int candidateId);

        Task<int> DeleteAsync(int candidateId);

        Task<int> DeleteHardAsync(int candidateId);
    }
}
