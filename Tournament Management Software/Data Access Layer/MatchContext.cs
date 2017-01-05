using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament_Management_Software.DataObjects;

namespace Tournament_Management_Software.Data_Access_Layer
{
    public class MatchContext : DbContext
    {
        public MatchContext() : base("TMSTestDB")
        {
            Database.SetInitializer<MatchContext>(new MatchInitializer());
        }
        public DbSet<Contestant> Contestants { get; set; }
        public DbSet<AgeClass> AgeClasses { get; set; }
        public DbSet<WeightClass> WeightClasses { get; set; }
        public DbSet<Match> Matches { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
