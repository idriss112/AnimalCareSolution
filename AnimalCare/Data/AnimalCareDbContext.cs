using AnimalCare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnimalCare.Data
{
    public class AnimalCareDbContext : IdentityDbContext<ApplicationUser>
    {
        public AnimalCareDbContext(DbContextOptions<AnimalCareDbContext> options)
            : base(options)
        {
        }

        // Domain tables
        public DbSet<Owner> Owners { get; set; } = default!;
        public DbSet<Animal> Animals { get; set; } = default!;
        public DbSet<Veterinarian> Veterinarians { get; set; } = default!;
        public DbSet<VetSpecialty> VetSpecialties { get; set; } = default!;
        public DbSet<VetSchedule> VetSchedules { get; set; } = default!;
        public DbSet<Appointment> Appointments { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // keep Identity configuration

            // -----------------------------
            // OWNER → ANIMALS  (1 → Many)
            // -----------------------------
            modelBuilder.Entity<Owner>()
                .HasMany(o => o.Animals)
                .WithOne(a => a.Owner)
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------
            // ANIMAL → APPOINTMENTS (1 → Many)
            // -----------------------------
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.Appointments)
                .WithOne(ap => ap.Animal)
                .HasForeignKey(ap => ap.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            // VETERINARIAN → APPOINTMENTS (1 → Many)
            // -----------------------------
            modelBuilder.Entity<Veterinarian>()
                .HasMany(v => v.Appointments)
                .WithOne(ap => ap.Veterinarian)
                .HasForeignKey(ap => ap.VeterinarianId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------
            // VETERINARIAN → SCHEDULES (1 → Many)
            // -----------------------------
            modelBuilder.Entity<Veterinarian>()
                .HasMany(v => v.VetSchedules)
                .WithOne(s => s.Veterinarian)
                .HasForeignKey(s => s.VeterinarianId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------------------------------------------
            // VETERINARIAN ↔ VETSPECIALTY (Many ↔ Many)
            // (NO explicit join entity — EF creates join table)
            // ---------------------------------------------------
            modelBuilder.Entity<Veterinarian>()
                .HasMany(v => v.VetSpecialties)
                .WithMany(s => s.Veterinarians)
                .UsingEntity<Dictionary<string, object>>(
                    "VeterinarianSpecialties", // Join table name
                    j => j
                        .HasOne<VetSpecialty>()
                        .WithMany()
                        .HasForeignKey("VetSpecialtyId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Veterinarian>()
                        .WithMany()
                        .HasForeignKey("VeterinarianId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("VeterinarianId", "VetSpecialtyId");
                        j.ToTable("VeterinarianSpecialties");
                    });

            // ---------------------------------------------------
            // APPLICATIONUSER ↔ VETERINARIAN (1 ↔ 0 or 1)
            // ---------------------------------------------------
                    modelBuilder.Entity<ApplicationUser>()
                    .HasOne(u => u.Veterinarian)
                    .WithOne(v => v.User)
                    .HasForeignKey<ApplicationUser>(u => u.VeterinarianId)
                    .OnDelete(DeleteBehavior.SetNull);


            // -----------------------------
            // PROPERTY CONFIGURATIONS
            // -----------------------------
            modelBuilder.Entity<Owner>(entity =>
            {
                entity.Property(o => o.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(o => o.LastName).HasMaxLength(100).IsRequired();
                entity.Property(o => o.Email).HasMaxLength(200);
                entity.Property(o => o.PhoneNumber).HasMaxLength(50);
            });

            modelBuilder.Entity<Animal>(entity =>
            {
                entity.Property(a => a.Name).HasMaxLength(100).IsRequired();
                entity.Property(a => a.Species).HasMaxLength(50).IsRequired();
                entity.Property(a => a.Breed).HasMaxLength(100);
            });

            modelBuilder.Entity<Veterinarian>(entity =>
            {
                entity.Property(v => v.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(v => v.LastName).HasMaxLength(100).IsRequired();
                entity.Property(v => v.Email).HasMaxLength(200);
            });

            modelBuilder.Entity<VetSpecialty>(entity =>
            {
                entity.Property(s => s.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(a => a.Reason).HasMaxLength(500);
                entity.HasIndex(a => new { a.VeterinarianId, a.AppointmentDateTime });
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.Email).HasDatabaseName("IX_ApplicationUser_Email");
            });

            // -----------------------------
            // SEEDING: ROLES, SPECIALTIES, ADMIN USER
            // -----------------------------
            SeedRoles(modelBuilder);
            SeedVetSpecialties(modelBuilder);
            SeedAdminUser(modelBuilder);
        }

        // -----------------------------
        // SEED ROLES
        // -----------------------------
        private void SeedRoles(ModelBuilder modelBuilder)
        {
            var adminRoleId = "role-admin-id";
            var vetRoleId = "role-vet-id";
            var receptionistRoleId = "role-receptionist-id";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = vetRoleId,
                    Name = "Veterinarian",
                    NormalizedName = "VETERINARIAN"
                },
                new IdentityRole
                {
                    Id = receptionistRoleId,
                    Name = "Receptionist",
                    NormalizedName = "RECEPTIONIST"
                }
            );
        }

        // -----------------------------
        // SEED SPECIALTIES
        // -----------------------------
        private void SeedVetSpecialties(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VetSpecialty>().HasData(
                new VetSpecialty { Id = 1, Name = "General Practice", Description = "General veterinary care." },
                new VetSpecialty { Id = 2, Name = "Surgery", Description = "Surgical procedures." },
                new VetSpecialty { Id = 3, Name = "Dentistry", Description = "Dental care for animals." }
            );
        }

        // -----------------------------
        // SEED ADMIN USER
        // -----------------------------
        private void SeedAdminUser(ModelBuilder modelBuilder)
        {
            var adminUserId = "user-admin-id";
            var adminRoleId = "role-admin-id";

            var hasher = new PasswordHasher<ApplicationUser>();

            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@animalcare.com",
                NormalizedUserName = "ADMIN@ANIMALCARE.COM",
                Email = "admin@animalcare.com",
                NormalizedEmail = "ADMIN@ANIMALCARE.COM",
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminUserId,
                    RoleId = adminRoleId
                }
            );
        }
    }
}
