using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament_Management_Software.DataObjects
{
    public class WeightClass
    {
        public int Id { get; set; }
        public double MinimumWeight { get; set; }
        public double MaximumWeight { get; set; }
        public string ClassName { get; set; }

        public static double _previousMinWeight = 0;

        public ICollection<Contestant> Contestants { get; set; }
        public double getPrevious()
        {
            return _previousMinWeight;
        }
        public WeightClass(double max)
        {
            // TODO: ZMIENIC USTAWIENIE - obecnie przyjmuje posortowane juz rosnaco limity
            MinimumWeight = (_previousMinWeight != 0.00) ? _previousMinWeight + 0.01 : 0;
            _previousMinWeight = max;   
            MaximumWeight = max;
            ClassName = "FROM " + MinimumWeight + " TO " + MaximumWeight;
        }
        public WeightClass() { }
    }
}
