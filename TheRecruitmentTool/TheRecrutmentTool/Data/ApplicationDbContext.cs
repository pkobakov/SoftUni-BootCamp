namespace TheRecrutmentTool.Data
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using TheRecrutmentTool.Data.Models;
    using TheRecrutmentTool.Data.Models.BaseModels;

    public class ApplicationDbContext : DbContext
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public virtual DbSet<Candidate> Candidates { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Recruiter> Recruiters { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<CandidateSkill> CandidateSkills { get; set; }
        public virtual DbSet<JobSkill> JobSkills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=TheRecruitmentTool;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recruiter>()
                .HasMany(e => e.Interviews)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);


            // Set global query filter for not deleted entities only
            var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
