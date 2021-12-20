namespace TheRecrutmentTool.Controllers
{
    using System;
    using System.Linq;
    using TheRecrutmentTool.Data;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using TheRecrutmentTool.Helpers;
    using TheRecrutmentTool.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using TheRecrutmentTool.Data.Models;
    using TheRecrutmentTool.ViewModels.Skill;
    using TheRecrutmentTool.ViewModels.Response;
    using TheRecrutmentTool.ViewModels.Candidate;
    using TheRecrutmentTool.ViewModels.Recruiter;

    [ApiController]
    [Route("[controller]")]
    public class CandidatesController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger; // TODO To record each action related to creating a new candidate. 
        private readonly IRecruitersServices recruitersServices;
        private readonly ICandidatesServices candidatesServices;
        private readonly ISkillsServices skillsServices;
        private readonly ApplicationDbContext dbContext;

        public CandidatesController(
            ILogger<WeatherForecastController> logger,
            IRecruitersServices recruitersServices,
            ICandidatesServices candidatesServices,
            ISkillsServices skillsServices,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            this.recruitersServices = recruitersServices;
            this.candidatesServices = candidatesServices;
            this.skillsServices = skillsServices;
            this.dbContext = dbContext;
        }

        [HttpPost, ActionName("/")]
        public async Task<IActionResult> CreateAsync([FromBody] CandidateCreateModel model)
        {
            try
            {
                var isCandidateEmailExist = await this.candidatesServices.IsCandidateEmailExistsAsync(model.Email);

                if (isCandidateEmailExist)
                {
                    var message = "This email is already used.";
                    return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = message });
                }

                // Create new Recruiter if not exists.
                var recruiter = new Recruiter()
                {
                    LastName = model.Recruiter.LastName,
                    Email = model.Recruiter.Email,
                    Country = model.Recruiter.Country,
                };
                var recruiterId = await this.recruitersServices.CreateIfNotExistsAsync(recruiter);
                await this.recruitersServices.IncreaseRecruiterExperience(recruiterId);

                // Create new Candidate.
                var newCandidate = new Candidate()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Bio = model.Bio,
                    BirthDate = DateTime.Parse(model.BirthDate),
                    RecruiterId = recruiterId,
                };
                var newCandidateId = await this.candidatesServices.CreateAsync(newCandidate);
                
                // Create new Skills if not exist.
                var skillNames = model.Skills.Select(x => x.Name).ToHashSet(); // Prevents duplicate elements.
                var skillIds = await this.skillsServices.CreateIfNotExistsAsync(skillNames);
                var skills = (await this.skillsServices.GetSkillsByIdAsync(skillIds)).Select(x => new CandidateSkill()
                {
                    CandidateId = newCandidateId,
                    SkillId = x.Id,
                }).ToArray();
                // Add Candidate's skills
                await this.skillsServices.UpdateCandidateSkills(newCandidateId, skills);

                return Ok(new { Id = newCandidateId, Message = "Candidate created successfully." });
            }
            catch (Exception ex)
            {
                var message = ExceptionMessageCreator.CreateMessage(ex);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = message });
            }
        }

        [HttpGet, ActionName("/")]
        [Route("{id}")]
        public async Task<IActionResult> GetOneAsync([FromRoute] int id)
        {
            try
            {
                var candidate = await this.candidatesServices.GetByIdAsync(id);

                if (candidate == null)
                {
                    var message = "This candidate do not exists.";
                    return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = message });
                };

                var recruiter = await this.recruitersServices.GetRecruiterByIdAsync(candidate.RecruiterId);

                var model = new CandidateViewModel()
                {
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                    BirthDate = candidate.BirthDate.ToString(),
                    Bio = candidate.Bio,
                    Email = candidate.Email,
                    Recruiter = new RecruiterViewModel()
                    {
                        LastName = recruiter.LastName,
                        Email = recruiter.Email,
                        Country = recruiter.Country,
                        ExperienceLevel = recruiter.ExperienceLevel,
                        InterviewSlots = recruiter.InterviewSlots,
                    },
                    Skills = (await this.dbContext.Skills
                                                    .Where(x => x.Candidates.Any(y => y.CandidateId == candidate.Id))
                                                    .ToArrayAsync())
                                                    .Select(x => new SkillViewModel() 
                                                    {
                                                        Name = x.Name,
                                                    }).ToArray(),
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                var message = ExceptionMessageCreator.CreateMessage(ex);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = message });
            }
        }

        [HttpPut, ActionName("/")]
        [Route("{id}")]
        public async Task<IActionResult> UpdateOneAsync([FromRoute] int id, [FromBody] CandidateCreateModel model)
        {
            try
            {
                var isCandidateExists = await this.candidatesServices.IsCandidateExistsAsync(id);
                if (!isCandidateExists)
                {
                    var message = "This candidate do not exists.";
                    return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = message });
                };

                var isCandidateOwnsEmail = await this.candidatesServices.IsCandidateOwnsEmail(id, model.Email);
                if (!isCandidateOwnsEmail)
                {
                    var message = "This candidate does not own this email.";
                    return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = message });
                }

                // Create new Recruiter if not exists.
                var recruiter = new Recruiter()
                {
                    LastName = model.Recruiter.LastName,
                    Email = model.Recruiter.Email,
                    Country = model.Recruiter.Country,
                };
                var recruiterId = await this.recruitersServices.CreateIfNotExistsAsync(recruiter);

                var updatedCandidateRecruiter = model.Recruiter;
                var currentCandidateRecruiter = await this.candidatesServices.GetCandidateRecruiter(id);

                // Check if updated recruiter is the same as current candidate recruiter.
                if (currentCandidateRecruiter.Email != updatedCandidateRecruiter.Email)
                {
                    await this.recruitersServices.IncreaseRecruiterExperience(recruiterId);
                }

                // Create new Skills if not exist.
                var skillNames = model.Skills.Select(x => x.Name).ToHashSet(); // Prevents duplicate elements.
                var skillIds = await this.skillsServices.CreateIfNotExistsAsync(skillNames);
                var skills = await this.skillsServices.GetSkillsByIdAsync(skillIds);

                // Update Candidate data.
                var candidate = new Candidate()
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Bio = model.Bio,
                    BirthDate = DateTime.Parse(model.BirthDate),
                    RecruiterId = recruiterId,
                    Skills = skills.Select(x => new CandidateSkill()
                    {
                        CandidateId = id,
                        SkillId = x.Id
                    }).ToArray(),
                };
                var candidateId = await this.candidatesServices.UpdateAsync(id, candidate);

                return Ok(new { Id = candidateId, Message = "Candidate updated successfully." });
            }
            catch (Exception ex)
            {
                var message = ExceptionMessageCreator.CreateMessage(ex);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = message });
            }
        }

        [HttpDelete, ActionName("/")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOneAsync([FromRoute] int id)
        {
            try
            {
                var isCandidateExists = await this.candidatesServices.IsCandidateExistsAsync(id);
                if (!isCandidateExists)
                {
                    var message = "This candidate do not exists.";
                    return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = message });
                };

                var deletedCandidateId = await this.candidatesServices.DeleteAsync(id);

                return Ok(new { Id = deletedCandidateId, Message = "Candidate deleted successfully." });
            }
            catch (Exception ex)
            {
                var message = ExceptionMessageCreator.CreateMessage(ex);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = message });
            }
        }

    }
}
