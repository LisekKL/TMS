using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament_Management_Software.DataObjects
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime? MatchDate { get; set; }
        public string MatchName { get; set; }
        
        public virtual ICollection<Contestant> Contestants { get; set; }

        public Match() { }

        public Match(Contestant cont1, Contestant cont2)
        {
            Contestants = new List<Contestant>();
            if (cont1 != null && cont2 != null)
            {
                Contestants.Add(cont1);
                Contestants.Add(cont2);
                MatchName = "Match " + Id + " --> " + cont1.FirstName + " " + cont1.LastName + " VS " + cont2.FirstName +
                            " " + cont2.LastName;
            }
        }
        public virtual AgeClass AgeClass { get; set; }
        public virtual WeightClass WeightClass { get; set; }
    }
}
