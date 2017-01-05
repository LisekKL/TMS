using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament_Management_Software.DataObjects;

namespace Tournament_Management_Software.Data_Access_Layer
{
    public class MatchInitializer : System.Data.Entity.DropCreateDatabaseAlways<MatchContext>
    {
        protected override void Seed(MatchContext context)
        {
            var contestants = new List<Contestant>
            {
                new Contestant (new DateTime(2005,10,15)) {FirstName = "Adam", LastName = "Adamiuk", Weight = 25},
                new Contestant (new DateTime(2002,01,16)) {FirstName = "Patryk", LastName = "Gęś", Weight = 21.75},
                new Contestant (new DateTime(2004,02,05)) {FirstName = "Ewa", LastName = "Bolec", Weight = 23.5},
                new Contestant (new DateTime(2003,02,23)) {FirstName = "Karol", LastName = "Kwaśniak", Weight = 22.5},
                new Contestant (new DateTime(2002,08,05)) {FirstName = "Bartłomiej", LastName = "Chrzeszczak", Weight = 22.5},
                new Contestant (new DateTime(2004,02,14)) {FirstName = "Marta", LastName = "Filipiec", Weight = 20},
                new Contestant (new DateTime(2005,03,29)) {FirstName = "Szymon", LastName = "Małek", Weight = 21.5},
                new Contestant (new DateTime(2006,04,18)) {FirstName = "Filip", LastName = "Lasota", Weight = 23.25}
            };

            contestants.ForEach(c => context.Contestants.Add(c));
            context.SaveChanges();
            
            var ageClasses = new List<AgeClass>
            {
                new AgeClass(1999,2001),
                new AgeClass(2002,2003),
                new AgeClass(2004,2005),
                new AgeClass(2006,2007),
                new AgeClass(2008,2010)
            };
            context.AgeClasses.AddRange(ageClasses);
            context.SaveChanges();

            var weightClasses = new List<WeightClass>();
            List<double> weightLimits = new List<double>() { 20, 22, 23, 24.5, 26, 27.5, 29 };
            foreach (var weight in weightLimits)
            {
                weightClasses.Add(new WeightClass(weight));
            }
            context.WeightClasses.AddRange(weightClasses);
            context.SaveChanges();
            //var weightclasses = new List<WeightClass>
            //{
            //    new WeightClass {ClassName = "Category_A", MaximumWeight = 20, MinimumWeight = 18.5},
            //    new WeightClass {ClassName = "Category_B", MaximumWeight = 22.5, MinimumWeight = 20},
            //    new WeightClass {ClassName = "Category_C", MaximumWeight = 25, MinimumWeight = 22.5}
            //};
            //weightclasses.ForEach(w => context.WeightClasses.Add(w));
            //context.SaveChanges();

        }
    }
}
