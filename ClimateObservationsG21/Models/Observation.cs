using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Observation
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ObserverId { get; set; }

        public int GeolocationId { get; set; }


        public override string ToString()
        {
            return Date.ToString();
        }
    }
}
