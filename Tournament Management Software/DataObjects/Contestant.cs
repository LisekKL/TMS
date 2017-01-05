using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament_Management_Software.DataObjects
{

    public class Contestant
    {
        public int Id { get; set; }
        //public int WeightClassId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        //public virtual WeightClass WeightClass { get; set; }
        public int? AgeId { get; set; }
        public int? WeightId { get; set; }

        public virtual AgeClass AgeClass { get; set; }
        public virtual WeightClass WeightClass { get; set; }
        public Contestant(DateTime date)
        {
            DateOfBirth = date;
            var currDate = DateTime.Now;
            Age = currDate.Year - date.Year;

        }
        public Contestant() { }
    }
}
