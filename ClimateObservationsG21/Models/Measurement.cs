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
        public int CategoryID { get; set; }
        public int ObservationId { get; set; }
    }
}
