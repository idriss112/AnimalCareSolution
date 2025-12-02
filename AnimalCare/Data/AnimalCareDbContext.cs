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
        public DbSet<Receptionist> Receptionists { get; set; } = default!;

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

            // ---------------------------------------------------
            // APPLICATIONUSER ↔ RECEPTIONIST (1 ↔ 0 or 1)
            // ---------------------------------------------------
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Receptionist)
                .WithOne(r => r.User)
                .HasForeignKey<ApplicationUser>(u => u.ReceptionistId)
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

            modelBuilder.Entity<Owner>().HasData(
                        new Owner
                        {
                            Id = 1,
                            FirstName = "Sarah",
                            LastName = "Tremblay",
                            PhoneNumber = "514-987-2234",
                            Email = "sarah.tremblay@example.com",
                            Address = "215 Rue Sainte-Catherine Ouest, Montréal, QC"
                        },
                        new Owner
                        {
                            Id = 2,
                            FirstName = "Julien",
                            LastName = "Moreau",
                            PhoneNumber = "438-771-9023",
                            Email = "julien.moreau@example.com",
                            Address = "88 Av. du Mont-Royal Est, Montréal, QC"
                        },
                        new Owner
                        {
                            Id = 3,
                            FirstName = "Amira",
                            LastName = "El-Haddad",
                            PhoneNumber = "514-622-3381",
                            Email = "amira.haddad@example.com",
                            Address = "4020 Boulevard Décarie, Montréal, QC"
                        },
                        new Owner
                        {
                            Id = 4,
                            FirstName = "Kevin",
                            LastName = "Ouellet",
                            PhoneNumber = "581-300-7709",
                            Email = "kevin.ouellet@example.com",
                            Address = "1200 Rue Sherbrooke Ouest, Montréal, QC"
                        },
                        new Owner
                        {
                            Id = 5,
                            FirstName = "Layla",
                            LastName = "Benali",
                            PhoneNumber = "438-245-1940",
                            Email = "layla.benali@example.com",
                            Address = "59 Rue Jean-Talon Est, Montréal, QC"
                        }
                    );

            modelBuilder.Entity<Animal>().HasData(
        new Animal
        {
            Id = 1,
            Name = "Bella",
            Species = "Dog",
            Breed = "Labrador Retriever",
            DateOfBirth = new DateTime(2019, 5, 12),
            Sex = "Female",
            Weight = 28.5m,
            ImportantNotes = "Very friendly, good with children. Up to date on vaccines.",
            OwnerId = 1,
            CreatedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc)
        },
        new Animal
        {
            Id = 2,
            Name = "Max",
            Species = "Dog",
            Breed = "German Shepherd",
            DateOfBirth = new DateTime(2018, 11, 3),
            Sex = "Male",
            Weight = 32.2m,
            ImportantNotes = "Needs regular exercise. Slight anxiety with strangers.",
            OwnerId = 1,
            CreatedAt = new DateTime(2024, 1, 11, 0, 0, 0, DateTimeKind.Utc)
        },
        new Animal
        {
            Id = 3,
            Name = "Luna",
            Species = "Cat",
            Breed = "Siamese",
            DateOfBirth = new DateTime(2020, 2, 20),
            Sex = "Female",
            Weight = 4.3m,
            ImportantNotes = "Indoor-only cat. Sensitive stomach, special food required.",
            OwnerId = 2,
            CreatedAt = new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc)
        },
        new Animal
        {
            Id = 4,
            Name = "Milo",
            Species = "Cat",
            Breed = "British Shorthair",
            DateOfBirth = new DateTime(2021, 7, 8),
            Sex = "Male",
            Weight = 5.1m,
            ImportantNotes = "Calm temperament. Slight overweight, on diet plan.",
            OwnerId = 3,
            CreatedAt = new DateTime(2024, 1, 13, 0, 0, 0, DateTimeKind.Utc)
        },
        new Animal
        {
            Id = 5,
            Name = "Rocky",
            Species = "Dog",
            Breed = "Bulldog",
            DateOfBirth = new DateTime(2017, 9, 15),
            Sex = "Male",
            Weight = 24.8m,
            ImportantNotes = "Brachycephalic, monitor breathing during exercise.",
            OwnerId = 3,
            CreatedAt = new DateTime(2024, 1, 14, 0, 0, 0, DateTimeKind.Utc)
        },
        new Animal
        {
            Id = 6,
            Name = "Coco",
            Species = "Bird",
            Breed = "Cockatiel",
            DateOfBirth = new DateTime(2022, 4, 30),
            Sex = "Female",
            Weight = 0.09m,
            ImportantNotes = "Very vocal in the morning. Needs regular wing checks.",
            OwnerId = 4,
            CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        },
        new Animal
        {
            Id = 7,
            Name = "Nala",
            Species = "Dog",
            Breed = "Golden Retriever",
            DateOfBirth = new DateTime(2020, 6, 1),
            Sex = "Female",
            Weight = 26.7m,
            ImportantNotes = "Allergic to chicken-based food. Use hypoallergenic treats.",
            OwnerId = 5,
            CreatedAt = new DateTime(2024, 1, 16, 0, 0, 0, DateTimeKind.Utc)
        }
    );

        }
    }
}
