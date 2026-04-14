using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShimsServer.Models.Labs;
using ShimsServer.Models.ConsultingRoom;
using ShimsServer.Models.Schemes;
using ShimsServer.Models.OPD;
using ShimsServer.Models.Drugs;
using ShimsServer.Models.Wards;
using ShimsServer.Models.Records;
using ShimsServer.Models.Services;
using ShimsServer.Models.Accounts;

namespace ShimsServer.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // Records
        public virtual DbSet<Patients> Patients { get; set; }
        public virtual DbSet<PatientAttendance> PatientAttendances { get; set; }
        public virtual DbSet<PatientSchemes> PatientSchemes { get; set; }

        // Schemes
        public virtual DbSet<Schemes> Schemes { get; set; }
        public virtual DbSet<SchemeDrugs> SchemeDrugs { get; set; }
        public virtual DbSet<SchemeServices> SchemeServices { get; set; }
        public virtual DbSet<SchemeInvestigations> SchemeInvestigations { get; set; }

        // Services
        public virtual DbSet<Models.Services.Services> Services { get; set; }
        public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }
        public virtual DbSet<ServicePayment> ServicePayments { get; set; }
        public virtual DbSet<ServiceRendering> ServiceRenderings { get; set; }

        // Drugs
        public virtual DbSet<Drugs> Drugs { get; set; }
        public virtual DbSet<DrugsStock> DrugsStocks { get; set; }
        public virtual DbSet<DrugsRequests> DrugsRequests { get; set; }
        public virtual DbSet<DispensingCalculations> DispensingCalculations { get; set; }
        public virtual DbSet<DrugPayments> DrugPayments { get; set; }
        public virtual DbSet<Dispensing> Dispensings { get; set; }

        // Labs
        public virtual DbSet<Investigations> Investigations  { get; set; }
        //public virtual DbSet<InvestigationParameters> InvestigationParameters { get; set; }
        public virtual DbSet<InvestigationsRequests> InvestigationsRequests { get; set; }
        public virtual DbSet<InvestigationsPayment> InvestigationsPayments { get; set; }
        public virtual DbSet<InvestigationsResults> InvestigationsResults { get; set; }

        //Diagnoses
        public virtual DbSet<Diagnoses> Diagnoses { get; set; }

        public virtual DbSet<SchemeDiagnoses> SchemeDiagnoses { get; set; }

        // Wards
        public virtual DbSet<Wards> Wards { get; set; }
        public virtual DbSet<WardAdmissions> WardAdmissions { get; set; }

        // Consulting Room
        public virtual DbSet<PatientConsultation> PatientConsultations { get; set; }
        public virtual DbSet<PatientOutcomes> PatientOutcomes { get; set; }
        public virtual DbSet<PatientDiagnosis> PatientDiagnoses { get; set; }
        public virtual DbSet<PatientSignsAndSymptoms> PatientSignsAndSymptoms { get; set; }

        // OPD
        public virtual DbSet<Vitals> Vitals { get; set; }

        // Accounts
        public virtual DbSet<Payments> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            foreach (var entity in mb.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName()?.ToLower());

                // Replace column names            
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToLower());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName()?.ToLower());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName()?.ToLower());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.Name?.ToLower());
                }
            }

            // Configure Identity table column lengths
            mb.Entity<ApplicationUser>(b =>
            {
                b.HasAlternateKey(x => x.UserName);
                b.Property(u => u.UserName).HasMaxLength(70);
                b.Property(u => u.NormalizedUserName).HasMaxLength(70);
                b.Property(u => u.Email).HasMaxLength(70);
                b.Property(u => u.NormalizedEmail).HasMaxLength(70);
                b.Property(u => u.SecurityStamp).HasMaxLength(64);
                b.Property(u => u.PasswordHash).HasMaxLength(168);
                b.Property(u => u.ConcurrencyStamp).HasMaxLength(64);
            });

            mb.Entity<IdentityRole<Guid>>(b =>
            {
                b.Property(r => r.Name).HasMaxLength(70);
                b.Property(r => r.NormalizedName).HasMaxLength(70);
                b.Property(r => r.ConcurrencyStamp).HasMaxLength(64);
            });

            // Configure claim types and values
            mb.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.Property(c => c.ClaimType).HasMaxLength(100);
                b.Property(c => c.ClaimValue).HasMaxLength(100);
            });

            mb.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.Property(c => c.ClaimType).HasMaxLength(100);
                b.Property(c => c.ClaimValue).HasMaxLength(100);
            });

            // Configure UserLogin and UserToken provider/key lengths
            mb.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.Property(l => l.LoginProvider).HasMaxLength(128);
                b.Property(l => l.ProviderKey).HasMaxLength(128);
                b.Property(l => l.ProviderDisplayName).HasMaxLength(100);
            });

            mb.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.Property(t => t.LoginProvider).HasMaxLength(128);
                b.Property(t => t.Name).HasMaxLength(128);
                b.Property(t => t.Value).HasMaxLength(512);
            });
        }
    }
}
