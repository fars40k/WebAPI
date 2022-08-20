using System;
using System.Data.Entity;

namespace WebAPI_App.Data
{
    public partial class WinTaskContext : DbContext
    {
        public WinTaskContext(string connectionString)
            : base(connectionString)
        {
        }

        public WinTaskContext()
            : base("WinTaskManager")
        {
        }

        public virtual DbSet<Goal> Goals { get; set; }
        public virtual DbSet<Person> Personel { get; set; }
        public virtual DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Person>()
                .HasMany(s => s.ProjectsWith)
                .WithMany(c => c.PersonelWith)
                .Map(cs =>
                {
                    cs.ToTable("PersonelToProjects");
                    cs.MapLeftKey("PersonID");
                    cs.MapRightKey("ProjectID");

                });

            modelBuilder.Entity<Person>()
              .HasMany(s => s.GoalsWith)
              .WithMany(c => c.PersonelWith)
              .Map(cs =>
              {
                  cs.ToTable("PersonelToGoals");
                  cs.MapLeftKey("PersonID");
                  cs.MapRightKey("GoalID");

              });

            modelBuilder.Entity<Goal>()
             .HasMany(s => s.ProjectsWith)
             .WithMany(c => c.GoalsIn)
             .Map(cs =>
             {
                 cs.ToTable("GoalsToProjects");
                 cs.MapLeftKey("GoalID");
                 cs.MapRightKey("ProjectID");

             });


            modelBuilder.Entity<Goal>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<Goal>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.FirstName)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.SurName)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.LastName)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.Division)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.Occupation)
                .IsFixedLength();

            modelBuilder.Entity<Project>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<Project>()
                .Property(e => e.Description)
                .IsUnicode(false);
        }


    }

}
