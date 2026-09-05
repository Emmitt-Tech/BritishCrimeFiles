using Microsoft.EntityFrameworkCore;

namespace UKCrimeWeb.Models
{
public class CrimeDbContext : DbContext    {
        public CrimeDbContext(DbContextOptions<CrimeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Person => Set<Person>();
        public DbSet<Programme> Programme => Set<Programme>();
        public DbSet<PersonProgramme> PersonProgramme => Set<PersonProgramme>();
        public DbSet<Case> Case => Set<Case>();
        public DbSet<Book> Books { get; set; }
        public DbSet<PersonBook> PersonBook => Set<PersonBook>();
        public DbSet<CasePerson> CasePerson => Set<CasePerson>();
        public DbSet<TimelineEvent> TimelineEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Programme>().ToTable("Programme");
            modelBuilder.Entity<PersonProgramme>().ToTable("PersonProgramme");
            modelBuilder.Entity<PersonBook>().ToTable("BookPerson");
            modelBuilder.Entity<Case>().ToTable("Case");
            modelBuilder.Entity<CasePerson>().ToTable("CasePerson");
            modelBuilder.Entity<Book>().ToTable("Book");

            modelBuilder.Entity<Person>().HasKey(p => p.PersonId);
            modelBuilder.Entity<Programme>().HasKey(p => p.ProgrammeId);
            modelBuilder.Entity<PersonProgramme>().HasKey(pp => new { pp.PersonId, pp.ProgrammeId });
            modelBuilder.Entity<PersonBook>().HasKey(pb => new { pb.PersonId, pb.BookId });
            modelBuilder.Entity<Book>().HasKey(b => b.BookId);

            modelBuilder.Entity<PersonProgramme>()
                .HasOne(pp => pp.Person)
                .WithMany()
                .HasForeignKey(pp => pp.PersonId);

            modelBuilder.Entity<PersonProgramme>()
                .HasOne(pp => pp.Programme)
                .WithMany()
                .HasForeignKey(pp => pp.ProgrammeId);


            modelBuilder.Entity<Case>().HasKey(c => c.CaseId);
            modelBuilder.Entity<CasePerson>().HasKey(cp => new { cp.CaseId, cp.PersonId });

            modelBuilder.Entity<CasePerson>()
                .HasOne(cp => cp.Case)
                .WithMany()
                .HasForeignKey(cp => cp.CaseId);

            modelBuilder.Entity<CasePerson>()
                .HasOne(cp => cp.Person)
                .WithMany()
                .HasForeignKey(cp => cp.PersonId);

            modelBuilder.Entity<PersonBook>()
                .HasOne(pb => pb.Person)
                .WithMany(p => p.PersonBooks)
                .HasForeignKey(pb => pb.PersonId);

            modelBuilder.Entity<PersonBook>()
                .HasOne(pb => pb.Book)
                .WithMany(b => b.PersonBooks)
                .HasForeignKey(pb => pb.BookId);
        }
    }
}