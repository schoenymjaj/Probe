using ProbeDAL.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Probe.DAL
{
    public class ProbeDataContext : DbContext
    {
        public DbSet<GameType> GameType { get; set; }
        public DbSet<QuestionType> QuestionType { get; set; }
        public DbSet<ReportType> ReportType { get; set; }
        public DbSet<Person> Person { get; set; }

        public DbSet<Game> Game { get; set; }
        public DbSet<ConfigurationG> ConfigurationG { get; set; }
        public DbSet<GameConfiguration> GameConfiguration { get; set; }
        public DbSet<GameTypeConfiguration> GameTypeConfiguration { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<ChoiceQuestion> ChoiceQuestion { get; set; }
        public DbSet<Choice> Choice { get; set; }
        public DbSet<GameQuestion> GameQuestion { get; set; }

        public DbSet<Player> Player { get; set; }
        public DbSet<GameAnswer> GameAnswer { get; set; }


        static ProbeDataContext()
        {
            //Database.SetInitializer(new InitializeProbeDatabaseWithSeedData()); //MNS - NO NEED TO SEED IN PRODUCTION
        }

        public ProbeDataContext()
            : base(System.Configuration.ConfigurationManager.ConnectionStrings["Probe.Models.ProbeDataContext"].ConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        {
            dbModelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            dbModelBuilder.Entity<Person>().ToTable("Person");
            dbModelBuilder.Entity<Player>().ToTable("Player");
            dbModelBuilder.Entity<Question>().ToTable("Question");
            dbModelBuilder.Entity<ChoiceQuestion>().ToTable("ChoiceQuestion");
        }

        public override int SaveChanges()
        {
            return SaveChangesLocal(null);
        }

        public int SaveChanges(string windowsIdentityName)
        {
            return SaveChangesLocal(windowsIdentityName);
        }

        private int SaveChangesLocal(string windowsIdentityName)
        {

            foreach (var entity in ChangeTracker.Entries().
                Where(p => p.State == EntityState.Added || p.State == EntityState.Modified))
            {
                if (entity.State == EntityState.Added && entity.Entity is IDateCreated)
                {
                    ((IDateCreated)entity.Entity).DateCreated = DateTime.UtcNow;
                    ((ICreatedBy)entity.Entity).CreatedBy = windowsIdentityName;
                }
                if (entity.State == EntityState.Modified && entity.Entity is IDateUpdated)
                {
                    ((IDateCreated)entity.Entity).DateCreated = entity.GetDatabaseValues().GetValue<DateTime>("DateCreated");
                    ((ICreatedBy)entity.Entity).CreatedBy = entity.GetDatabaseValues().GetValue<string>("CreatedBy");
                    ((IDateUpdated)entity.Entity).DateUpdated = DateTime.UtcNow;
                    ((IUpdatedBy)entity.Entity).UpdatedBy = windowsIdentityName;
                }
            }

            return base.SaveChanges();

        }

    }
}