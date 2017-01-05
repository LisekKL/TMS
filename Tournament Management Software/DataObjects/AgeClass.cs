using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament_Management_Software.DataObjects
{
    public class AgeClass
    {
        public int Id { get; set; }    
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        public string ClassName { get; set; }

        public AgeClass(int min, int max)
        {
            MinYear = min;
            MaxYear = max;
            ClassName = MinYear + " UNTIL " + MaxYear;
        }
        public AgeClass() { }
        //public List<Contestant> Contestants { get; set; }
        //public List<WeightClass> WeightClasses { get; set; }

        public virtual ICollection<Contestant> Contestants { get; set; }
    }
}
