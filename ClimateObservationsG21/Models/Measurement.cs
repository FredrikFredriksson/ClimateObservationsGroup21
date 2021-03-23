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
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ObservationId { get; set; }
        public Observation Observation { get; set; }
        

        public override string ToString()
        {
            return $"{Value} {Category.Unit.Abbreviation} {Category.Name}";
        }
    }
}
