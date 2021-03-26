using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Measurement
    {
        public int Id { get; set; }
        public float? Value { get; set; }
        public Category Category { get; set; }
        public Observation Observation { get; set; }

        public override string ToString()
        {
            if (Category.BaseCategory.Id == 1 || Category.BaseCategory.Id == 2 || Category.BaseCategory.Id == 3) 
            {
                return $"{Value} {Category.Unit.Abbreviation} {Category.Name}";
            }
            else
            {
                return $"{Value} {Category.Unit.Abbreviation} {Category.BaseCategory} med {Category.Name}";
            }
        }
    }
}
