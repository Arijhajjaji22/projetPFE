using App_plateforme_de_recurtement.Models;
using Microsoft.EntityFrameworkCore;

namespace App_plateforme_de_recurtement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

       
        public DbSet<StageOffer> StageOffres { get; set; }

        public DbSet<AdminStageOffer> adminStageOffers { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Question { get; set; }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Answercandidat> answercandidats { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<AvailabilityConfirmation> availabilityConfirmations { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<UserRegistrationTemp> UserRegistrationTemps { get; set; }
        public DbSet<PdfDocument> PdfDocuments { get; set; }

        public DbSet<RescheduleRequesttest> rescheduleRequesttests { get; set; }
        /*  protected override void OnModelCreating(ModelBuilder modelBuilder)
          {
              // Configuration de la suppression en cascade
              modelBuilder.Entity<Test>()
                  .HasMany(t => t.Questions)
                  .WithOne(q => q.test)
                  .OnDelete(DeleteBehavior.Cascade);
          }*/



    }
}
    